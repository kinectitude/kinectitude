using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal abstract class SceneLoader
    {
        internal Dictionary<string, HashSet<int>> IsType { get; private set; }
        internal Dictionary<string, HashSet<int>> IsExactType { get; private set; }
        internal Dictionary<int, Entity> EntityById { get; private set; }
        internal Dictionary<string, Entity> EntityByName { get; private set; }
        internal protected LoadedScene LoadedScene { get; protected set; }
        internal Game Game { get; private set; }
        protected GameLoader GameLoader { get; private set; }

        private static Dictionary<string, LoadedEntity> loadedPrototypes = new Dictionary<string, LoadedEntity>();

        protected int Onid = 0;

        protected SceneLoader(GameLoader gameLoader)
        {
            IsType = new Dictionary<string, HashSet<int>>();
            IsExactType = new Dictionary<string, HashSet<int>>();
            EntityById = new Dictionary<int, Entity>();
            EntityByName = new Dictionary<string, Entity>();
            GameLoader = gameLoader;
            Game = GameLoader.Game;
        }

        protected abstract LoadedEntity PrototypeMaker(string name);

        internal void CreateEntity(string name, Scene scene)
        {
            LoadedEntity loadedEntity;
            if (!loadedPrototypes.TryGetValue(name, out loadedEntity))
            {
                loadedEntity = PrototypeMaker(name);
                loadedPrototypes.Add(name, loadedEntity);
            }
            addToHashSet(Onid, name, IsType);
            addToHashSet(Onid, name, IsExactType);
            addToAllTypes(name, Onid);
            Entity entity = loadedEntity.Create(Onid, scene);
            EntityById[Onid++] = entity;
            entity.Scene = scene;
            entity.Ready();
        }

        protected void addToAllTypes(string name, int id)
        {
            HashSet<int> addTo;

            foreach (string prototypeIs in GameLoader.PrototypeIs[name])
            {
                if (!IsType.ContainsKey(prototypeIs))
                {
                    addTo = new HashSet<int>();
                    IsType.Add(prototypeIs, addTo);
                }
                else
                {
                    addTo = IsType[prototypeIs];
                }
                addTo.Add(id);
            }
        }

        private static void addToHashSet(int value, string name, Dictionary<string, HashSet<int>> dictionary)
        {
            HashSet<int> addTo;
            if (!dictionary.ContainsKey(name))
            {
                addTo = new HashSet<int>();
                dictionary[name] = addTo;
            }
            else
            {
                addTo = dictionary[name];
            }
            addTo.Add(value);
        }

    }
}