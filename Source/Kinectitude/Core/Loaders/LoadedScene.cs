//-----------------------------------------------------------------------
// <copyright file="LoadedScene.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedScene : LoadedObject
    {
        private readonly List<LoadedManager> Managers = new List<LoadedManager>();

        private readonly List<LoadedEntity> loadedEntities = new List<LoadedEntity>();
        private readonly SceneLoader Loader;
        private readonly Game Game;
        private readonly string Name;

        internal LoadedScene(string name, PropertyHolder values, SceneLoader loader, Game game, LoaderUtility loaderUtil) 
            : base(values, loaderUtil)
        {
            Loader = loader;
            Game = game;
            Name = name;
        }

        internal Scene Create()
        {
            Scene scene = new Scene(Loader, Game);
            scene.Name = Name;

            foreach (Tuple<string, object> value in Values)
            {
                object assignable = LoaderUtil.MakeAssignable(value.Item2, scene);
                scene[value.Item1] = assignable as ValueReader;
            }
            
            foreach (LoadedManager loadedManager in Managers)
            {
                IManager manager = loadedManager.CreateManager(scene);
                scene.Managers.Add(manager);
                scene.ManagersDictionary[manager.GetType()] =  manager;
            }

            foreach (LoadedEntity loadedEntity in loadedEntities)
            {
                Entity entity = loadedEntity.Create(scene);
                entity.Ready();
            }

            return scene;
        }

        internal void addLoadedEntity(LoadedEntity entity)
        {
            loadedEntities.Add(entity);
        }

        internal void addLoadedManager(LoadedManager manager)
        {
            Managers.Add(manager);
        }

    }
}
