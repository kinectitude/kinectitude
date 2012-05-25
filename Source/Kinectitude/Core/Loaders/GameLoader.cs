using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Kinectitude.Core.Base;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    public abstract class GameLoader
    {
        protected static Dictionary<string, Assembly> loadedFiles = new Dictionary<string, Assembly>();

        internal Dictionary<string, MethodInfo> MemberSetters { get; private set; }
        internal Dictionary<string, Type> MemberType { get; private set; }
        public Game Game { get; protected set; }

        /**
         * Factory method for loading files.  We don't need to expose XMLGameLoader this way 
         */
        public static GameLoader GetGameLoader(string fileName, Assembly [] preloads)
        {
            foreach (Assembly loaded in preloads)
            {
                loadedFiles.Add(loaded.Location, loaded);
            }
            string extention = fileName.Substring(fileName.IndexOf('.'));
            if (".xml" == extention)
            {
                return new XMLGameLoader(fileName);
            }
            throw new ArgumentException("File " + fileName + " could not be loaded");
        }

        protected GameLoader()
        {
            MemberType = new Dictionary<string, Type>();
            MemberSetters = new Dictionary<string, MethodInfo>();
            Game = new Game(this);
        }

        internal abstract SceneLoader GetSceneLoader(string name);

        protected void LoadReflection(string file, string named, string fullName)
        {
            Assembly assembly = null;
            if (!loadedFiles.ContainsKey(file))
            {
                assembly = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "Plugins", file));
                loadedFiles.Add(file, assembly);
            }
            else
            {
                assembly = loadedFiles[file];
            }
            ClassFactory.RegisterType(named, assembly.GetType(fullName));
        }
    }
}
