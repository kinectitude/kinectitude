using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.Commands.Entity;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Plugins;
using Kinectitude.Editor.Views;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class AttributeAvailableEventArgs : EventArgs
    {
        private readonly string oldKey;
        private readonly string newKey;

        public string OldKey
        {
            get { return oldKey; }
        }

        public string NewKey
        {
            get { return newKey; }
        }

        public AttributeAvailableEventArgs(string oldKey, string newKey)
        {
            this.oldKey = oldKey;
            this.newKey = newKey;
        }
    }

    internal sealed class EntityViewModel : BaseModel
    {
        //private const string RenderComponent = "Kinectitude.Render.RenderComponent";
        //private const string Shape = "Shape";
        //private const string Ellipse = "Ellipse";

        private static readonly Dictionary<Entity, EntityViewModel> entities;

        static EntityViewModel()
        {
            entities = new Dictionary<Entity, EntityViewModel>();
        }

        public static EntityViewModel GetViewModel(Entity entity)
        {
            EntityViewModel viewModel = null;
            entities.TryGetValue(entity, out viewModel);
            if (null == viewModel)
            {
                viewModel = new EntityViewModel(entity);
                entities[entity] = viewModel;
            }
            return viewModel;
        }

        private int autoIncrement;
        private PluginViewModel selectedComponent;
        private readonly Entity entity;
        private readonly ObservableCollection<EntityViewModel> _prototypes;
        private readonly ObservableCollection<EntityAttributeViewModel> _attributes;
        private readonly ObservableCollection<ComponentViewModel> _components;
        private readonly ObservableCollection<EventViewModel> _events;
        private readonly ModelCollection<EntityViewModel> prototypes;
        private readonly ModelCollection<EntityAttributeViewModel> attributes;
        private readonly ModelCollection<ComponentViewModel> components;
        private readonly ModelCollection<EventViewModel> events;

        public event EventHandler<AttributeAvailableEventArgs> AttributeAvailable = delegate { };

        public Entity Entity
        {
            get { return entity; }
        }

        public string Name
        {
            get { return entity.Name; }
            set
            {
                if (entity.Name != value)
                {
                    CommandHistory.LogCommand(new RenameEntityCommand(this, value));
                    entity.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public ModelCollection<EntityAttributeViewModel> Attributes
        {
            get { return attributes; }
        }

        public ModelCollection<ComponentViewModel> Components
        {
            get { return components; }
        }

        public ModelCollection<EventViewModel> Events
        {
            get { return events; }
        }

        public PluginViewModel SelectedComponent
        {
            get { return selectedComponent; }
            set
            {
                if (selectedComponent != value)
                {
                    selectedComponent = value;
                    RaisePropertyChanged("SelectedComponent");
                }
            }
        }

        /*public int X
        {
            get
            {
                Attribute attribute = entity.GetAttribute("x");
                return null != attribute ? entity.GetAttribute("x").Value : 0;
            }
            set
            {
                entity.SetAttribute("x", value);
                RaisePropertyChanged("X");
            }
        }

        public int Y
        {
            get
            {
                Attribute attribute = entity.GetAttribute("y");
                return null != attribute ? entity.GetAttribute("y").Value : 0;
            }
            set
            {
                entity.SetAttribute("y", value);
                RaisePropertyChanged("Y");
            }
        }

        public int Width
        {
            get
            {
                Attribute attribute = entity.GetAttribute("width");
                return null != attribute ? entity.GetAttribute("width").Value : 0;
            }
            set
            {
                entity.SetAttribute("width", value);
                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get
            {
                Attribute attribute = entity.GetAttribute("height");
                return null != attribute ? entity.GetAttribute("height").Value : 0;
            }
            set
            {
                entity.GetAttribute("height").Value = value;
                RaisePropertyChanged("Height");
            }
        }

        public bool IsRenderable
        {
            get
            {
                return true;
            }
        }

        public bool IsRectangular
        {
            get
            {
                bool ret = true;
                Component component = entity.GetComponent("Kinectitude.Render.RenderComponent");
                if (null != component)
                {
                    TextProperty property = component.GetProperty<TextProperty>("Shape");
                    if (null != property && property.Value == "Ellipse")
                    {
                        ret = false;
                    }
                }

                return !HasText && ret;
            }
        }

        public bool IsElliptical
        {
            get { return !HasText && !IsRectangular; }
        }

        public string Text
        {
            get
            {
                string text = string.Empty;
                Component component = entity.GetComponent("Kinectitude.Render.TextRenderComponent");
                if (null != component)
                {
                    TextProperty property = component.GetProperty<TextProperty>("Value");
                    if (null != property)
                    {
                        text = property.Value;
                    }
                }

                return text;
            }
        }

        public bool HasText
        {
            get { return Text != string.Empty; }
        }

        public string Color
        {
            get
            {
                string color = string.Empty;
                Component component = entity.GetComponent("Kinectitude.Render.RenderComponent");
                if (null != component)
                {
                    TextProperty property = component.GetProperty<TextProperty>("Fillcolor");
                    color = property.Value ?? color;
                }
                if (color == string.Empty)
                {
                    component = entity.GetComponent("Kinectitude.Render.TextRenderComponent");
                    if (null != component)
                    {
                        TextProperty property = component.GetProperty<TextProperty>("Fontcolor");
                        color = property.Value ?? color;
                    }
                }

                return color;
            }
        }*/

        public ModelCollection<EntityViewModel> Prototypes
        {
            get { return prototypes; }
        }

        public ICommand AddAttributeCommand
        {
            get { return new DelegateCommand(null, ExecuteAddAttributeCommand); }
        }

        public ICommand RemoveAttributeCommand
        {
            get { return new DelegateCommand(null, ExecuteRemoveAttributeCommand); }
        }

        public ICommand AddComponentCommand
        {
            get { return new DelegateCommand(null, ExecuteAddComponentCommand); }
        }

        public ICommand RemoveComponentCommand
        {
            get { return new DelegateCommand(null, ExecuteRemoveComponentCommand); }
        }

        public ICommand AddPrototypeCommand
        {
            get { return new DelegateCommand(null, ExecuteAddPrototypeCommand); }
        }

        public ICommand RemovePrototypeCommand
        {
            get { return new DelegateCommand(null, ExecuteRemovePrototypeCommand); }
        }

        public ICommand ConvertToPrototypeCommand
        {
            get { return new DelegateCommand(null, ExecuteConvertToPrototypeCommand); }
        }

        public ICommand AddEventCommand
        {
            get { return new DelegateCommand(null, ExecuteAddEventCommand); }
        }

        public ICommand RemoveEventCommand
        {
            get { return new DelegateCommand(null, ExecuteRemoveEventCommand); }
        }

        private EntityViewModel(Entity entity)
        {
            this.entity = entity;

            var prototypeViewModels = from prototype in entity.Prototypes select EntityViewModel.GetViewModel(prototype);
            var attributeViewModels = from attribute in entity.Attributes select new EntityAttributeViewModel(entity, attribute.Key);
            var componentViewModels = from component in entity.Components select new ComponentViewModel(entity, component.Descriptor);
            var eventViewModels = from evt in entity.Events select new EventViewModel(evt);

            _prototypes = new ObservableCollection<EntityViewModel>();
            _attributes = new ObservableCollection<EntityAttributeViewModel>(attributeViewModels);
            _components = new ObservableCollection<ComponentViewModel>(componentViewModels);
            _events = new ObservableCollection<EventViewModel>(eventViewModels);

            foreach (EntityViewModel prototype in prototypeViewModels)
            {
                PrivateAddPrototype(prototype);
            }

            prototypes = new ModelCollection<EntityViewModel>(_prototypes);
            attributes = new ModelCollection<EntityAttributeViewModel>(_attributes);
            components = new ModelCollection<ComponentViewModel>(_components);
            events = new ModelCollection<EventViewModel>(_events);
        }

        public void ExecuteAddAttributeCommand(object parameter)
        {
            EntityAttributeViewModel attribute = new EntityAttributeViewModel(entity, string.Format("attribute{0}", autoIncrement++));
            AddAttribute(attribute);
        }

        public void ExecuteRemoveAttributeCommand(object parameter)
        {
            EntityAttributeViewModel attribute = parameter as EntityAttributeViewModel;
            if (null != attribute)
            {
                RemoveAttribute(attribute);
            }
        }

        public void ExecuteAddComponentCommand(object parameter)
        {
            ModalDialogService.ShowDialog<EntityViewModel>(ModalDialogService.Constants.ComponentDialog, this, (result) =>
            {
                if (true == result)
                {
                    PluginDescriptor descriptor = selectedComponent.Descriptor;
                    ComponentViewModel componentViewModel = new ComponentViewModel(entity, descriptor);
                    AddComponent(componentViewModel);
                }
            });
        }

        public void ExecuteRemoveComponentCommand(object parameter)
        {

        }

        public void ExecuteAddPrototypeCommand(object parameter)
        {
            EntityViewModel entityViewModel = parameter as EntityViewModel;
            if (null != entityViewModel)
            {
                AddPrototype(entityViewModel);
            }
        }

        public void ExecuteRemovePrototypeCommand(object parameter)
        {
            EntityViewModel entityViewModel = parameter as EntityViewModel;
            if (null != entityViewModel)
            {
                RemovePrototype(entityViewModel);
            }
        }

        public void ExecuteConvertToPrototypeCommand(object parameter)
        {

        }

        public void ExecuteAddEventCommand(object parameter)
        {

        }

        public void ExecuteRemoveEventCommand(object parameter)
        {
            
        }

        public void AddAttribute(EntityAttributeViewModel attribute)
        {
            CommandHistory.LogCommand(new AddAttributeCommand(this, attribute));
            attribute.AddAttribute();
            _attributes.Add(attribute);
        }

        public void RemoveAttribute(EntityAttributeViewModel attribute)
        {
            CommandHistory.LogCommand(new RemoveAttributeCommand(this, attribute));
            attribute.RemoveAttribute();
            _attributes.Remove(attribute);
        }

        public void AddPrototype(EntityViewModel entityViewModel)
        {
            CommandHistory.LogCommand(new AddPrototypeCommand(this, entityViewModel));
            entity.AddPrototype(entityViewModel.Entity);
            PrivateAddPrototype(entityViewModel);
        }

        public void RemovePrototype(EntityViewModel entityViewModel)
        {
            CommandHistory.LogCommand(new RemovePrototypeCommand(this, entityViewModel));
            entity.RemovePrototype(entityViewModel.Entity);
            PrivateRemovePrototype(entityViewModel);
        }

        public void AddComponent(ComponentViewModel componentViewModel)
        {
            //CommandHistory.LogCommand(new AddComponentCommand(this, componentViewModel));
            componentViewModel.AddComponent();
            _components.Add(componentViewModel);
        }

        public void RemoveComponent(ComponentViewModel componentViewModel)
        {
            //CommandHistory.LogCommand(new RemoveComponentCommand(this, componentViewModel));
            componentViewModel.RemoveComponent();
            _components.Remove(componentViewModel);
        }

        public EntityAttributeViewModel GetEntityAttributeViewModel(string key)
        {
            return attributes.FirstOrDefault(x => x.Key == key);
        }

        public ComponentViewModel GetComponentViewModel(PluginDescriptor descriptor)
        {
            return components.FirstOrDefault(x => x.Descriptor == descriptor);
        }

        private void PrivateAddPrototype(EntityViewModel prototype)
        {
            prototype.AttributeAvailable += OnAttributeAvailable;
            prototype.Attributes.CollectionChanged += OnAttributesChanged;
            prototype.Components.CollectionChanged += OnComponentsChanged;
            prototype.Events.CollectionChanged += OnEventsChanged;

            foreach (EntityAttributeViewModel inheritedAttribute in prototype.Attributes)
            {
                EntityAttributeViewModel localAttribute = _attributes.FirstOrDefault(x => x.Key == inheritedAttribute.Key);
                if (null == localAttribute)
                {
                    localAttribute = new EntityAttributeViewModel(entity, inheritedAttribute.Key);
                    _attributes.Add(localAttribute);
                }
                localAttribute.FindInheritedViewModel();
            }

            // TODO components and events

            foreach (ComponentViewModel inheritedComponent in prototype.Components)
            {
                ComponentViewModel localComponent = _components.FirstOrDefault(x => x.Descriptor == inheritedComponent.Descriptor);
                if (null == localComponent)
                {
                    localComponent = new ComponentViewModel(entity, inheritedComponent.Descriptor);
                    _components.Add(localComponent);
                }
            }

            foreach (EventViewModel inheritedEvent in prototype.Events)
            {

            }

            _prototypes.Add(prototype);
        }

        private void PrivateRemovePrototype(EntityViewModel prototype)
        {
            prototype.AttributeAvailable -= OnAttributeAvailable;
            prototype.Attributes.CollectionChanged -= OnAttributesChanged;
            prototype.Components.CollectionChanged -= OnComponentsChanged;
            prototype.Events.CollectionChanged -= OnEventsChanged;

            foreach (EntityAttributeViewModel inheritedAttribute in prototype.Attributes)
            {
                EntityAttributeViewModel localAttribute = _attributes.FirstOrDefault(x => x.Key == inheritedAttribute.Key);
                if (null != localAttribute)
                {
                    localAttribute.FindInheritedViewModel();
                }
            }

            foreach (ComponentViewModel inheritedComponent in prototype.Components)
            {
                ComponentViewModel localComponent = _components.FirstOrDefault(x => x.Descriptor == inheritedComponent.Descriptor);
                if (null != localComponent)
                {
                    // TODO Recalc parent
                }
            }

            _prototypes.Remove(prototype);
        }

        private void OnAttributesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (EntityAttributeViewModel inheritedAttribute in args.NewItems)
                {
                    EntityAttributeViewModel localAttribute = _attributes.FirstOrDefault(x => x.Key == inheritedAttribute.Key);
                    if (null == localAttribute)
                    {
                        _attributes.Add(new EntityAttributeViewModel(entity, inheritedAttribute.Key));
                    }
                    else
                    {
                        localAttribute.FindInheritedViewModel();
                    }
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (EntityAttributeViewModel inheritedAttribute in args.OldItems)
                {
                    EntityAttributeViewModel localAttribute = _attributes.FirstOrDefault(x => x.Key == inheritedAttribute.Key);
                    if (null != localAttribute)
                    {
                        if (localAttribute.IsInherited)
                        {
                            _attributes.Remove(localAttribute);
                        }
                        else
                        {
                            localAttribute.FindInheritedViewModel();
                        }
                    }
                }
            }
        }

        private void OnComponentsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ComponentViewModel inheritedComponent in args.NewItems)
                {
                    ComponentViewModel localComponent = _components.FirstOrDefault(x => x.Descriptor == inheritedComponent.Descriptor);
                    if (null == localComponent)
                    {
                        _components.Add(new ComponentViewModel(entity, inheritedComponent.Descriptor));
                    }
                    else
                    {
                        // TODO: Tell component to resolve properties
                    }
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ComponentViewModel inheritedComponent in args.OldItems)
                {
                    ComponentViewModel localComponent = _components.FirstOrDefault(x => x.Descriptor == inheritedComponent.Descriptor);
                    if (null != localComponent)
                    {
                        if (localComponent.IsInherited)
                        {
                            _components.Remove(localComponent);
                        }
                        else
                        {
                            // TODO: Tell component to resolve properties
                        }
                    }
                }
            }
        }

        private void OnEventsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (EventViewModel evt in args.NewItems)
                {

                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (EventViewModel evt in args.OldItems)
                {

                }
            }
        }

        public void RaiseAttributeAvailable(string oldKey, string key)
        {
            foreach (EntityViewModel prototype in _prototypes)
            {
                EntityAttributeViewModel exposedAttribute = prototype.GetEntityAttributeViewModel(oldKey);

                if (null != exposedAttribute)
                {
                    _attributes.Add(new EntityAttributeViewModel(entity, oldKey));
                }
            }
        
            AttributeAvailable(this, new AttributeAvailableEventArgs(oldKey, key));
        }

        private void OnAttributeAvailable(object sender, AttributeAvailableEventArgs args)
        {
            EntityAttributeViewModel localAttribute = _attributes.FirstOrDefault(x => x.Key == args.NewKey);

            if (null != localAttribute)
            {
                localAttribute.FindInheritedViewModel();
            }
            else
            {
                _attributes.Add(new EntityAttributeViewModel(entity, args.NewKey));
            }

            AttributeAvailable(sender, args);
        }
    }
}
