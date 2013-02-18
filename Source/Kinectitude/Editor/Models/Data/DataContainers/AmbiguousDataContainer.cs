using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class AmbiguousDataContainer : BaseDataContainer
    {
        private readonly Value value;
        private readonly string entityOrComponentName;
        private Scene scene;
        private Entity namedEntity;
        private Entity thisEntity;
        private Component thisComponent;

        private Scene Scene
        {
            get { return scene; }
            set
            {
                if (scene != value)
                {
                    if (null != scene)
                    {
                        scene.Entities.CollectionChanged -= OnSceneEntitiesChanged;
                        NamedEntity = null;

                        foreach (var entity in scene.Entities)
                        {
                            UnfollowEntity(entity);
                        }
                    }

                    scene = value;

                    if (null != scene)
                    {
                        scene.Entities.CollectionChanged += OnSceneEntitiesChanged;
                        NamedEntity = scene.GetEntityByName(entityOrComponentName);

                        foreach (var entity in scene.Entities)
                        {
                            FollowEntity(entity);
                        }
                    }
                }
            }
        }

        public Entity NamedEntity
        {
            get { return namedEntity; }
            private set
            {
                if (namedEntity != value)
                {
                    if (null != namedEntity)
                    {
                        namedEntity.Attributes.CollectionChanged -= OnNamedEntityAttributesChanged;

                        foreach (var attribute in namedEntity.Attributes)
                        {
                            UnfollowAttribute(attribute);
                        }
                    }

                    namedEntity = value;

                    if (null != namedEntity)
                    {
                        namedEntity.Attributes.CollectionChanged += OnNamedEntityAttributesChanged;

                        foreach (var attribute in namedEntity.Attributes)
                        {
                            FollowAttribute(attribute);
                        }
                    }

                    if (null != NamedEntityChanged)
                    {
                        NamedEntityChanged();
                    }

                    PublishAll();
                }
            }
        }

        private Entity ThisEntity
        {
            get { return thisEntity; }
            set
            {
                if (thisEntity != value)
                {
                    if (null != thisEntity)
                    {
                        thisEntity.Components.CollectionChanged -= OnThisEntityComponentsChanged;
                        ThisComponent = null;

                        foreach (var component in thisEntity.Components)
                        {
                            UnfollowComponent(component);
                        }
                    }

                    thisEntity = value;

                    if (null != thisEntity)
                    {
                        thisEntity.Components.CollectionChanged += OnThisEntityComponentsChanged;
                        ThisComponent = thisEntity.GetComponentByType(entityOrComponentName);

                        foreach (var component in thisEntity.Components)
                        {
                            FollowComponent(component);
                        }
                    }
                }
            }
        }

        public Component ThisComponent
        {
            get { return thisComponent; }
            private set
            {
                if (thisComponent != value)
                {
                    thisComponent = value;
                    PublishAll();
                }
            }
        }

        public event System.Action NamedEntityChanged;

        public AmbiguousDataContainer(Value value, string name)
        {
            this.value = value;
            this.entityOrComponentName = name;

            value.AddHandler<ScopeChanged>(OnScopeChanged);
        }

        private void OnScopeChanged(ScopeChanged obj)
        {
            Scene = value.Scene;
            ThisEntity = value.Entity;
        }

        private void FollowEntity(Entity entity)
        {
            entity.PropertyChanged += OnEntityPropertyChanged;
        }

        private void UnfollowEntity(Entity entity)
        {
            entity.PropertyChanged -= OnEntityPropertyChanged;
        }

        private void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                var entity = (Entity)sender;
                if (entity.Name == entityOrComponentName)
                {
                    NamedEntity = entity;
                }
            }
        }

        private void OnSceneEntitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Entity entity in e.NewItems)
                {
                    if (entity.Name == entityOrComponentName)
                    {
                        NamedEntity = entity;
                    }

                    FollowEntity(entity);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Entity entity in e.OldItems)
                {
                    if (entity.Name == entityOrComponentName)
                    {
                        NamedEntity = null;
                    }

                    UnfollowEntity(entity);
                }
            }
        }

        private void FollowAttribute(Attribute attribute)
        {
            attribute.PropertyChanged += OnAttributePropertyChanged;
            attribute.NameChanged += OnAttributeNameChanged;
        }

        private void UnfollowAttribute(Attribute attribute)
        {
            attribute.PropertyChanged -= OnAttributePropertyChanged;
            attribute.NameChanged -= OnAttributeNameChanged;
        }

        private void OnAttributePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                PublishAttributeChange(((Attribute)sender).Name);
            }
        }

        private void OnAttributeNameChanged(string oldName, string newName)
        {
            PublishAttributeChange(oldName);
            PublishAttributeChange(newName);
        }

        private void OnNamedEntityAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Attribute attribute in e.NewItems)
                {
                    FollowAttribute(attribute);
                    PublishAttributeChange(attribute.Name);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Attribute attribute in e.OldItems)
                {
                    UnfollowAttribute(attribute);
                    PublishAttributeChange(attribute.Name);
                }
            }
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
                if (component == ThisComponent)
                {
                    if (component.Type != entityOrComponentName)
                    {
                        ThisComponent = null;
                    }
                }
                else if (component.Type == entityOrComponentName)
                {
                    ThisComponent = component;
                }
            }
        }

        private void OnPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                PublishAttributeChange(((Property)sender).Name);
            }
        }

        private void OnThisEntityComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Component component in e.NewItems)
                {
                    if (component.Type == entityOrComponentName)
                    {
                        ThisComponent = component;
                    }

                    FollowComponent(component);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Component component in e.OldItems)
                {
                    if (ThisEntity.GetDefinedName(component.Plugin) == entityOrComponentName)
                    {
                        ThisComponent = null;
                    }

                    UnfollowComponent(component);
                }
            }
        }

        protected override ValueReader CreateAttributeReader(string key)
        {
            return new AmbiguousValueReader(this, key);
        }

        protected override IChangeable CreateChangeable(string name)
        {
            return new ComponentChangeable(this, name);
        }
    }
}
