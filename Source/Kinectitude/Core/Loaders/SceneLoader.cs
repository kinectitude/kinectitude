using System.Collections.Generic;
using Kinectitude.Core.Base;
using System;
using System.Linq;

namespace Kinectitude.Core.Loaders
{
    internal class SceneLoader
    {
        private readonly Dictionary<string, LoadedEntity> loadedPrototypes = new Dictionary<string, LoadedEntity>();
        private readonly LoaderUtility loaderUtility;
        private readonly LanguageKeywords lang;

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
            lang = loaderUtility.Lang;

            HashSet<string> nameSet = new HashSet<string>() { lang.Name };

            List<Tuple<string, string>> sceneValues = loaderUtility.GetValues(scene, nameSet);

            LoadedScene = new LoadedScene(sceneName, sceneValues, this, gameLoader.Game);

            IEnumerable<object> managers = loaderUtility.GetOfType(scene, loaderUtility.ManagerType);

            foreach (object manager in managers)
            {
                List<Tuple<string, string>> managerValues = loaderUtility.GetValues(manager, nameSet);
                Dictionary<string, string> managerProperties = loaderUtility.GetProperties(manager, nameSet);
                LoadedManager.GetLoadedManager(managerProperties[lang.Name], LoadedScene, managerValues);
            }

            IEnumerable<object> entities = loaderUtility.GetOfType(scene, loaderUtility.EntityType);

            HashSet<string> entityWords = new HashSet<string>() { lang.Name, lang.Prototype };

            foreach (object entity in entities)
            {
                Dictionary<string, string> entityProperties = loaderUtility.GetProperties(entity, entityWords);

                string prototypes;

                object obj = entity;

                List<string> isType = new List<string>();
                List<string> isExactType = new List<string>();

                if (entityProperties.TryGetValue(lang.Prototype, out prototypes))
                {
                    prototypes = prototypes.Trim();
                    if (prototypes.Contains(' '))
                    {
                        string[] names = prototypes.Split(' ');
                        foreach (string n in names)
                        {
                            isExactType.Add(n);
                            isType.AddRange(gameLoader.PrototypeIs[n]);
                            loaderUtility.CreateWithPrototype(gameLoader, n, ref obj, Onid);
                        }
                    }
                    else
                    {
                        isExactType.Add(prototypes);
                        isType.AddRange(gameLoader.PrototypeIs[prototypes]);
                        loaderUtility.CreateWithPrototype(gameLoader, prototypes, ref obj, Onid);
                    }
                }

                string entityName = null;
                entityProperties.TryGetValue(lang.Name, out entityName);

                List<Tuple<string, string>> entityValues = loaderUtility.GetValues(entity, entityWords);

                LoadedEntity loadedEntity = new LoadedEntity(entityName, entityValues, Onid++, isType, isExactType);
                LoadedScene.addLoadedEntity(loadedEntity);

                entityParse(entity, loadedEntity);
            }

        }

        protected LoadedEntity PrototypeMaker(string name)
        {
            object prototype = GameLoader.Prototypes[name];
            HashSet<string> words = new HashSet<string>() { lang.Prototype, lang.Name };
            List<Tuple<string, string>> values = loaderUtility.GetValues(prototype, words);
            Dictionary<string, string> properties = loaderUtility.GetProperties(prototype, words);
            List<string> isType = new List<string>();
            List<string> isExactType = new List<string>();

            string prototypes;

            if (properties.TryGetValue(lang.Prototype, out prototypes))
            {
                if (prototypes.Contains(' '))
                {
                    string[] names = name.Split(' ');
                    foreach (string n in names)
                    {
                        isExactType.Add(n);
                        isType.AddRange(GameLoader.PrototypeIs[n]);
                    }
                }
                else
                {
                    isExactType.Add(prototypes);
                    isType.AddRange(GameLoader.PrototypeIs[prototypes]);
                }
            }

            LoadedEntity entity = new LoadedEntity(null, values, Onid, isType, isExactType);
            entityParse(prototype, entity);
            return entity;
        }

        internal void CreateEntity(string name, Scene scene)
        {
            LoadedEntity loadedEntity;
            if (!loadedPrototypes.TryGetValue(name, out loadedEntity))
            {
                loadedEntity = PrototypeMaker(name);
                loadedPrototypes.Add(name, loadedEntity);
            }
            addToHashSet(Onid, name, scene.IsType);
            addToHashSet(Onid, name, scene.IsExactType);
            Entity entity = loadedEntity.Create(Onid, scene);
            entity.Scene = scene;
            entity.Ready();
#if TEST
            EntityCreated = entity;
#endif
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

        private void entityParse(object e, LoadedEntity entity)
        {
            IEnumerable<object> components = loaderUtility.GetOfType(e, loaderUtility.ComponentType);
            IEnumerable<object> events = loaderUtility.GetOfType(e, loaderUtility.EventType);

            HashSet<string> componentWords = new HashSet<string>() { lang.Type };

            foreach (object component in components)
            {
                Dictionary<string, string> componentProperties = loaderUtility.GetProperties(component, componentWords);
                List<Tuple<string, string>> componentValues = loaderUtility.GetValues(component, componentWords);
                LoadedComponent lc = new LoadedComponent(componentProperties[lang.Type], componentValues);
                entity.AddLoadedComponent(lc);
            }

            foreach (object evt in events) createEvent(Game, evt, entity);
        }

        private LoadedEvent createEvent(Game game, object from, LoadedEntity entity)
        {
            HashSet<string> eventWords = new HashSet<string>() { lang.Type };
            Dictionary<string, string> keyWords = loaderUtility.GetProperties(from, eventWords);
            List<Tuple<string, string>> values = loaderUtility.GetValues(from, eventWords);

            LoadedEvent loadedEvt = new LoadedEvent(keyWords[lang.Type], values, entity);
            addActions(game, from, loadedEvt);
            return loadedEvt;
        }

        private LoadedCondition createCondition(Game game, LoadedEvent e, object condition)
        {
            Dictionary<string, string> conditionProperties = loaderUtility.GetProperties(condition, new HashSet<string>() { lang.If });
            LoadedCondition lc = new LoadedCondition((conditionProperties[lang.If]));
            addActions(game, condition, e, lc);
            return lc;
        }

        //Adds actions to an event or trigger
        private void addActions(Game game, object evt, LoadedEvent loadedEvent, LoadedCondition cond = null)
        {
            IEnumerable<object> actions = loaderUtility.GetAll(evt);
            HashSet<string> actionWords = new HashSet<string>() { lang.Type };
            
            foreach (object action in actions)
            {
                List<Tuple<string, string>> actionValues = loaderUtility.GetValues(action, actionWords);
                Dictionary<string, string> actionProperties = loaderUtility.GetProperties(action, actionWords);

                if (loaderUtility.IsAciton(action))
                {
                    LoadedAction loadedAction = new LoadedAction(actionProperties[lang.Type], actionValues);
                    if (null != cond) cond.AddAction(loadedAction);
                    else loadedEvent.AddAction(loadedAction);
                }
                else if (null == cond)
                {
                    loadedEvent.AddAction(createCondition(game, loadedEvent, action));
                }
                else
                {
                    cond.AddAction(createCondition(game, loadedEvent, action));
                }
            }
        }
    }
}