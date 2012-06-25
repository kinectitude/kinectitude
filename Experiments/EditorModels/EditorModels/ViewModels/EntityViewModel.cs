﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using EditorModels.Base;
using EditorModels.Models;
using EditorModels.ViewModels.Interfaces;
using System;
using System.ComponentModel;

namespace EditorModels.ViewModels
{
    internal delegate void NameChangedEventHandler(EntityViewModel entity, string oldName, string newName);

    internal sealed class EntityViewModel : BaseViewModel, IAttributeScope, IComponentScope
    {
        private readonly Entity entity;
        private IEntityContainer container;
        private IEntityScope scope;
        private int nextAttribute;

#if TEST

        public Entity Entity
        {
            get { return entity; }
        }

#endif

        public event ScopeChangedEventHandler ScopeChanged;
        public event NameChangedEventHandler NameChanged;
        
        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefinedNameChanged;
        
        public event PluginAddedEventHandler PluginAdded;

        public event KeyEventHandler InheritedAttributeAdded;
        public event KeyEventHandler InheritedAttributeRemoved;
        public event KeyEventHandler InheritedAttributeChanged;

        public string Name
        {
            get { return entity.Name; }
            set
            {
                if (entity.Name != value)
                {
                    if (!EntityNameExists(value))
                    {
                        string oldName = entity.Name;
                        entity.Name = value;

                        if (null != NameChanged)
                        {
                            NameChanged(this, oldName, entity.Name);
                        }

                        NotifyPropertyChanged("Name");
                    }
                }
            }
        }

        public ObservableCollection<EntityViewModel> Prototypes
        {
            get;
            private set;
        }

        public ObservableCollection<AttributeViewModel> Attributes
        {
            get;
            private set;
        }

        public ObservableCollection<ComponentViewModel> Components
        {
            get;
            private set;
        }

        public ObservableCollection<EventViewModel> Events
        {
            get;
            private set;
        }

        public IEnumerable<PluginViewModel> Plugins
        {
            get { return Components.Select(x => x.Plugin).Union(Events.SelectMany(x => x.Plugins)); }
        }

        public ICommand AddPrototypeCommand
        {
            get;
            private set;
        }

        public ICommand RemovePrototypeCommand
        {
            get;
            private set;
        }

        public ICommand AddAttributeCommand
        {
            get;
            private set;
        }

        public ICommand RemoveAttributeCommand
        {
            get;
            private set;
        }

        public ICommand AddComponentCommand
        {
            get;
            private set;
        }

        public ICommand RemoveComponentCommand
        {
            get;
            private set;
        }

        public ICommand AddEventCommand
        {
            get;
            private set;
        }

        public ICommand RemoveEventCommand
        {
            get;
            private set;
        }

        public EntityViewModel()
        {
            entity = new Entity();

            Prototypes = new ObservableCollection<EntityViewModel>();
            Attributes = new ObservableCollection<AttributeViewModel>();
            Components = new ObservableCollection<ComponentViewModel>();
            Events = new ObservableCollection<EventViewModel>();

            AddPrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel prototype = parameter as EntityViewModel;
                    if (null != prototype)
                    {
                        AddPrototype(prototype);
                    }
                }
            );

            RemovePrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel prototype = parameter as EntityViewModel;
                    if (null != prototype)
                    {
                        RemovePrototype(prototype);
                    }
                }
            );

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    AttributeViewModel attribute = new AttributeViewModel(GetNextAttributeKey());
                    AddAttribute(attribute);
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    RemoveAttribute(parameter as AttributeViewModel);
                }
            );

            AddComponentCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    PluginViewModel plugin = parameter as PluginViewModel;
                    if (null != plugin)
                    {
                        ComponentViewModel component = new ComponentViewModel(plugin);
                        AddComponent(component);
                    }
                }
            );

            RemoveComponentCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    ComponentViewModel component = parameter as ComponentViewModel;
                    if (null != component)
                    {
                        RemoveComponent(component);
                    }
                }
            );

            AddEventCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    PluginViewModel plugin = parameter as PluginViewModel;
                    if (null != plugin)
                    {
                        EventViewModel evt = new EventViewModel(plugin);
                        AddEvent(evt);
                    }
                }
            );

            RemoveEventCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EventViewModel evt = parameter as EventViewModel;
                    if (null != evt)
                    {
                        RemoveEvent(evt);
                    }
                }
            );
        }

        public void SetScope(IEntityContainer container, IEntityScope scope)
        {
            if (null != this.scope)
            {
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefinedNameChanged -= OnDefinedNameChanged;
                this.scope.ScopeChanged -= OnScopeChanged;
            }

            if (null != this.container)
            {
                this.container.RemoveEntity(entity);
            }

            this.scope = scope;
            this.container = container;

            if (null != this.container)
            {
                this.container.AddEntity(entity);
            }

            if (null != this.scope)
            {
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefinedNameChanged += OnDefinedNameChanged;
                this.scope.ScopeChanged += OnScopeChanged;
            }

            NotifyScopeChanged();
        }

        public void AddPrototype(EntityViewModel prototype)
        {
            if (null != prototype.Name)
            {
                entity.AddPrototype(prototype.Name);
                Prototypes.Add(prototype);

                prototype.Attributes.CollectionChanged += OnPrototypeAttributesChanged;
                foreach (AttributeViewModel inheritedAttribute in prototype.Attributes)
                {
                    InheritAttribute(inheritedAttribute);
                }

                prototype.Components.CollectionChanged += OnPrototypeComponentsChanged;
                foreach (ComponentViewModel inheritedComponent in prototype.Components)
                {
                    InheritComponent(inheritedComponent);
                }

                prototype.NameChanged += OnPrototypeNameChanged;
            }
        }

        public void RemovePrototype(EntityViewModel prototype)
        {
            entity.RemovePrototype(prototype.Name);
            Prototypes.Remove(prototype);

            prototype.Attributes.CollectionChanged -= OnPrototypeAttributesChanged;
            foreach (AttributeViewModel inheritedAttribute in prototype.Attributes)
            {
                DisinheritAttribute(inheritedAttribute);
            }

            prototype.Components.CollectionChanged -= OnPrototypeComponentsChanged;
            foreach (ComponentViewModel inheritedComponent in prototype.Components)
            {
                DisinheritComponent(inheritedComponent);
            }

            prototype.NameChanged -= OnPrototypeNameChanged;
        }

        private void InheritAttribute(AttributeViewModel inheritedAttribute)
        {
            inheritedAttribute.KeyChanged += OnPrototypeAttributeKeyChanged;

            AttributeViewModel localAttribute = GetAttribute(inheritedAttribute.Key);
            if (null == localAttribute)
            {
                localAttribute = new AttributeViewModel(inheritedAttribute.Key) { IsInherited = true };
                AddAttribute(localAttribute);
            }

            if (null != InheritedAttributeAdded)
            {
                InheritedAttributeAdded(inheritedAttribute.Key);
            }
        }

        private void DisinheritAttribute(AttributeViewModel inheritedAttribute)
        {
            inheritedAttribute.KeyChanged -= OnPrototypeAttributeKeyChanged;

            AttributeViewModel localAttribute = GetAttribute(inheritedAttribute.Key);
            if (null != localAttribute && localAttribute.IsInherited)
            {
                RemoveAttribute(localAttribute);
            }

            if (null != InheritedAttributeRemoved)
            {
                InheritedAttributeRemoved(inheritedAttribute.Key);
            }   
        }

        private void OnPrototypeAttributesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AttributeViewModel inheritedAttribute in args.NewItems)
                {
                    InheritAttribute(inheritedAttribute);
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AttributeViewModel inheritedAttribute in args.OldItems)
                {
                    DisinheritAttribute(inheritedAttribute);
                }
            }
        }

        private void OnPrototypeAttributeKeyChanged(string oldKey, string newKey)
        {
            AttributeViewModel inheritedAttributeWithOldKey = null;

            foreach (EntityViewModel prototype in Prototypes)
            {
                inheritedAttributeWithOldKey = prototype.GetAttribute(oldKey);
                if (null != inheritedAttributeWithOldKey)
                {
                    break;
                }
            }

            if (null == inheritedAttributeWithOldKey)
            {
                AttributeViewModel localAttribute = GetAttribute(oldKey);
                if (null != localAttribute && localAttribute.IsInherited)
                {
                    RemoveAttribute(localAttribute);

                    if (null != InheritedAttributeRemoved)
                    {
                        InheritedAttributeRemoved(oldKey);
                    }
                }
            }
            else if (null != InheritedAttributeChanged)
            {
                InheritedAttributeChanged(oldKey);
            }

            AttributeViewModel localAttributeWithNewKey = GetAttribute(newKey);
            if (null == localAttributeWithNewKey)
            {
                localAttributeWithNewKey = new AttributeViewModel(newKey) { IsInherited = true };
                AddAttribute(localAttributeWithNewKey);

                if (null != InheritedAttributeAdded)
                {
                    InheritedAttributeAdded(newKey);
                }
            }
            else if (null != InheritedAttributeChanged)
            {
                InheritedAttributeChanged(newKey);
            }
        }

        private void OnLocalAttributeKeyChanged(string oldKey, string newKey)
        {
            AttributeViewModel inheritedAttributeWithOldKey = null;

            foreach (EntityViewModel prototype in Prototypes)
            {
                inheritedAttributeWithOldKey = prototype.GetAttribute(oldKey);
                if (null != inheritedAttributeWithOldKey)
                {
                    InheritAttribute(inheritedAttributeWithOldKey);
                }
            }
        }

        public AttributeViewModel GetAttribute(string key)
        {
            return Attributes.FirstOrDefault(x => x.Key == key);
        }

        public void AddAttribute(AttributeViewModel attribute)
        {
            if (!HasLocalAttribute(attribute.Key))
            {
                attribute.SetScope(entity, this);
                attribute.KeyChanged += OnLocalAttributeKeyChanged;
                Attributes.Add(attribute);
            }
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            if (attribute.IsLocal || attribute.IsInherited && !attribute.CanInherit)
            {
                attribute.SetScope(null, null);
                attribute.KeyChanged -= OnLocalAttributeKeyChanged;
                Attributes.Remove(attribute);
            }
        }

        private void InheritComponent(ComponentViewModel inheritedComponent)
        {
            ComponentViewModel localComponent = GetComponentByRole(inheritedComponent.Provides);
            if (null == localComponent)
            {
                localComponent = new ComponentViewModel(inheritedComponent.Plugin);
                AddComponent(localComponent);
            }
            UpdateComponentInheritance(localComponent);
        }

        private void DisinheritComponent(ComponentViewModel inheritedComponent)
        {
            ComponentViewModel localComponent = GetComponentByRole(inheritedComponent.Provides);
            if (null != localComponent)
            {
                UpdateComponentInheritance(localComponent);
            }
        }

        public void OnPrototypeComponentsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ComponentViewModel component in args.NewItems)
                {
                    InheritComponent(component);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ComponentViewModel component in args.OldItems)
                {
                    DisinheritComponent(component);
                }
            }
        }

        private void UpdateComponentInheritance(ComponentViewModel component)
        {
            foreach (EntityViewModel prototype in Prototypes)
            {
                ComponentViewModel inheritedComponent = prototype.GetComponentByType(component.Type);
                if (null != inheritedComponent)
                {
                    component.SetInheritedComponent(inheritedComponent);
                    return;
                }
            }

            component.SetInheritedComponent(null);
            if (!component.CanInherit && component.IsInherited)
            {
                RemoveComponent(component);
            }
        }

        public void AddComponent(ComponentViewModel component)
        {
            if (!HasComponentWithRole(component.Provides))
            {
                foreach (string require in component.Requires)
                {
                    ComponentViewModel requiredComponent = new ComponentViewModel(GetPlugin(require));
                    AddComponent(requiredComponent);
                }

                UpdateComponentInheritance(component);

                component.SetScope(entity, this);
                Components.Add(component);

                NotifyPluginAdded(component.Plugin);
            }
        }

        public void RemoveComponent(ComponentViewModel component)
        {
            if (component.IsLocal || component.IsInherited && !component.CanInherit)
            {
                if (!Components.Any(x => x.DependsOn(component)))
                {
                    component.SetScope(null, null);
                    Components.Remove(component);
                }
            }
        }

        public ComponentViewModel GetComponentByRole(string provides)
        {
            return Components.FirstOrDefault(x => x.Provides == provides);
        }

        public ComponentViewModel GetComponentByType(string type)
        {
            return Components.FirstOrDefault(x => x.Type == type);
        }

        public bool HasComponentWithRole(string provides)
        {
            return Components.Any(x => x.Provides == provides);
        }

        public bool HasComponentWithType(string type)
        {
            return Components.Any(x => x.Type == type);
        }

        public void AddEvent(EventViewModel evt)
        {
            evt.SetEntity(entity);
            evt.PluginAdded += OnEventPluginAdded;
            Events.Add(evt);
        }

        public void RemoveEvent(EventViewModel evt)
        {
            evt.SetEntity(null);
            evt.PluginAdded -= OnEventPluginAdded;
            Events.Remove(evt);
        }

        private string GetNextAttributeKey()
        {
            return string.Format("attribute{0}", nextAttribute++);
        }

        private void OnPrototypeNameChanged(EntityViewModel prototype, string oldName, string newName)
        {
            entity.ReplacePrototype(oldName, newName);
        }

        public bool EntityNameExists(string name)
        {
            return null != scope ? scope.EntityNameExists(name) : false;
        }

        private void NotifyPluginAdded(PluginViewModel plugin)
        {
            if (null != PluginAdded)
            {
                PluginAdded(plugin);
            }
        }

        private void OnEventPluginAdded(PluginViewModel plugin)
        {
            NotifyPluginAdded(plugin);
        }

        private void NotifyScopeChanged()
        {
            if (null != ScopeChanged)
            {
                ScopeChanged();
            }
        }

        private void OnScopeChanged()
        {
            NotifyScopeChanged();
        }

        public PluginViewModel GetPlugin(string name)
        {
            return null != scope ? scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }

        private void OnDefineAdded(DefineViewModel define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefinedNameChanged(PluginViewModel plugin, string newName)
        {
            if (null != DefinedNameChanged)
            {
                DefinedNameChanged(plugin, newName);
            }
        }

        private void OnPrototypeAttributePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (null != InheritedAttributeChanged && args.PropertyName == "Value")
            {
                AttributeViewModel attribute = sender as AttributeViewModel;
                InheritedAttributeChanged(attribute.Key);
            }
        }

        string IPluginNamespace.GetDefinedName(PluginViewModel plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        string IAttributeScope.GetInheritedValue(string key)
        {
            foreach (EntityViewModel prototype in Prototypes)
            {
                AttributeViewModel attribute = prototype.GetAttribute(key);
                if (null != attribute)
                {
                    return attribute.Value;
                }
            }

            return null;
        }

        public bool HasInheritedAttribute(string key)
        {
            return Prototypes.Any(x => x.HasLocalAttribute(key));
        }

        public bool HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Key == key);
        }
    }
}
