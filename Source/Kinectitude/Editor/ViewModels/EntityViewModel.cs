using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Commands.Entity;
using Kinectitude.Editor.Commands.Base;

namespace Kinectitude.Editor.ViewModels
{
    public class EntityViewModel : BaseModel
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

        private readonly Entity entity;
        private readonly ObservableCollection<EntityViewModel> _prototypes;
        private readonly ObservableCollection<AttributeViewModel> _attributes;
        private readonly ObservableCollection<ComponentViewModel> _components;
        private readonly ObservableCollection<EventViewModel> _events;
        private readonly ModelCollection<EntityViewModel> prototypes;
        private readonly ModelCollection<AttributeViewModel> attributes;
        private readonly ModelCollection<ComponentViewModel> components;
        private readonly ModelCollection<EventViewModel> events;

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
                    RenameEntityCommand command = new RenameEntityCommand(this, value);
                    command.Execute();
                }
            }
        }

        public ModelCollection<AttributeViewModel> Attributes
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
            var attributeViewModels = from attribute in entity.Attributes select AttributeViewModel.GetViewModel(entity, attribute.Key);
            var componentViewModels = from component in entity.Components select new ComponentViewModel(component);
            var eventViewModels = from evt in entity.Events select new EventViewModel(evt);

            _prototypes = new ObservableCollection<EntityViewModel>();
            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            _components = new ObservableCollection<ComponentViewModel>(componentViewModels);
            _events = new ObservableCollection<EventViewModel>(eventViewModels);

            foreach (EntityViewModel prototype in prototypeViewModels)
            {
                PrivateAddPrototype(prototype);
            }

            prototypes = new ModelCollection<EntityViewModel>(_prototypes);
            attributes = new ModelCollection<AttributeViewModel>(_attributes);
            components = new ModelCollection<ComponentViewModel>(_components);
            events = new ModelCollection<EventViewModel>(_events);
        }

        private void OnAttributesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            /*if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AttributeViewModel attribute in args.NewItems)
                {
                    AttributeViewModel localAttribute = AttributeViewModel.GetViewModel(attribute);
                    _attributes.Add(localAttribute);

                    // TODO Check all cases for key existing or not existing
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                // TODO Check all cases for key existing or not existing
            }*/
        }

        private void OnComponentsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ComponentViewModel component in args.NewItems)
                {

                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ComponentViewModel component in args.OldItems)
                {

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

        public void ExecuteAddAttributeCommand(object parameter)
        {
            
        }

        public void ExecuteRemoveAttributeCommand(object parameter)
        {
            AttributeViewModel viewModel = parameter as AttributeViewModel;
            if (null != viewModel)
            {
                RemoveAttribute(viewModel);
            }
        }

        public void ExecuteAddComponentCommand(object parameter)
        {

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

        public void AddAttribute(AttributeViewModel attribute)
        {
            entity.AddAttribute(attribute.Attribute);
            _attributes.Add(attribute);
        }

        public void RemoveAttribute(AttributeViewModel attribute)
        {
            //en
        }

        public void AddPrototype(EntityViewModel entityViewModel)
        {
            entity.AddPrototype(entityViewModel.Entity);
            PrivateAddPrototype(entityViewModel);
        }

        public void RemovePrototype(EntityViewModel entityViewModel)
        {
            entity.RemovePrototype(entityViewModel.Entity);
            PrivateRemovePrototype(entityViewModel);
        }

        private void PrivateAddPrototype(EntityViewModel entityViewModel)
        {
            entityViewModel.Attributes.CollectionChanged += OnAttributesChanged;
            entityViewModel.Components.CollectionChanged += OnComponentsChanged;
            entityViewModel.Events.CollectionChanged += OnEventsChanged;

            foreach (AttributeViewModel inheritedAttribute in entityViewModel.Attributes)
            {
                // TODO handle case for local non-inheriting attribute which has same key as one of the new ones

                AttributeViewModel localViewModel = _attributes.FirstOrDefault(x => x.Key == inheritedAttribute.Key);
                if (null == localViewModel)
                {
                    _attributes.Add(AttributeViewModel.GetViewModel(entity, inheritedAttribute.Key));
                }
            }

            // TODO components and events

            foreach (ComponentViewModel inheritedComponent in entityViewModel.Components)
            {

            }

            foreach (EventViewModel inheritedEvent in entityViewModel.Events)
            {

            }

            _prototypes.Add(entityViewModel);
        }

        private void PrivateRemovePrototype(EntityViewModel entityViewModel)
        {
            entityViewModel.Attributes.CollectionChanged -= OnAttributesChanged;
            entityViewModel.Components.CollectionChanged -= OnComponentsChanged;
            entityViewModel.Events.CollectionChanged -= OnEventsChanged;



            _prototypes.Remove(entityViewModel);
        }
    }
}
