using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedScene : LoadedObject
    {
        private readonly List<string> Managers = new List<string>();

        private readonly List<LoadedEntity> loadedEntities = new List<LoadedEntity>();
        private readonly SceneLoader Loader;
        private readonly Game Game;
        private readonly string Name;

        internal LoadedScene(string name, List<Tuple<string, string>> values, SceneLoader loader, Game game) 
            : base(values)
        {
            Loader = loader;
            Game = game;
            Name = name;
        }

        internal Scene Create()
        {
            Scene scene = new Scene(Loader, Game);
            scene.Name = Name;
            int onid = 0;
            setValues(scene);
            foreach (string type in Managers)
            {
                IManager manager = LoadedManager.CreateManagers(this, type);
                scene.Managers.Add(manager);
                scene.ManagersDictionary[manager.GetType()] =  manager;
            }

            foreach (LoadedEntity loadedEntity in loadedEntities)
            {
                Entity entity = loadedEntity.Create(scene);
                Loader.EntityById[entity.Id] = entity;
                entity.Scene = scene;
                if (null != entity.Name) Loader.EntityByName.Add(entity.Name, entity);
                entity.Ready();
            }

            return scene;
        }

        internal void addLoadedEntity(LoadedEntity entity)
        {
            loadedEntities.Add(entity);
        }

        internal void addManagerName(string type)
        {
            if(!Managers.Contains(type)) Managers.Add(type);
        }

    }
}
