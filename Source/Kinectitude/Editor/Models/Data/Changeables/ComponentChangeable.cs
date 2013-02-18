using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal sealed class ComponentChangeable : BaseChangeable
    {
        private readonly AmbiguousDataContainer container;
        private readonly string type;
        private Entity namedEntity;
        private Component component;

        private Entity NamedEntity
        {
            get { return namedEntity; }
            set
            {
                if (namedEntity != value)
                {
                    if (null != namedEntity)
                    {
                        namedEntity.Components.CollectionChanged -= OnNamedEntityComponentsChanged;
                        Component = null;

                        foreach (var component in namedEntity.Components)
                        {
                            UnfollowComponent(component);
                        }
                    }

                    namedEntity = value;

                    if (null != namedEntity)
                    {
                        namedEntity.Components.CollectionChanged += OnNamedEntityComponentsChanged;
                        Component = namedEntity.GetComponentByType(type);

                        foreach (var component in namedEntity.Components)
                        {
                            FollowComponent(component);
                        }
                    }
                }
            }
        }

        public Component Component
        {
            get { return component; }
            private set
            {
                if (component != value)
                {
                    component = value;
                    PublishComponentChange();
                }
            }
        }

        public ComponentChangeable(AmbiguousDataContainer container, string type) : base(container)
        {
            this.container = container;
            this.type = type;

            NamedEntity = container.NamedEntity;
            container.NamedEntityChanged += OnNamedEntityChanged;
        }

        private void OnNamedEntityChanged()
        {
            NamedEntity = container.NamedEntity;
        }

        private void UnfollowComponent(Component component)
        {
            component.PropertyChanged -= OnComponentPropertyChanged;

            foreach (var property in component.Properties)
            {
                property.PropertyChanged -= OnPropertyPropertyChanged;
            }
        }

        private void FollowComponent(Component component)
        {
            component.PropertyChanged += OnComponentPropertyChanged;

            foreach (var property in component.Properties)
            {
                property.PropertyChanged += OnPropertyPropertyChanged;
            }
        }

        private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                var component = (Component)sender;
                if (component == Component)
                {
                    if (component.Type != type)
                    {
                        Component = null;
                    }
                }
                else if (component.Type == type)
                {
                    Component = component;
                }
            }
        }

        private void OnPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                PublishPropertyChange(((Property)sender).Name);
            }
        }

        private void OnNamedEntityComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Component component in e.NewItems)
                {
                    if (NamedEntity.GetDefinedName(component.Plugin) == type)
                    {
                        Component = component;
                    }

                    FollowComponent(component);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Component component in e.OldItems)
                {
                    if (NamedEntity.GetDefinedName(component.Plugin) == type)
                    {
                        Component = null;
                    }

                    UnfollowComponent(component);
                }
            }
        }

        protected override ValueReader CreatePropertyReader(string name)
        {
            return new ComponentValueReader(this, name);
        }
    }
}
