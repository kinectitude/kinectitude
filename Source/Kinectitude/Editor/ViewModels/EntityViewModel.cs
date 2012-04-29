using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Editor.ViewModels
{
    public class EntityViewModel : BaseModel
    {
        private static readonly Dictionary<Entity, EntityViewModel> viewModels;

        static EntityViewModel()
        {
            viewModels = new Dictionary<Entity, EntityViewModel>();
        }

        public static EntityViewModel Create(Entity entity)
        {
            if (!viewModels.ContainsKey(entity))
            {
                EntityViewModel viewModel = new EntityViewModel(entity);
                viewModels[entity] = viewModel;
            }
            return viewModels[entity];
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
                    entity.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public int X
        {
            get
            {
                IntegerAttribute x = entity.GetAttribute<IntegerAttribute>("x");
                return null != x ? x.Value : 0;
            }
            set
            {
                IntegerAttribute x = entity.GetAttribute<IntegerAttribute>("x");
                x.Value = value;
                RaisePropertyChanged("X");
            }
        }

        public int Y
        {
            get
            {
                IntegerAttribute y = entity.GetAttribute<IntegerAttribute>("y");
                return null != y ? y.Value : 0;
            }
            set
            {
                IntegerAttribute y = entity.GetAttribute<IntegerAttribute>("y");
                y.Value = value;
                RaisePropertyChanged("Y");
            }
        }

        public int Width
        {
            get
            {
                IntegerAttribute width = entity.GetAttribute<IntegerAttribute>("width");
                return null != width ? width.Value : 24;
            }
            set
            {
                IntegerAttribute width = entity.GetAttribute<IntegerAttribute>("width");
                width.Value = value;
                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get
            {
                IntegerAttribute height = entity.GetAttribute<IntegerAttribute>("height");
                return null != height ? height.Value : 24;
            }
            set
            {
                IntegerAttribute height = entity.GetAttribute<IntegerAttribute>("height");
                height.Value = value;
                RaisePropertyChanged("Height");
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
                if (entity.HasComponent("RenderComponent"))
                {
                    TextProperty property = entity.GetPropertyForComponent<TextProperty>("RenderComponent", "Shape");
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
                if (entity.HasComponent("TextRenderComponent"))
                {
                    TextProperty property = entity.GetPropertyForComponent<TextProperty>("TextRenderComponent", "Value");
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
                if (entity.HasComponent("RenderComponent"))
                {
                    TextProperty property = entity.GetPropertyForComponent<TextProperty>("RenderComponent", "Fillcolor");
                    if (null != property)
                    {
                        color = property.Value;
                    }
                }
                if (color == string.Empty && entity.HasComponent("TextRenderComponent"))
                {
                    TextProperty property = entity.GetPropertyForComponent<TextProperty>("TextRenderComponent", "Fontcolor");
                    if (null != property)
                    {
                        color = property.Value;
                    }
                }
                return color;
            }
        }

        public ModelCollection<EntityViewModel> Prototypes
        {
            get { return prototypes; }
        }

        /*public ModelCollection<EntityViewModel> UnusedPrototypes
        {
            get { return 
        }*/

        private EntityViewModel(Entity entity)
        {
            this.entity = entity;

            var prototypeViewModels = from prototype in entity.Prototypes select new EntityViewModel(prototype);
            var attributeViewModels = from attribute in entity.Attributes select new AttributeViewModel(attribute);
            var componentViewModels = from component in entity.Components select new ComponentViewModel(component);
            var eventViewModels = from evt in entity.Events select new EventViewModel(evt);

            _prototypes = new ObservableCollection<EntityViewModel>(prototypeViewModels);
            _attributes = new ObservableCollection<AttributeViewModel>(attributeViewModels);
            _components = new ObservableCollection<ComponentViewModel>(componentViewModels);
            _events = new ObservableCollection<EventViewModel>(eventViewModels);

            prototypes = new ModelCollection<EntityViewModel>(_prototypes);
            attributes = new ModelCollection<AttributeViewModel>(_attributes);
            components = new ModelCollection<ComponentViewModel>(_components);
            events = new ModelCollection<EventViewModel>(_events);
        }
    }
}
