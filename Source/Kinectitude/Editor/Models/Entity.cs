//-----------------------------------------------------------------------
// <copyright file="Entity.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Exceptions;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Transactions;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Views.Dialogs;
using Kinectitude.Editor.Views.Scenes.Presenters;
using Kinectitude.Editor.Views.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Event = Kinectitude.Editor.Models.Statements.Events.Event;

namespace Kinectitude.Editor.Models
{
    internal delegate void AttributeChangedEventHandler(Attribute attribute, string oldName);

    internal sealed class Entity : GameModel<IEntityScope>, IAttributeScope, IComponentScope, IStatementScope
    {
        private string name;
        private bool prototype;
        private int nextAttribute;
        private Attribute selectedAttribute;
        private bool shouldCopyStatement;
        
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    if (value.ContainsWhitespace())
                    {
                        throw new InvalidEntityNameException(value);
                    }

                    if (EntityNameExists(value))
                    {
                        throw new EntityNameExistsException(value);
                    }

                    if (IsDefinedName(value))
                    {
                        throw new NameDefinedException(value);
                    }

                    string oldName = name;
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        if (IsPrototype)
                        {
                            throw new InvalidPrototypeNameException();
                        }

                        value = null;
                    }

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

        public bool IsPrototype
        {
            get { return prototype; }
            set
            {
                if (prototype != value)
                {
                    prototype = value;
                    NotifyPropertyChanged("IsPrototype");
                }
            }
        }

        [DependsOn("Name")]
        public string DisplayName
        {
            get
            {
                if (null != name)
                {
                    return name;
                }

                if (Prototypes.Count > 0)
                {
                    return "<" + string.Join(" ", Prototypes.Select(x => x.Name)) + ">";
                }

                return "<Unnamed Entity>";
            }
        }

        public Attribute SelectedAttribute
        {
            get { return selectedAttribute; }
            set
            {
                if (selectedAttribute != value)
                {
                    selectedAttribute = value;
                    NotifyPropertyChanged("SelectedAttribute");
                }
            }
        }

        public IEnumerable<Plugin> Plugins
        {
            get { return Components.Select(x => x.Plugin).Union(Events.SelectMany(x => x.Plugins)).Distinct(); }
        }

        public int Index
        {
            get { return null != Scope ? Scope.IndexOf(this) : -1; }
        }

        public bool ShouldCopyStatement
        {
            get { return shouldCopyStatement; }
            set
            {
                if (shouldCopyStatement != value)
                {
                    shouldCopyStatement = value;
                    NotifyPropertyChanged("ShouldCopyStatement");
                }
            }
        }

        public EntityPresenter Presenter { get; private set; }

        public ObservableCollection<Entity> Prototypes { get; private set; }
        public ObservableCollection<Attribute> Attributes { get; private set; }
        public ObservableCollection<Component> Components { get; private set; }
        public ObservableCollection<AbstractEvent> Events { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand PropertiesCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand RemoveEventCommand { get; private set; }
        public ICommand DeleteStatementCommand { get; private set; }

        public Entity()
        {
            Prototypes = new ObservableCollection<Entity>();
            Attributes = new ObservableCollection<Attribute>();
            Components = new ObservableCollection<Component>();
            Events = new ObservableCollection<AbstractEvent>();

            RenameCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowDialog<NameDialog>(new EntityRenameTransaction(this));
            });

            PropertiesCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowDialog<EntityDialog>(new EntityTransaction(AvailablePrototypes(), this));
            });

            AddAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                var attribute = CreateAttribute();

                Workspace.Instance.CommandHistory.Log(
                    "add attribute '" + attribute.Name + "'",
                    () => AddAttribute(attribute),
                    () => RemoveAttribute(attribute)
                );
            });

            RemoveAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                if (null != selectedAttribute)
                {
                    var attribute = selectedAttribute;
                    RemoveAttribute(attribute);

                    Workspace.Instance.CommandHistory.Log(
                        "remove attribute '" + attribute.Name + "'",
                        () => RemoveAttribute(attribute),
                        () => AddAttribute(attribute)
                    );
                }
            });

            AddEventCommand = new DelegateCommand(null, (parameter) =>
            {
                StatementFactory factory = parameter as StatementFactory;
                if (null != factory)
                {
                    if (factory.Type == StatementType.Event)
                    {
                        Event evt = factory.CreateStatement() as Event;
                        AddEvent(evt);

                        Workspace.Instance.CommandHistory.Log(
                            "add event '" + evt.Type + "'",
                            () => AddEvent(evt),
                            () => RemoveEvent(evt)
                        );
                    }
                }
            });

            RemoveEventCommand = new DelegateCommand(null, (parameter) =>
            {
                Event evt = parameter as Event;
                if (null != evt)
                {
                    RemoveEvent(evt);

                    Workspace.Instance.CommandHistory.Log(
                        "remove event '" + evt.Type + "'",
                        () => RemoveEvent(evt),
                        () => AddEvent(evt)
                    );
                }
            });

            DeleteStatementCommand = new DelegateCommand(null, (parameter) =>
            {
                var statement = (AbstractStatement)parameter;
                var oldParent = statement.Scope;
                int idx = oldParent.IndexOf(statement);
                statement.RemoveFromParent();

                Workspace.Instance.CommandHistory.Log(
                    "remove statement",
                    () => statement.RemoveFromParent(),
                    () => oldParent.InsertAt(idx, statement)
                );
            });

            Presenter = new EntityPresenter(this);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private IEnumerable<Entity> AvailablePrototypes()
        {
            return null != Scope ? Scope.Prototypes : Enumerable.Empty<Entity>();
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

                NotifyPropertyChanged("DisplayName");
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

            NotifyPropertyChanged("DisplayName");
        }

        public void ClearPrototypes()
        {
            IEnumerable<Entity> prototypes = Prototypes.ToArray();

            foreach (Entity prototype in prototypes)
            {
                RemovePrototype(prototype);
            }
        }

        public void ClearComponents()
        {
            var components = Components.ToArray();

            foreach (var component in components)
            {
                RemoveComponent(component);
            }
        }

        private void InheritAttribute(Attribute inheritedAttribute)
        {
            inheritedAttribute.NameChanged += OnPrototypeAttributeNameChanged;
            inheritedAttribute.PropertyChanged += OnPrototypeAttributePropertyChanged;

            EnsureAttributeExists(inheritedAttribute.Name);

            if (null != InheritedAttributeAdded)
            {
                InheritedAttributeAdded(inheritedAttribute.Name);
            }
        }

        private void DisinheritAttribute(Attribute inheritedAttribute)
        {
            inheritedAttribute.NameChanged -= OnPrototypeAttributeNameChanged;
            inheritedAttribute.PropertyChanged -= OnPrototypeAttributePropertyChanged;

            CheckAttributeInheritance(inheritedAttribute.Name);

            if (null != InheritedAttributeRemoved)
            {
                InheritedAttributeRemoved(inheritedAttribute.Name);
            }
        }

        private void EnsureAttributeExists(string key)
        {
            var localAttribute = GetAttribute(key);
            if (null == localAttribute)
            {
                localAttribute = new Attribute(key) { IsInherited = true };
                PrivateAddAttribute(localAttribute);
            }
            else if (!localAttribute.IsInherited)
            {
                localAttribute.IsInherited = true;
            }
        }

        private void CheckAttributeInheritance(string key)
        {
            var localAttribute = GetAttribute(key);
            if (null != localAttribute)
            {
                if (HasInheritedAttribute(key))
                {
                    if (!localAttribute.IsInherited)
                    {
                        localAttribute.IsInherited = true;
                    }
                }
                else if (localAttribute.HasOwnValue)
                {
                    if (localAttribute.IsInherited)
                    {
                        localAttribute.IsInherited = false;
                    }
                }
                else
                {
                    PrivateRemoveAttribute(localAttribute);
                }
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

        private void OnPrototypeAttributeNameChanged(string oldKey, string newKey)
        {
            CheckAttributeInheritance(oldKey);
            EnsureAttributeExists(newKey);
        }

        public Attribute GetAttribute(string key)
        {
            return Attributes.FirstOrDefault(x => x.Name == key);
        }

        private void PrivateAddAttribute(Attribute attribute)
        {
            attribute.Scope = this;
            Attributes.Add(attribute);
        }

        public void AddAttribute(Attribute attribute)
        {
            if (HasLocalAttribute(attribute.Name) || HasInheritedAttribute(attribute.Name))
            {
                throw new AttributeNameExistsException(attribute.Name);
            }

            PrivateAddAttribute(attribute);
        }

        private void PrivateRemoveAttribute(Attribute attribute)
        {
            attribute.Scope = null;
            Attributes.Remove(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            if (!attribute.IsInherited)
            {
                PrivateRemoveAttribute(attribute);
            }
        }

        public Attribute CreateAttribute()
        {
            var attribute = new Attribute(GetNextAttributeKey());
            AddAttribute(attribute);
            return attribute;
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
            if (null != localComponent && !localComponent.HasOwnValues)
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

                component.Scope = this;
                Components.Add(component);

                Notify(new PluginUsed(component.Plugin));
            }
        }

        public void RemoveComponent(Component component)
        {
            if (component.IsRoot)
            {
                if (!Components.Any(x => x.DependsOn(component)))
                {
                    var definedType = component.Type;

                    component.Scope = null;
                    Components.Remove(component);
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
            PrivateAddEvent(Events.Count, evt);
        }

        private void PrivateAddEvent(int idx, AbstractEvent evt)
        {
            evt.Scope = this;
            Events.Insert(idx, evt);

            foreach (Plugin plugin in evt.Plugins)
            {
                Notify(new PluginUsed(plugin));
            }
        }

        public void RemoveEvent(AbstractEvent evt)
        {
            if (evt.IsEditable)
            {
                PrivateRemoveEvent(evt);
            }
        }

        private void PrivateRemoveEvent(AbstractEvent evt)
        {
            evt.Scope = null;
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
            return null != Scope ? Scope.EntityNameExists(name) : false;
        }

        public bool IsDefinedName(string name)
        {
            return null != Scope ? Scope.HasDefinedName(name) : false;
        }

        private void OnPrototypeAttributePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (null != InheritedAttributeChanged && e.PropertyName == "Value")
            {
                Attribute attribute = sender as Attribute;
                InheritedAttributeChanged(attribute.Name);
            }
        }

        public void RemoveFromScope()
        {
            if (null != Scope)
            {
                Scope.RemoveEntity(this);
            }
        }

        #region IPluginNamespace implementation

        public bool HasDefinedName(string name)
        {
            return null != Scope ? Scope.HasDefinedName(name) : false;
        }

        public string GetDefinedName(Plugin plugin)
        {
            return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        public Plugin GetPlugin(string name)
        {
            return null != Scope ? Scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }

        #endregion

        #region IAttributeScope implementation

        Entity IAttributeScope.Entity
        {
            get { return this; }
        }

        Scene IAttributeScope.Scene
        {
            get { return null; }
        }

        Game IAttributeScope.Game
        {
            get { return null; }
        }

        public event AttributeEventHandler InheritedAttributeAdded;
        public event AttributeEventHandler InheritedAttributeRemoved;
        public event AttributeEventHandler InheritedAttributeChanged;

        public Value GetInheritedValue(string key)
        {
            foreach (Entity prototype in Prototypes)
            {
                var attribute = prototype.GetAttribute(key);
                if (null != attribute && attribute.HasOwnOrInheritedValue)
                {
                    return attribute.Value;
                }
            }

            return Attribute.DefaultValue;
        }

        public bool HasInheritedAttribute(string key)
        {
            return Prototypes.Any(x => x.HasLocalAttribute(key));
        }

        public bool HasInheritedNonDefaultAttribute(string key)
        {
            var localAttribute = GetAttribute(key);
            if (null != localAttribute && localAttribute.HasOwnValue)
            {
                return true;
            }

            foreach (var prototype in Prototypes)
            {
                if (prototype.HasInheritedNonDefaultAttribute(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Name == key);
        }

        #endregion

        #region IComponentScope implementation

        public event ComponentEventHandler InheritedComponentAdded;
        public event ComponentEventHandler InheritedComponentRemoved;
        public event PropertyEventHandler InheritedPropertyChanged;

        public bool HasRootComponent(Plugin plugin)
        {
            return Components.Any(x => x.Plugin == plugin);
        }

        public bool HasInheritedComponent(Plugin plugin)
        {
            return Prototypes.SelectMany(x => x.Components).Any(x => x.Plugin == plugin);
        }

        public bool HasInheritedNonDefaultProperty(Plugin plugin, PluginProperty property)
        {
            var localComponent = Components.FirstOrDefault(x => x.Plugin == plugin);
            if (null != localComponent)
            {
                var localProperty = localComponent.Properties.FirstOrDefault(x => x.PluginProperty == property);
                if (null != localProperty)
                {
                    if (localProperty.HasOwnValue)
                    {
                        return true;
                    }
                }
            }

            foreach (var prototype in Prototypes)
            {
                if (prototype.HasInheritedNonDefaultProperty(plugin, property))
                {
                    return true;
                }
            }

            return false;
        }

        public Value GetInheritedValue(Plugin plugin, PluginProperty pluginProperty)
        {
            foreach (Entity prototype in Prototypes)
            {
                Component component = prototype.Components.FirstOrDefault(x => x.Plugin == plugin);
                if (null != component)
                {
                    Property property = component.GetProperty(pluginProperty.Name);
                    if (property.HasOwnOrInheritedValue)
                    {
                        return property.Value;
                    }
                }
            }

            return pluginProperty.DefaultValue;
        }

        #endregion

        private void OnPrototypeComponentLocalPropertyChanged(Component component, PluginProperty property)
        {
            if (null != InheritedPropertyChanged)
            {
                InheritedPropertyChanged(property);
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
                localEvent = new ReadOnlyEvent(evt);
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

        public Entity DeepCopy()
        {
            Entity copy = new Entity(); // Name is not copied since the copy must have a unique/anonymous name to be pasted in the same scene

            foreach (Entity prototype in this.Prototypes)
            {
                copy.AddPrototype(prototype);
            }

            foreach (Attribute attribute in this.Attributes)
            {
                if (attribute.HasOwnValue)
                {
                    copy.AddAttribute(attribute.DeepCopy());
                }
            }

            foreach (Component component in this.Components)
            {
                if (component.HasOwnValues || component.IsRoot)
                {
                    copy.AddComponent(component.DeepCopy());
                }
            }

            foreach (AbstractEvent evt in this.Events)
            {
                if (!evt.IsReadOnly)
                {
                    copy.AddEvent(evt.DeepCopy());
                }
            }

            return copy;
        }

        #region IStatementScope implementation

        int IStatementScope.IndexOf(AbstractStatement statement)
        {
            int idx = -1;
            var evt = statement as AbstractEvent;
            if (null != evt)
            {
                idx = Events.IndexOf(evt);
            }

            return idx;
        }
        
        void IStatementScope.RemoveStatement(AbstractStatement statement)
        {
            RemoveEvent((AbstractEvent)statement);
        }

        void IStatementScope.InsertAt(int idx, AbstractStatement statement)
        {
            if (idx != -1)
            {
                var evt = statement as AbstractEvent;
                if (null != evt)
                {
                    evt.RemoveFromParent();
                    PrivateAddEvent(idx, evt);
                }
            }
        }

        void IStatementScope.InsertBefore(AbstractStatement statement, AbstractStatement toInsert)
        {
            int idx = Events.IndexOf((AbstractEvent)statement);
            if (idx != -1)
            {
                var evt = toInsert as AbstractEvent;
                if (null != evt)
                {
                    evt.RemoveFromParent();
                    PrivateAddEvent(idx, evt);
                }
            }
        }

        #endregion
    }
}
