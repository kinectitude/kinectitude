//-----------------------------------------------------------------------
// <copyright file="ThisDataContainer.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    /// <summary>
    /// This is a wrapper class to serve as the entity IDataContainer for a ValueReader in the editor.
    /// A ThisDataContainer is associated with a Value as soon as it is created. If that Value is in
    /// an entity's scope, the ThisDataContainer will be able to get data from that entity. If the Value
    /// is not in any entity's scope, attempts to access data will return ConstantReader.NullValue.
    /// 
    /// If the scope of the Value changes, this class will publish the appropriate notifications.
    /// </summary>
    internal sealed class ThisDataContainer : BaseDataContainer
    {
        private readonly Value value;
        private Entity entity;

        public Entity Entity
        {
            get { return entity; }
            private set
            {
                if (entity != value)
                {
                    if (null != entity)
                    {
                        entity.Attributes.CollectionChanged -= OnEntityAttributesChanged;

                        foreach (var attribute in entity.Attributes)
                        {
                            UnfollowAttribute(attribute);
                        }
                    }

                    entity = value;

                    if (null != entity)
                    {
                        entity.Attributes.CollectionChanged += OnEntityAttributesChanged;

                        foreach (var attribute in entity.Attributes)
                        {
                            FollowAttribute(attribute);
                        }
                    }

                    if (null != EntityChanged)
                    {
                        EntityChanged();
                    }

                    PublishAll();
                }
            }
        }

        public event System.Action EntityChanged;

        public ThisDataContainer(Value value)
        {
            this.value = value;

            Entity = value.Entity;
            value.AddHandler<ScopeChanged>(OnScopeChanged);
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

        private void OnScopeChanged(ScopeChanged obj)
        {
            Entity = value.Entity;
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

        private void OnEntityAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        protected override ValueReader CreateAttributeReader(string key)
        {
            return new ThisEntityValueReader(this, key);
        }

        protected override IChangeable CreateChangeable(string name)
        {
            return new ThisChangeable(this, name);
        }
    }
}
