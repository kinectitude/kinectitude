using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    public class GameLoader
    {
        private readonly LoaderUtility loaderUtility;
        private readonly LanguageKeywords lang;
        private readonly Dictionary<string, LoadedScene> Scenes = new Dictionary<string, LoadedScene>();

        public string FirstScene { get; private set; }

        private static Dictionary<string, Assembly> LoadedFiles = new Dictionary<string, Assembly>();

        internal readonly HashSet<string> AvaliblePrototypes = new HashSet<string>();
        internal readonly Dictionary<string, List<string>> PrototypeIs = new Dictionary<string, List<string>>();

        internal Dictionary<string, object> Prototypes { get; private set; }
        internal Game Game { get; private set; }

        public Game CreateGame()
        {
            object root = loaderUtility.Load();
            HashSet<string> gameWords = new HashSet<string>() { lang.FirstScene, lang.Name };
            Dictionary<string, string> gameProperties = loaderUtility.GetProperties(root, gameWords);
            List<Tuple<string, string>> gameValues = loaderUtility.GetValues(root, gameWords);

            foreach (Tuple<string, string> gameValue in gameValues) Game[gameValue.Item1] = gameValue.Item2;

            Game.Name = gameProperties[lang.Name];
            FirstScene = gameProperties[lang.FirstScene];
            //TODO for Widht, Height, IsFullScreen

            IEnumerable<object> usings = loaderUtility.GetOfType(root, loaderUtility.UsingType);
            HashSet<string> usingWords = new HashSet<string>() { lang.File};
            HashSet<string> defineWords = new HashSet<string>() { lang.Name, lang.Class };

            foreach (object uses in usings)
            {
                string useFile = loaderUtility.GetProperties(uses, usingWords)[lang.File];
                IEnumerable<object> defines = loaderUtility.GetOfType(uses, loaderUtility.DefineType);

                foreach (object defined in defines)
                {
                    Dictionary<string, string> defineProperties = loaderUtility.GetProperties(defined, defineWords);
                    string defName = defineProperties[lang.Name];
                    string className = defineProperties[lang.Class];
                    loadReflection(useFile, defName , className);
                }
            }

            IEnumerable<object> prototypes = loaderUtility.GetOfType(root, loaderUtility.PrototypeType);
            HashSet<string> typeWords = new HashSet<string>() { lang.Name, lang.Prototype };

            foreach (object prototype in prototypes)
            {
                Dictionary<string, string> prototypeProperties = loaderUtility.GetProperties(prototype, typeWords);
                string myName = prototypeProperties[lang.Name];

                PrototypeIs[myName] = new List<string>();
                PrototypeIs[myName].Add(myName);

                string name;
                object prototypeRef = prototype;
                if (prototypeProperties.TryGetValue(lang.Prototype, out name))
                {
                    name = name.Trim();
                    if (name.Contains(' '))
                    {
                        string[] names = name.Split(' ');
                        foreach (string n in names) mergePrototpye(ref prototypeRef, myName, n);
                    }
                    else
                    {
                        mergePrototpye(ref prototypeRef, myName, name);
                    }
                }
                Prototypes.Add(myName, prototype);
                AvaliblePrototypes.Add(myName);
            }

            IEnumerable<object> scenes = loaderUtility.GetOfType(root, loaderUtility.SceneType);

            foreach (object scene in scenes)
            {
                HashSet<string> sceneWords = new HashSet<string>() { lang.Name };
                string sceneName = loaderUtility.GetProperties(scene, sceneWords)[lang.Name];
                SceneLoader sceneLoader = new SceneLoader(sceneName, scene, loaderUtility, this);
                Scenes[sceneName] = sceneLoader.LoadedScene;
            }

            return Game;
        }

        public GameLoader
            (string fileName, Assembly [] preloads, float scaleX, float scaleY, Func<Tuple<int, int>> windowOffset)
        {

            Game = new Game(this, scaleX, scaleY, windowOffset);

            string extention = fileName.Substring(fileName.IndexOf('.'));
            if (".xml" == extention)
            {
                loaderUtility = new XMLLoaderUtility(fileName, this);
            }
            else
            {
                throw new ArgumentException("File " + fileName + " could not be loaded");
            }

            foreach (Assembly loaded in preloads)
            {
                string name = Path.GetFileName(loaded.Location);
                if (!LoadedFiles.ContainsKey(name))
                {
                    LoadedFiles.Add(name, loaded);
                    ClassFactory.LoadServices(loaded);
                }
            }
            lang = loaderUtility.Lang;
            Prototypes = new Dictionary<string, object>();
        }

        internal Scene GetScene(string name) { return Scenes[name].Create(); }

        private void loadReflection(string file, string named, string fullName)
        {
            Assembly assembly = null;
            if (!LoadedFiles.ContainsKey(file))
            {
                assembly =  Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, lang.Plugins, file));
                ClassFactory.LoadServices(assembly);
                LoadedFiles.Add(file, assembly);
            }
            else
            {
                assembly = LoadedFiles[file];
            }
            ClassFactory.RegisterType(named, assembly.GetType(fullName));
        }

        private void mergePrototpye(ref object newPrototype, string myName, string mergeWith)
        {
            loaderUtility.MergePrototpye(ref newPrototype, myName, mergeWith);
            foreach (string isPrototype in PrototypeIs[mergeWith])
            {
                if (!PrototypeIs[myName].Contains(isPrototype)) PrototypeIs[myName].Add(isPrototype);
            }
        }
    }
}
