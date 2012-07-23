using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.ViewModels.Interfaces;
using System.Windows.Input;
using System;

namespace Kinectitude.Editor.ViewModels
{
    internal delegate void NameChangedEventHandler(EntityViewModel entity, string oldName, string newName);

    internal sealed class EntityViewModel : BaseViewModel, IAttributeScope, IComponentScope, IEventScope
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

        public ObservableCollection<AbstractEventViewModel> Events
        {
            get;
            private set;
        }

        public IEnumerable<PluginViewModel> Plugins
        {
            get { return Components.Select(x => x.Plugin).Union(Events.SelectMany(x => x.Plugins)).Distinct(); }
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

        public double X
        {
            get
            {
                double ret = 0;

                ComponentViewModel transformComponent = GetComponentByCoreType(typeof(Kinectitude.Core.Components.TransformComponent));
                if (null != transformComponent)
                {
                    AbstractPropertyViewModel x = transformComponent.GetProperty("X");
                    if (null != x)
                    {
                        double.TryParse(x.Value.ToString(), out ret);
                    }
                }

                return ret;
            }
        }

        public double Y
        {
            get
            {
                double ret = 0;

                ComponentViewModel transformComponent = GetComponentByCoreType(typeof(Kinectitude.Core.Components.TransformComponent));
                if (null != transformComponent)
                {
                    AbstractPropertyViewModel y = transformComponent.GetProperty("Y");
                    if (null != y)
                    {
                        double.TryParse(y.Value.ToString(), out ret);
                    }
                }

                return ret;
            }
        }

        public double Width
        {
            get
            {
                double ret = 0;

                ComponentViewModel transformComponent = GetComponentByCoreType(typeof(Kinectitude.Core.Components.TransformComponent));
                if (null != transformComponent)
                {
                    AbstractPropertyViewModel width = transformComponent.GetProperty("Width");
                    if (null != width)
                    {
                        double.TryParse(width.Value.ToString(), out ret);
                    }
                }

                return ret;
            }
        }

        public double Height
        {
            get
            {
                double ret = 0;

                ComponentViewModel transformComponent = GetComponentByCoreType(typeof(Kinectitude.Core.Components.TransformComponent));
                if (null != transformComponent)
                {
                    AbstractPropertyViewModel height = transformComponent.GetProperty("Height");
                    if (null != height)
                    {
                        double.TryParse(height.Value.ToString(), out ret);
                    }
                }

                return ret;
            }
        }

        public EntityViewModel()
        {
            Prototypes = new ObservableCollection<EntityViewModel>();
            Attributes = new ObservableCollection<AttributeViewModel>();
            Components = new ObservableCollection<ComponentViewModel>();
            Events = new ObservableCollection<AbstractEventViewModel>();

            AddPrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    EntityViewModel prototype = parameter as EntityViewModel;
                    if (null != prototype)
                    {
                        Workspace.Instance.CommandHistory.Log(
                            "change entity prototype",
                            () => AddPrototype(prototype),
                            () => RemovePrototype(prototype)
                        );

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
                        Workspace.Instance.CommandHistory.Log(
                            "change entity prototype",
                            () => RemovePrototype(prototype),
                            () => AddPrototype(prototype)
                        );

                        RemovePrototype(prototype);
                    }
                }
            );

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    AttributeViewModel attribute = new AttributeViewModel(GetNextAttributeKey());

                    Workspace.Instance.CommandHistory.Log(
                        "add attribute '" + attribute.Key + "'",
                        () => AddAttribute(attribute),
                        () => RemoveAttribute(attribute)
                    );

                    AddAttribute(attribute);
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    AttributeViewModel attribute = parameter as AttributeViewModel;

                    Workspace.Instance.CommandHistory.Log(
                        "remove attribute '" + attribute.Key + "'",
                        () => RemoveAttribute(attribute),
                        () => AddAttribute(attribute)
                    );

                    RemoveAttribute(attribute);
                }
            );

            AddComponentCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    PluginViewModel plugin = parameter as PluginViewModel;
                    if (null != plugin)
                    {
                        ComponentViewModel component = new ComponentViewModel(plugin);

                        Workspace.Instance.CommandHistory.Log(
                            "add component '" + component.DisplayName + "'",
                            () => AddComponent(component),
                            () => RemoveComponent(component)
                        );

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
                        Workspace.Instance.CommandHistory.Log(
                            "remove component '" + component.DisplayName + "'",
                            () => RemoveComponent(component),
                            () => AddComponent(component)
                        );

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

                        Workspace.Instance.CommandHistory.Log(
                            "add event '" + evt.DisplayName + "'",
                            () => AddEvent(evt),
                            () => RemoveEvent(evt)
                        );

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
                        Workspace.Instance.CommandHistory.Log(
                            "remove event '" + evt.DisplayName + "'",
                            () => RemoveEvent(evt),
                            () => AddEvent(evt)
                        );

                        RemoveEvent(evt);
                    }
                }
            );
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

        public void AddPrototype(EntityViewModel prototype)
        {
            if (null != prototype.Name)
            {
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

                prototype.Events.CollectionChanged += OnPrototypeEventsChanged;
                foreach (AbstractEventViewModel inheritedEvent in prototype.Events)
                {
                    InheritEvent(inheritedEvent);
                }
            }
        }

        public void RemovePrototype(EntityViewModel prototype)
        {
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

            prototype.Events.CollectionChanged -= OnPrototypeEventsChanged;
            foreach (AbstractEventViewModel inheritedEvent in prototype.Events)
            {
                DisinheritEvent(inheritedEvent);
            }
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
            else if (args.Action == NotifyCollectionChangedAction.Remove)
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
                attribute.SetScope(this);
                attribute.KeyChanged += OnLocalAttributeKeyChanged;
                Attributes.Add(attribute);
            }
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            if (attribute.IsLocal || attribute.IsInherited && !attribute.CanInherit)
            {
                attribute.SetScope(null);
                attribute.KeyChanged -= OnLocalAttributeKeyChanged;
                Attributes.Remove(attribute);
            }
        }

        private void InheritComponent(ComponentViewModel inheritedComponent)
        {
            inheritedComponent.LocalPropertyChanged += OnPrototypeComponentLocalPropertyChanged;

            ComponentViewModel localComponent = GetComponentByRole(inheritedComponent.Provides);
            if (null == localComponent)
            {
                localComponent = new ComponentViewModel(inheritedComponent.Plugin);
                AddComponent(localComponent);
            }

            if (null != InheritedComponentAdded)
            {
                InheritedComponentAdded(inheritedComponent.Plugin);
            }
        }

        private void DisinheritComponent(ComponentViewModel inheritedComponent)
        {
            inheritedComponent.LocalPropertyChanged -= OnPrototypeComponentLocalPropertyChanged;

            ComponentViewModel localComponent = GetComponentByRole(inheritedComponent.Provides);
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

        public void AddComponent(ComponentViewModel component)
        {
            if (!HasComponentWithRole(component.Provides))
            {
                foreach (string require in component.Requires)
                {
                    ComponentViewModel requiredComponent = new ComponentViewModel(GetPlugin(require));
                    AddComponent(requiredComponent);
                }

                component.SetScope(this);
                Components.Add(component);

                NotifyPluginAdded(component.Plugin);
            }
        }

        public void RemoveComponent(ComponentViewModel component)
        {
            if (component.IsRoot)
            {
                if (!Components.Any(x => x.DependsOn(component)))
                {
                    component.SetScope(null);
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

        public ComponentViewModel GetComponentByCoreType(Type type)
        {
            return Components.FirstOrDefault(x => x.Plugin.CoreType == type);
        }

        public bool HasComponentWithRole(string provides)
        {
            return Components.Any(x => x.Provides == provides);
        }

        public bool HasComponentWithType(string type)
        {
            return Components.Any(x => x.Type == type);
        }

        public void AddEvent(AbstractEventViewModel evt)
        {
            evt.SetScope(this);
            evt.PluginAdded += OnEventPluginAdded;
            Events.Add(evt);
        }

        public void RemoveEvent(AbstractEventViewModel evt)
        {
            if (evt.IsLocal)
            {
                PrivateRemoveEvent(evt);
            }
        }

        private void PrivateRemoveEvent(AbstractEventViewModel evt)
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
            if (null != DefineChanged)
            {
                DefineChanged(plugin, newName);
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

        public bool HasRootComponent(PluginViewModel plugin)
        {
            return Components.Any(x => x.Plugin == plugin);
        }

        public bool HasInheritedComponent(PluginViewModel plugin)
        {
            return Prototypes.SelectMany(x => x.Components).Any(x => x.Plugin == plugin);
        }

        object IComponentScope.GetInheritedValue(PluginViewModel plugin, string name)
        {
            foreach (EntityViewModel prototype in Prototypes)
            {
                ComponentViewModel component = prototype.Components.FirstOrDefault(x => x.Plugin == plugin);
                if (null != component)
                {
                    PropertyViewModel property = component.GetProperty(name);
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
                foreach (AbstractEventViewModel inheritedEvent in args.NewItems)
                {
                    InheritEvent(inheritedEvent);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractEventViewModel inheritedEvent in args.OldItems)
                {
                    DisinheritEvent(inheritedEvent);
                }
            }
        }

        private void InheritEvent(AbstractEventViewModel evt)
        {
            AbstractEventViewModel localEvent = Events.FirstOrDefault(x => x.InheritsFrom(evt));
            if (null == localEvent)
            {
                localEvent = new InheritedEventViewModel(evt);
                AddEvent(localEvent);
            }
        }

        private void DisinheritEvent(AbstractEventViewModel evt)
        {
            AbstractEventViewModel localEvent = Events.FirstOrDefault(x => x.InheritsFrom(evt));
            if (null != localEvent)
            {
                PrivateRemoveEvent(localEvent);
            }
        }
    }
}
