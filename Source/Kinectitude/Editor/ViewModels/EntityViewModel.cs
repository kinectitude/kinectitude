using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Attribute = Kinectitude.Editor.Models.Attribute;
using Component = Kinectitude.Editor.Models.Component;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class EntityViewModel : BaseModel
    {
        private readonly Entity entity;
        private EntityVisualViewModel visual;
        private bool selected;

        public Entity Model
        {
            get { return entity; }
        }

        public string Name
        {
            get { return entity.Name; }
            set { entity.Name = value; }
        }

        public ComputedObservableCollection<Entity, EntityViewModel> Prototypes
        {
            get;
            private set;
        }

        public ObservableCollection<Attribute> Attributes
        {
            get { return entity.Attributes; }
        }

        public ObservableCollection<Component> Components
        {
            get { return entity.Components; }
        }

        public ObservableCollection<AbstractEvent> Events
        {
            get { return entity.Events; }
        }

        public EntityVisualViewModel Visual
        {
            get { return visual; }
            set
            {
                if (visual != value)
                {
                    visual = value;
                    NotifyPropertyChanged("Visual");
                }
            }
        }

        public bool IsSelected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
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

        public EntityViewModel(Entity entity)
        {
            this.entity = entity;

            entity.PropertyChanged += Entity_PropertyChanged;
            entity.Components.CollectionChanged += Components_CollectionChanged;

            visual = EntityVisualViewModel.Create(entity);

            AddPrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = parameter as Entity;
                    if (null != prototype)
                    {
                        entity.AddPrototype(prototype);
                    }
                }
            );

            RemovePrototypeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Entity prototype = parameter as Entity;
                    if (null != prototype)
                    {
                        entity.RemovePrototype(prototype);
                    }
                }
            );

            AddAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    entity.CreateAttribute();
                }
            );

            RemoveAttributeCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Attribute attribute = parameter as Attribute;
                    entity.RemoveAttribute(attribute);
                }
            );

            AddComponentCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Plugin plugin = parameter as Plugin;
                    if (null != plugin)
                    {
                        Component component = new Component(plugin);
                        entity.AddComponent(component);
                    }
                }
            );

            RemoveComponentCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Component component = parameter as Component;
                    if (null != component)
                    {
                        entity.RemoveComponent(component);
                    }
                }
            );

            AddEventCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Plugin plugin = parameter as Plugin;
                    if (null != plugin)
                    {
                        Event evt = new Event(plugin);
                        entity.AddEvent(evt);
                    }
                }
            );

            RemoveEventCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Event evt = parameter as Event;
                    if (null != evt)
                    {
                        entity.RemoveEvent(evt);
                    }
                }
            );
        }

        private void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Visual = EntityVisualViewModel.Create(entity);
        }
    }
}
