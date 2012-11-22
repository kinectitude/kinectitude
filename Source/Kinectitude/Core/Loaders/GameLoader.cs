using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Loaders
{
    public class GameLoader
    {
        private readonly LoaderUtility loaderUtility;
        private readonly Dictionary<string, LoadedScene> Scenes = new Dictionary<string, LoadedScene>();

        public string FirstScene { get; private set; }

        private Dictionary<string, Assembly> LoadedFiles = new Dictionary<string, Assembly>();

        internal Game Game { get; private set; }

        public Game CreateGame()
        {
            object root = loaderUtility.GetGame();
            PropertyHolder gameProperties = loaderUtility.GetProperties(root);

            foreach (Tuple<string, object> gameValue in gameProperties)
                Game[gameValue.Item1] = loaderUtility.MakeAssignable(gameValue.Item2) as ValueReader;

            FirstScene = loaderUtility.MakeAssignable(gameProperties["FirstScene"]) as ValueReader;
            //TODO for Widht, Height, IsFullScreen

            IEnumerable<object> usings = loaderUtility.GetOfType(root, loaderUtility.UsingType);

            foreach (object uses in usings)
            {
                IEnumerable<Tuple<string, string>> defines = loaderUtility.GetDefines(uses);
                string useFile = loaderUtility.GetFile(uses);
                foreach (Tuple<string, string> defined in defines) loadReflection(useFile, defined.Item1, defined.Item2);
            }

            IEnumerable<object> prototypes = loaderUtility.GetOfType(root, loaderUtility.PrototypeType);

            foreach (object prototype in prototypes)
            {
                PropertyHolder prototypeProperties = loaderUtility.GetProperties(prototype);
                string myName = loaderUtility.GetName(prototype);
                LoadedEntity loadedPrototype = entityParse(prototype, myName, -3);
            }

            IEnumerable<object> services= loaderUtility.GetOfType(root, loaderUtility.ServiceType);
            foreach (object serviceobj in services)
            {
                PropertyHolder values = loaderUtility.GetProperties(serviceobj);
                string name = loaderUtility.GetName(serviceobj);
                foreach (Tuple<string, object> value in values)
                {
                    Service service = Game.GetChangeable(name) as Service;
                    ClassFactory.SetParam(service, value.Item1, loaderUtility.MakeAssignable(value.Item2));
                }
            }

            IEnumerable<object> scenes = loaderUtility.GetOfType(root, loaderUtility.SceneType);

            foreach (object scene in scenes)
            {
                string sceneName = loaderUtility.GetName(scene);
                SceneLoader sceneLoader = new SceneLoader(sceneName, scene, loaderUtility, this);
                Scenes[sceneName] = sceneLoader.LoadedScene;
            }

            return Game;
        }

        public GameLoader
            (string fileName, Assembly [] preloads, float scaleX, float scaleY, Func<Tuple<int, int>> windowOffset, Action<string> die)
        {

            Game = new Game(this, scaleX, scaleY, windowOffset, die);
            string extention = null;
            try
            {
                extention = fileName.Substring(fileName.IndexOf('.'));
            }
            catch (Exception e)
            {
                die("File " + fileName + " could Not be loaded");
                return;
            }

            if (".kgl" == extention)
            {
                loaderUtility = new KGLLoaderUtility(fileName, this);
            }
            else
            {
                die("File " + fileName + " could Not be loaded");
            }

            foreach (Assembly loaded in preloads)
            {
                string name = Path.GetFileName(loaded.Location);
                if (!LoadedFiles.ContainsKey(name))
                {
                    LoadedFiles.Add(name, loaded);
                    ClassFactory.LoadServicesAndManagers(loaded);
                }
            }
        }

        internal Scene GetScene(string name) { return Scenes[name].Create(); }

        private void loadReflection(string file, string named, string fullName)
        {
            Assembly assembly = null;
            if (!LoadedFiles.ContainsKey(file))
            {
                assembly =  Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "plugins", file));
                ClassFactory.LoadServicesAndManagers(assembly);
                LoadedFiles.Add(file, assembly);
            }
            else
            {
                assembly = LoadedFiles[file];
            }
            ClassFactory.RegisterType(named, assembly.GetType(fullName));

        }

        internal LoadedEntity entityParse(object entity, string name, int id)
        {
            List<string> isType = new List<string>();
            List<string> isExactType = new List<string>();
            IEnumerable<object> components = loaderUtility.GetOfType(entity, loaderUtility.ComponentType);
            IEnumerable<object> events = loaderUtility.GetOfType(entity, loaderUtility.EventType);
            PropertyHolder entityProperties = loaderUtility.GetProperties(entity);
            IEnumerable<string> prototypes = loaderUtility.GetPrototypes(entity);

            LoadedEntity loadedEntity = new LoadedEntity(name, entityProperties, id, prototypes, loaderUtility);

            foreach (object component in components)
            {
                PropertyHolder componentProperties = loaderUtility.GetProperties(component);
                string type = loaderUtility.GetType(component);
                LoadedComponent lc = new LoadedComponent(type, componentProperties, loaderUtility);
                loadedEntity.AddLoadedComponent(lc);
            }

            foreach (object evt in events) createEvent(Game, evt, loadedEntity);
            
            loadedEntity.Prepare();
            return loadedEntity;
        }

        private LoadedEvent createEvent(Game game, object from, LoadedEntity entity)
        {
            PropertyHolder keyWords = loaderUtility.GetProperties(from);
            string type = loaderUtility.GetType(from);
            LoadedEvent loadedEvt = new LoadedEvent(type, keyWords, entity, loaderUtility);
            addActions(game, from, loadedEvt);
            return loadedEvt;
        }

        private LoadedCondition createCondition(Game game, LoadedEvent e, object condition)
        {
            IEnumerable<object> elseStatements = loaderUtility.GetOfType(condition, loaderUtility.Else);
            elseStatements = elseStatements.Reverse();
            LoadedCondition next = null;

            foreach (object elseStatement in elseStatements)
            {
                next = new LoadedCondition(loaderUtility.GetCondition(elseStatement), next, loaderUtility);
                addActions(game, elseStatement, e, next);
            }

            LoadedCondition lc = new LoadedCondition(loaderUtility.GetCondition(condition), next, loaderUtility);
            addActions(game, condition, e, lc);
            return lc;
        }

        //Adds actions to an event or trigger
        private void addActions(Game game, object evt, LoadedEvent loadedEvent, LoadedCondition cond = null)
        {
            IEnumerable<object> actions = loaderUtility.GetOfType(evt, loaderUtility.ActionType);

            foreach (object action in actions)
            {
                LoadedBaseAction actionToAdd;
                if (loaderUtility.IsAciton(action))
                {
                    PropertyHolder actionProperties = loaderUtility.GetProperties(action);
                    string type = loaderUtility.GetType(action);
                    actionToAdd = new LoadedAction(type, actionProperties, loaderUtility);
                }
                else if (loaderUtility.IsAssignment(action))
                {
                    Tuple<object, object, object> assignment = loaderUtility.GetAssignment(action);
                    actionToAdd = new LoadedAssignment(assignment.Item1, assignment.Item2, assignment.Item3, loaderUtility);
                }
                else
                {
                    actionToAdd = createCondition(game, loadedEvent, action);
                }
                if (null != cond) cond.AddAction(actionToAdd);
                else loadedEvent.AddAction(actionToAdd);
            }
        }

    }
}
