//-----------------------------------------------------------------------
// <copyright file="SceneDataContainer.cs" company="Kinectitude">
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
    internal sealed class SceneDataContainer : BaseDataContainer, IScene
    {
        private readonly Value value;
        private readonly Dictionary<string, AmbiguousDataContainer> ambiguousContainers;
        private readonly GameDataContainer gameContainer;
        private Scene scene;

        public Scene Scene
        {
            get { return scene; }
            private set
            {
                if (scene != value)
                {
                    if (null != scene)
                    {
                        scene.Attributes.CollectionChanged -= OnSceneAttributesChanged;

                        foreach (var attribute in scene.Attributes)
                        {
                            UnfollowAttribute(attribute);
                        }
                    }

                    scene = value;

                    if (null != scene)
                    {
                        scene.Attributes.CollectionChanged += OnSceneAttributesChanged;

                        foreach (var attribute in scene.Attributes)
                        {
                            UnfollowAttribute(attribute);
                        }
                    }

                    if (null != SceneChanged)
                    {
                        SceneChanged();
                    }

                    PublishAll();
                }
            }
        }

        public event System.Action SceneChanged;

        public SceneDataContainer(Value value)
        {
            this.value = value;

            Scene = value.Scene;
            value.AddHandler<ScopeChanged>(OnScopeChanged);

            ambiguousContainers = new Dictionary<string, AmbiguousDataContainer>();
            gameContainer = new GameDataContainer(value);
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

        private void OnScopeChanged(ScopeChanged e)
        {
            Scene = value.Scene;
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

        private void OnSceneAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            return new SceneValueReader(this, key);
        }

        protected override IChangeable CreateChangeable(string name)
        {
            return new ManagerChangeable(this, name);
        }

        #region IScene implementation

        IDataContainer IScene.Game
        {
            get { return gameContainer; }
        }

        IDataContainer IScene.GetEntity(string name)
        {
            AmbiguousDataContainer container;
            ambiguousContainers.TryGetValue(name, out container);

            if (null == container)
            {
                container = new AmbiguousDataContainer(value, name);
                ambiguousContainers[name] = container;
            }

            return container;
        }

        HashSet<int> IScene.GetOfPrototype(string prototype, bool exact)
        {
            return new HashSet<int>();
        }

        #endregion
    }
}
