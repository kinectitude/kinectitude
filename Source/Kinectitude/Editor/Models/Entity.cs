using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Views;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Statements;

namespace Kinectitude.Editor.Models
{
    internal delegate void NameChangedEventHandler(Entity entity, string oldName, string newName);

    internal sealed class Entity : GameModel<IEntityScope>, IAttributeScope, IComponentScope, IStatementScope
    {
        private string name;
        private bool prototype;
        private int nextAttribute;
        
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
        [DependsOn("Prototypes")]
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

        public ObservableCollection<Entity> Prototypes { get; private set; }
        public ObservableCollection<Attribute> Attributes { get; private set; }
        public ObservableCollection<Component> Components { get; private set; }
        public ObservableCollection<AbstractEvent> Events { get; private set; }

        //public EventCollection LocalEvents { get; private set; }

        public IEnumerable<Plugin> Plugins
        {
            get { return Components.Select(x => x.Plugin).Union(Events.SelectMany(x => x.Plugins)).Distinct(); }
        }

        public ICommand RenameCommand { get; private set; }
        public ICommand PropertiesCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand RemoveEventCommand { get; private set; }

        public Entity()
        {
            Prototypes = new ObservableCollection<Entity>();
            Attributes = new ObservableCollection<Attribute>();
            Components = new ObservableCollection<Component>();
            Events = new ObservableCollection<AbstractEvent>();

            //LocalEvents = new EventCollection(this);

            RenameCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowDialog(DialogService.Constants.NameDialog, new EntityRenameTransaction(this));
            });

            PropertiesCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowDialog(DialogService.Constants.EntityDialog, new EntityTransaction(Scope.Prototypes, this));
            });

            AddAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                CreateAttribute();
            });

            RemoveAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                Attribute attribute = parameter as Attribute;
                RemoveAttribute(attribute);
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
                    }
                }
            });

            RemoveEventCommand = new DelegateCommand(null, (parameter) =>
            {
                Event evt = parameter as Event;
                if (null != evt)
                {
                    RemoveEvent(evt);
                }
            });
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
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

        public void ClearPrototypes()
        {
            IEnumerable<Entity> prototypes = Prototypes.ToArray();

            foreach (Entity prototype in prototypes)
            {
                RemovePrototype(prototype);
            }
        }

        private void InheritAttribute(Attribute inheritedAttribute)
        {
            inheritedAttribute.KeyChanged += OnPrototypeAttributeKeyChanged;
            inheritedAttribute.PropertyChanged += OnPrototypeAttributePropertyChanged;

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
            inheritedAttribute.PropertyChanged -= OnPrototypeAttributePropertyChanged;

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
                attribute.Scope = this;
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
                attribute.Scope = null;
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

                component.Scope = this;
                Components.Add(component);

                Notify(new PluginUsed(component.Plugin));

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
                    component.Scope = null;
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
            evt.Scope = this;
            Events.Add(evt);

            foreach (Plugin plugin in evt.Plugins)
            {
                Notify(new PluginUsed(plugin));
            }

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

        private void OnPrototypeAttributePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (null != InheritedAttributeChanged && e.PropertyName == "Value")
            {
                Attribute attribute = sender as Attribute;
                InheritedAttributeChanged(attribute.Key);
            }
        }

        #region IPluginNamespace implementation

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

        public event AttributeEventHandler InheritedAttributeAdded;
        public event AttributeEventHandler InheritedAttributeRemoved;
        public event AttributeEventHandler InheritedAttributeChanged;

        public string GetInheritedValue(string key)
        {
            foreach (Entity prototype in Prototypes)
            {
                Attribute attribute = prototype.GetAttribute(key);
                if (null != attribute)
                {
                    return attribute.Value.ToString();
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

        public object GetInheritedValue(Plugin plugin, string name)
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

        #endregion

        private void OnPrototypeComponentLocalPropertyChanged(PluginProperty property)
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

        public Entity DeepCopy()
        {
            Entity copy = new Entity();

            foreach (Attribute attribute in this.Attributes)
            {
                copy.AddAttribute(attribute.DeepCopy());
            }

            foreach (Component component in this.Components)
            {
                copy.AddComponent(component.DeepCopy());
            }

            foreach (AbstractEvent evt in this.Events)
            {
                copy.AddEvent(evt.DeepCopy());
            }

            return copy;
        }

        public void InsertBefore(AbstractStatement statement, AbstractStatement toInsert)
        {

        }

        public void InsertPrototypeBefore(Entity prototype, Entity toInsert)
        {

        }
    }
}
