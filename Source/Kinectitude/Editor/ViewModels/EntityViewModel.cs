using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Kinectitude.Editor.Models;
using Attribute = Kinectitude.Editor.Models.Attribute;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    public class EntityViewModel : BaseModel
    {
        private const string RenderComponent = "Kinectitude.Render.RenderComponent";
        private const string Shape = "Shape";
        private const string Ellipse = "Ellipse";

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
                Component component = entity.GetComponent("Kinectitude.Render.RenderComponent");
                if (null != component)
                {
                    TextProperty property = component.GetProperty<TextProperty>("Shape");
                    if (null != property && property.Value == "Ellipse")
                    {
                        ret = false;
                    }
                }

                /*if (entity.HasComponent("RenderComponent"))
                {
                    TextProperty property = entity.GetPropertyForComponent<TextProperty>("RenderComponent", "Shape");
                    if (null != property && property.Value == "Ellipse")
                    {
                        ret = false;
                    }
                }*/
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

                /*if (entity.HasComponent("TextRenderComponent"))
                {
                    TextProperty property = entity.GetPropertyForComponent<TextProperty>("TextRenderComponent", "Value");
                    if (null != property)
                    {
                        text = property.Value;
                    }
                }*/
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

                /*if (entity.HasComponent("RenderComponent"))
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
                }*/
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
