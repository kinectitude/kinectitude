//-----------------------------------------------------------------------
// <copyright file="ManagerChangeable.cs" company="Kinectitude">
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
    internal sealed class ManagerChangeable : BaseChangeable
    {
        private readonly SceneDataContainer container;
        private readonly string type;
        private Scene scene;
        private Manager manager;

        private Scene Scene
        {
            get { return scene; }
            set
            {
                if (scene != value)
                {
                    if (null != scene)
                    {
                        scene.Managers.CollectionChanged -= OnSceneManagersChanged;
                        Manager = null;

                        foreach (var manager in scene.Managers)
                        {
                            UnfollowManager(manager);
                        }
                    }

                    scene = value;

                    if (null != scene)
                    {
                        scene.Managers.CollectionChanged += OnSceneManagersChanged;
                        Manager = scene.GetManagerByDefinedName(type);

                        foreach (var manager in scene.Managers)
                        {
                            FollowManager(manager);
                        }
                    }
                }
            }
        }

        public Manager Manager
        {
            get { return manager; }
            private set
            {
                if (manager != value)
                {
                    manager = value;
                    PublishComponentChange();
                }
            }
        }

        public ManagerChangeable(SceneDataContainer container, string type) : base(container)
        {
            this.container = container;
            this.type = type;

            Scene = container.Scene;
            container.SceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged()
        {
            Scene = container.Scene;
        }


        private void UnfollowManager(Manager manager)
        {
            manager.PropertyChanged -= OnManagerPropertyChanged;

            foreach (var property in manager.Properties)
            {
                property.PropertyChanged -= OnPropertyPropertyChanged;
            }
        }

        private void FollowManager(Manager manager)
        {
            manager.PropertyChanged += OnManagerPropertyChanged;

            foreach (var property in manager.Properties)
            {
                property.PropertyChanged += OnPropertyPropertyChanged;
            }
        }

        private void OnManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                var manager = (Manager)sender;
                if (manager == Manager)
                {
                    if (manager.Type != type)
                    {
                        Manager = null;
                    }
                }
                else if (manager.Type == type)
                {
                    Manager = manager;
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

        private void OnSceneManagersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Manager manager in e.NewItems)
                {
                    if (Scene.GetDefinedName(manager.Plugin) == type)
                    {
                        Manager = manager;
                    }

                    FollowManager(manager);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Manager manager in e.OldItems)
                {
                    if (Scene.GetDefinedName(manager.Plugin) == type)
                    {
                        Manager = null;
                    }

                    UnfollowManager(manager);
                }
            }
        }

        protected override ValueReader CreatePropertyReader(string name)
        {
            return new ManagerValueReader(this, name);
        }
    }
}
