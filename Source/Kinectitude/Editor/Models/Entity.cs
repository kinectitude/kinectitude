using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using System.Windows.Input;
using System;
using Kinectitude.Core.Components;
using Kinectitude.Render;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Models
{
    internal delegate void NameChangedEventHandler(Entity entity, string oldName, string newName);

    internal sealed class Entity : BaseModel, IAttributeScope, IComponentScope, IEventScope
    {
        private string name;
        private IEntityScope scope;
        private int nextAttribute;

        public event ScopeChangedEventHandler ScopeChanged;
        public event PluginAddedEventHandler PluginAdded;
        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefineChanged;
        public event AttributeEventHandler InheritedAttributeAdded;
        public event AttributeEventHandler InheritedAttributeRemoved;
        public event AttributeEventHandler InheritedAttributeChanged;
        public event ComponentEventHandler InheritedComponentAdded;
        public event ComponentEventHandler InheritedComponentRemoved;
        public event PropertyEventHandler InheritedPropertyChanged;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value && !EntityNameExists(value))
                {
                    string oldName = name;

                    Workspace.Instance.CommandHistory.Log(
                        "rename entity",
                        () => Name = value,
                        () => Name = oldName
                    );

                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<Entity> Prototypes
        {
            get;
            private set;
        }

        public ObservableCollection<Attribute> Attributes
        {
            get;
            private set;
        }

        public ObservableCollection<Component> Components
        {
            get;
            private set;
        }

        public ObservableCollection<AbstractEvent> Events
        {
            get;
            private set;
        }

        public IEnumerable<Plugin> Plugins
        {
            get { return Components.Select(x => x.Plugin).Union(Events.SelectMany(x => x.Plugins)).Distinct(); }
        }

        public Entity()
        {
            Prototypes = new ObservableCollection<Entity>();
            Attributes = new ObservableCollection<Attribute>();
            Components = new ObservableCollection<Component>();
            Events = new ObservableCollection<AbstractEvent>();
        }

        public void SetScope(IEntityScope scope)
        {
            if (null != this.scope)
            {
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefineChanged -= OnDefinedNameChanged;
                this.scope.ScopeChanged -= OnScopeChanged;
            }

            this.scope = scope;

            if (null != this.scope)
            {
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefineChanged += OnDefinedNameChanged;
                this.scope.ScopeChanged += OnScopeChanged;
            }

            NotifyScopeChanged();
        }

        public void AddPrototype(Entity prototype)
        {
            if (null != prototype.Name)
            {
                Prototypes.Add(prototype);

                prototype.Attributes.CollectionChanged += OnPrototypeAttributesChanged;
                foreach (Attribute inheritedAttribute in prototype.Attributes)
                {
                    InheritAttribute(inheritedAttribute);
                }

                prototype.Components.CollectionChanged += OnPrototypeComponentsChanged;
                foreach (Component inheritedComponent in prototype.Components)
                {
                    InheritComponent(inheritedComponent);
                }

                prototype.Events.CollectionChanged += OnPrototypeEventsChanged;
                foreach (AbstractEvent inheritedEvent in prototype.Events)
                {
                    InheritEvent(inheritedEvent);
                }

                Workspace.Instance.CommandHistory.Log(
                    "change entity prototype",
                    () => AddPrototype(prototype),
                    () => RemovePrototype(prototype)
                );
            }
        }

        public void RemovePrototype(Entity prototype)
        {
            Prototypes.Remove(prototype);

            prototype.Attributes.CollectionChanged -= OnPrototypeAttributesChanged;
            foreach (Attribute inheritedAttribute in prototype.Attributes)
            {
                DisinheritAttribute(inheritedAttribute);
            }

            prototype.Components.CollectionChanged -= OnPrototypeComponentsChanged;
            foreach (Component inheritedComponent in prototype.Components)
            {
                DisinheritComponent(inheritedComponent);
            }

            prototype.Events.CollectionChanged -= OnPrototypeEventsChanged;
            foreach (AbstractEvent inheritedEvent in prototype.Events)
            {
                DisinheritEvent(inheritedEvent);
            }

            Workspace.Instance.CommandHistory.Log(
                "change entity prototype",
                () => RemovePrototype(prototype),
                () => AddPrototype(prototype)
            );
        }

        private void InheritAttribute(Attribute inheritedAttribute)
        {
            inheritedAttribute.KeyChanged += OnPrototypeAttributeKeyChanged;

            Attribute localAttribute = GetAttribute(inheritedAttribute.Key);
            if (null == localAttribute)
            {
                localAttribute = new Attribute(inheritedAttribute.Key) { IsInherited = true };
                AddAttribute(localAttribute);
            }

            if (null != InheritedAttributeAdded)
            {
                InheritedAttributeAdded(inheritedAttribute.Key);
            }
        }

        private void DisinheritAttribute(Attribute inheritedAttribute)
        {
            inheritedAttribute.KeyChanged -= OnPrototypeAttributeKeyChanged;

            Attribute localAttribute = GetAttribute(inheritedAttribute.Key);
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
                foreach (Attribute inheritedAttribute in args.NewItems)
                {
                    InheritAttribute(inheritedAttribute);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Attribute inheritedAttribute in args.OldItems)
                {
                    DisinheritAttribute(inheritedAttribute);
                }
            }
        }

        private void OnPrototypeAttributeKeyChanged(string oldKey, string newKey)
        {
            Attribute inheritedAttributeWithOldKey = null;

            foreach (Entity prototype in Prototypes)
            {
                inheritedAttributeWithOldKey = prototype.GetAttribute(oldKey);
                if (null != inheritedAttributeWithOldKey)
                {
                    break;
                }
            }

            if (null == inheritedAttributeWithOldKey)
            {
                Attribute localAttribute = GetAttribute(oldKey);
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

            Attribute localAttributeWithNewKey = GetAttribute(newKey);
            if (null == localAttributeWithNewKey)
            {
                localAttributeWithNewKey = new Attribute(newKey) { IsInherited = true };
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
            Attribute inheritedAttributeWithOldKey = null;

            foreach (Entity prototype in Prototypes)
            {
                inheritedAttributeWithOldKey = prototype.GetAttribute(oldKey);
                if (null != inheritedAttributeWithOldKey)
                {
                    InheritAttribute(inheritedAttributeWithOldKey);
                }
            }
        }

        public Attribute GetAttribute(string key)
        {
            return Attributes.FirstOrDefault(x => x.Key == key);
        }

        public void AddAttribute(Attribute attribute)
        {
            if (!HasLocalAttribute(attribute.Key))
            {
                attribute.SetScope(this);
                attribute.KeyChanged += OnLocalAttributeKeyChanged;
                Attributes.Add(attribute);

                Workspace.Instance.CommandHistory.Log(
                    "add attribute '" + attribute.Key + "'",
                    () => AddAttribute(attribute),
                    () => RemoveAttribute(attribute)
                );
            }
        }

        public void RemoveAttribute(Attribute attribute)
        {
            if (attribute.IsLocal || attribute.IsInherited && !attribute.CanInherit)
            {
                attribute.SetScope(null);
                attribute.KeyChanged -= OnLocalAttributeKeyChanged;
                Attributes.Remove(attribute);

                Workspace.Instance.CommandHistory.Log(
                    "remove attribute '" + attribute.Key + "'",
                    () => RemoveAttribute(attribute),
                    () => AddAttribute(attribute)
                );
            }
        }

        public void CreateAttribute()
        {
            Attribute attribute = new Attribute(GetNextAttributeKey());
            AddAttribute(attribute);
        }

        private void InheritComponent(Component inheritedComponent)
        {
            inheritedComponent.LocalPropertyChanged += OnPrototypeComponentLocalPropertyChanged;

            Component localComponent = GetComponentByRole(inheritedComponent.Provides);
            if (null == localComponent)
            {
                localComponent = new Component(inheritedComponent.Plugin);
                AddComponent(localComponent);
            }

            if (null != InheritedComponentAdded)
            {
                InheritedComponentAdded(inheritedComponent.Plugin);
            }
        }

        private void DisinheritComponent(Component inheritedComponent)
        {
            inheritedComponent.LocalPropertyChanged -= OnPrototypeComponentLocalPropertyChanged;

            Component localComponent = GetComponentByRole(inheritedComponent.Provides);
            if (null != localComponent && !localComponent.HasLocalProperties)
            {
                RemoveComponent(localComponent);
            }

            if (null != InheritedComponentRemoved)
            {
                InheritedComponentRemoved(inheritedComponent.Plugin);
            }
        }

        public void OnPrototypeComponentsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Component component in args.NewItems)
                {
                    InheritComponent(component);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Component component in args.OldItems)
                {
                    DisinheritComponent(component);
                }
            }
        }

        public void AddComponent(Component component)
        {
            if (!HasComponentWithRole(component.Provides))
            {
                foreach (string require in component.Requires)
                {
                    Plugin plugin = GetPlugin(require);

                    if (plugin.Type == PluginType.Component)
                    {
                        Component requiredComponent = new Component(plugin);
                        AddComponent(requiredComponent);
                    }
                }

                component.SetScope(this);
                Components.Add(component);

                NotifyPluginAdded(component.Plugin);

                Workspace.Instance.CommandHistory.Log(
                    "add component '" + component.DisplayName + "'",
                    () => AddComponent(component),
                    () => RemoveComponent(component)
                );
            }
        }

        public void RemoveComponent(Component component)
        {
            if (component.IsRoot)
            {
                if (!Components.Any(x => x.DependsOn(component)))
                {
                    component.SetScope(null);
                    Components.Remove(component);

                    Workspace.Instance.CommandHistory.Log(
                        "remove component '" + component.DisplayName + "'",
                        () => RemoveComponent(component),
                        () => AddComponent(component)
                    );
                }
            }
        }

        public Component GetComponentByRole(string provides)
        {
            return Components.FirstOrDefault(x => x.Provides == provides);
        }

        public Component GetComponentByRole(Type type)
        {
            return GetComponentByRole(type.FullName);
        }

        public Component GetComponentByType(string type)
        {
            return Components.FirstOrDefault(x => x.Type == type);
        }

        public Component GetComponentByType(Type type)
        {
            return Components.FirstOrDefault(x => x.Plugin.CoreType == type);
        }

        public bool HasComponentWithRole(string provides)
        {
            return Components.Any(x => x.Provides == provides);
        }

        public bool HasComponentWithRole(Type type)
        {
            return HasComponentWithRole(type.FullName);
        }

        public bool HasComponentOfType(string type)
        {
            return Components.Any(x => x.Type == type);
        }

        public bool HasComponentOfType(Type type)
        {
            return Components.Any(x => x.Plugin.CoreType == type);
        }

        public void AddEvent(AbstractEvent evt)
        {
            evt.SetScope(this);
            evt.PluginAdded += OnEventPluginAdded;
            Events.Add(evt);

            Workspace.Instance.CommandHistory.Log(
                "add event '" + evt.Header + "'",
                () => AddEvent(evt),
                () => RemoveEvent(evt)
            );
        }

        public void RemoveEvent(AbstractEvent evt)
        {
            if (evt.IsLocal)
            {
                PrivateRemoveEvent(evt);

                Workspace.Instance.CommandHistory.Log(
                    "remove event '" + evt.Header + "'",
                    () => RemoveEvent(evt),
                    () => AddEvent(evt)
                );
            }
        }

        private void PrivateRemoveEvent(AbstractEvent evt)
        {
            evt.SetScope(null);
            evt.PluginAdded -= OnEventPluginAdded;
            Events.Remove(evt);
        }

        private string GetNextAttributeKey()
        {
            string ret = "attribute" + nextAttribute;

            while (HasInheritedAttribute(ret) || HasLocalAttribute(ret))
            {
                nextAttribute++;
                ret = "attribute" + nextAttribute;
            }

            return ret;
        }

        public bool EntityNameExists(string name)
        {
            return null != scope ? scope.EntityNameExists(name) : false;
        }

        private void NotifyPluginAdded(Plugin plugin)
        {
            if (null != PluginAdded)
            {
                PluginAdded(plugin);
            }
        }

        private void OnEventPluginAdded(Plugin plugin)
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

        public Plugin GetPlugin(string name)
        {
            return null != scope ? scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }

        private void OnDefineAdded(Define define)
        {
            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        private void OnDefinedNameChanged(Plugin plugin, string newName)
        {
            if (null != DefineChanged)
            {
                DefineChanged(plugin, newName);
            }
        }

        private void OnPrototypeAttributePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (null != InheritedAttributeChanged && args.PropertyName == "Value")
            {
                Attribute attribute = sender as Attribute;
                InheritedAttributeChanged(attribute.Key);
            }
        }

        string IPluginNamespace.GetDefinedName(Plugin plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        string IAttributeScope.GetInheritedValue(string key)
        {
            foreach (Entity prototype in Prototypes)
            {
                Attribute attribute = prototype.GetAttribute(key);
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

        public bool HasRootComponent(Plugin plugin)
        {
            return Components.Any(x => x.Plugin == plugin);
        }

        public bool HasInheritedComponent(Plugin plugin)
        {
            return Prototypes.SelectMany(x => x.Components).Any(x => x.Plugin == plugin);
        }

        object IComponentScope.GetInheritedValue(Plugin plugin, string name)
        {
            foreach (Entity prototype in Prototypes)
            {
                Component component = prototype.Components.FirstOrDefault(x => x.Plugin == plugin);
                if (null != component)
                {
                    Property property = component.GetProperty(name);
                    if (null != property)
                    {
                        return property.Value;
                    }
                }
            }
            return null;
        }

        private void OnPrototypeComponentLocalPropertyChanged(string name)
        {
            if (null != InheritedPropertyChanged)
            {
                InheritedPropertyChanged(name);
            }
        }

        private void OnPrototypeEventsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractEvent inheritedEvent in args.NewItems)
                {
                    InheritEvent(inheritedEvent);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractEvent inheritedEvent in args.OldItems)
                {
                    DisinheritEvent(inheritedEvent);
                }
            }
        }

        private void InheritEvent(AbstractEvent evt)
        {
            AbstractEvent localEvent = Events.FirstOrDefault(x => x.InheritsFrom(evt));
            if (null == localEvent)
            {
                localEvent = new InheritedEvent(evt);
                AddEvent(localEvent);
            }
        }

        private void DisinheritEvent(AbstractEvent evt)
        {
            AbstractEvent localEvent = Events.FirstOrDefault(x => x.InheritsFrom(evt));
            if (null != localEvent)
            {
                PrivateRemoveEvent(localEvent);
            }
        }
    }
}
