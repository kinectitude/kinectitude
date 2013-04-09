//-----------------------------------------------------------------------
// <copyright file="ThisChangeable.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
    internal sealed class ThisChangeable : BaseChangeable
    {
        private readonly ThisDataContainer container;
        private readonly string type;
        private Entity entity;
        private Component component;

        private Entity Entity
        {
            get { return entity; }
            set
            {
                if (entity != value)
                {
                    if (null != entity)
                    {
                        entity.Components.CollectionChanged -= OnEntityComponentsChanged;
                        Component = null;

                        foreach (var component in entity.Components)
                        {
                            UnfollowComponent(component);
                        }
                    }

                    entity = value;

                    if (null != entity)
                    {
                        entity.Components.CollectionChanged += OnEntityComponentsChanged;
                        Component = entity.GetComponentByType(type);

                        foreach (var component in entity.Components)
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
            set
            {
                if (component != value)
                {
                    if (null != component)
                    {
                        foreach (var property in component.Properties)
                        {
                            UnfollowProperty(property);
                        }
                    }

                    component = value;

                    if (null != component)
                    {
                        foreach (var property in component.Properties)
                        {
                            FollowProperty(property);
                        }
                    }

                    PublishComponentChange();
                }
            }
        }

        public ThisChangeable(ThisDataContainer container, string type) : base(container)
        {
            this.container = container;
            this.type = type;

            Entity = container.Entity;
            container.EntityChanged += OnEntityChanged;
        }

        private void OnEntityChanged()
        {
            Entity = container.Entity;
        }

        private void UnfollowComponent(Component component)
        {
            foreach (var property in component.Properties)
            {
                property.PropertyChanged -= OnPropertyPropertyChanged;
            }
        }

        private void FollowComponent(Component component)
        {
            foreach (var property in component.Properties)
            {
                property.PropertyChanged += OnPropertyPropertyChanged;
            }
        }

        private void UnfollowProperty(Property property)
        {
            property.PropertyChanged -= OnPropertyPropertyChanged;
        }

        private void FollowProperty(Property property)
        {
            property.PropertyChanged += OnPropertyPropertyChanged;
        }

        private void OnPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                PublishPropertyChange(((Property)sender).Name);
            }
        }

        private void OnEntityComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Component component in e.NewItems)
                {
                    if (component.Type == type)
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
                    if (component.Type == type)
                    {
                        Component = null;
                    }

                    UnfollowComponent(component);
                }
            }
        }

        protected override ValueReader CreatePropertyReader(string name)
        {
            return new ThisComponentValueReader(this, name);
        }
    }
}
