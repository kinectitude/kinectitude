using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedScene : LoadedObject
    {
        private readonly List<LoadedManager> Managers = new List<LoadedManager>();

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

            foreach (Tuple<string, string> value in Values)
                scene[value.Item1] = ExpressionReader.CreateExpressionReader(value.Item2, null, null).GetValue();
            
            foreach (LoadedManager loadedManager in Managers)
            {
                IManager manager = loadedManager.CreateManager();
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
