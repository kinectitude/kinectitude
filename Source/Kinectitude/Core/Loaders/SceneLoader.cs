using System.Collections.Generic;
using Kinectitude.Core.Base;
using System;
using System.Linq;

namespace Kinectitude.Core.Loaders
{
    internal sealed class SceneLoader
    {
        private readonly LoaderUtility loaderUtility;
        private readonly GameLoader GameLoader;
        private int onid = 0;

        internal LoadedScene LoadedScene { get; private set; }
        internal Game Game { get; private set; }
        
        internal SceneLoader(string sceneName, object scene, LoaderUtility loaderUtility, GameLoader gameLoader)
        {    
            GameLoader = gameLoader;
            Game = GameLoader.Game;
            this.loaderUtility = loaderUtility;

            PropertyHolder sceneValues = loaderUtility.GetProperties(scene);

            LoadedScene = new LoadedScene(sceneName, sceneValues, this, gameLoader.Game, loaderUtility);

            IEnumerable<object> managers = loaderUtility.GetOfType(scene, loaderUtility.ManagerType);

            foreach (object manager in managers)
            {
                PropertyHolder managerValues = loaderUtility.GetProperties(manager);
                string type = loaderUtility.GetType(manager);
                LoadedManager lm = LoadedManager.GetLoadedManager(type, LoadedScene, managerValues, loaderUtility);
                LoadedScene.addLoadedManager(lm);
            }

            IEnumerable<object> entities = loaderUtility.GetOfType(scene, loaderUtility.EntityType);

            foreach (object entity in entities)
            {
                string name = loaderUtility.GetName(entity);
                if (ClassFactory.isRegistered(name)) Game.CurrentGame.Die("The name " + name + " can't be both defined and used for an entity");
                LoadedEntity loadedEntity = gameLoader.entityParse(entity, name, onid++);
                LoadedScene.addLoadedEntity(loadedEntity);
            }

        }

        internal void CreateEntity(string name, Scene scene)
        {
            LoadedEntity loadedEntity;
            if (!LoadedEntity.Prototypes.TryGetValue(name, out loadedEntity)) Game.CurrentGame.Die("The prototype " + name + " does not exist!");
            Entity entity = loadedEntity.Create(onid++, scene, true);
            entity.Scene = scene;
            entity.Ready();
        }
    }
}