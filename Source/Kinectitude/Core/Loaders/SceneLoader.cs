using System.Collections.Generic;
using Kinectitude.Core.Base;
using System;
using System.Linq;

namespace Kinectitude.Core.Loaders
{
    internal class SceneLoader
    {
        private readonly LoaderUtility loaderUtility;
        protected int Onid = 0;

        internal protected LoadedScene LoadedScene { get; protected set; }
        internal Game Game { get; private set; }
        protected GameLoader GameLoader { get; private set; }

#if TEST
        public Entity EntityCreated = null;
#endif

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
                LoadedEntity loadedEntity = gameLoader.entityParse(entity, name, Onid++);
                LoadedScene.addLoadedEntity(loadedEntity);
            }

        }

        internal void CreateEntity(string name, Scene scene)
        {
            LoadedEntity loadedEntity;
            if (!LoadedEntity.Prototypes.TryGetValue(name, out loadedEntity))
            {
                //TODO throw exception?
            }
            Entity entity = loadedEntity.Create(Onid++, scene);
            entity.Scene = scene;
            entity.Ready();
#if TEST
            EntityCreated = entity;
#endif
        }
    }
}