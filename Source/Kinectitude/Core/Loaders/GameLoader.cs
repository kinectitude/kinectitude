using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    public abstract class GameLoader
    {

        public string FirstScene { get; protected set; }

        protected static Dictionary<string, Assembly> LoadedFiles = new Dictionary<string, Assembly>();
        internal readonly HashSet<string> AvaliblePrototypes = new HashSet<string>();
        internal readonly Dictionary<string, List<string>> PrototypeIs = new Dictionary<string, List<string>>();
        public Game Game { get; private set; }

        /**
         * Factory method for loading files.  We don't need to expose XMLGameLoader this way 
         */
        public static GameLoader GetGameLoader(string fileName, Assembly [] preloads)
        {
            string extention = fileName.Substring(fileName.IndexOf('.'));
            GameLoader gameLoader;
            if (".xml" == extention)
            {
                gameLoader = new XMLGameLoader(fileName);
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
            return gameLoader;
        }

        protected GameLoader()
        {
            Game = new Game(this);
        }

        internal abstract SceneLoader GetSceneLoader(string name);

        protected void LoadReflection(string file, string named, string fullName)
        {
            Assembly assembly = null;
            if (!LoadedFiles.ContainsKey(file))
            {
                assembly = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "Plugins", file));
                ClassFactory.LoadServices(assembly);
                LoadedFiles.Add(file, assembly);
            }
            else
            {
                assembly = LoadedFiles[file];
            }
            ClassFactory.RegisterType(named, assembly.GetType(fullName));
        }
    }
}
