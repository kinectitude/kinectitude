using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Kinectitude.Core
{
    public abstract class GameLoader
    {
        protected Dictionary<string, Assembly> loadedFiles = new Dictionary<string, Assembly>();

        internal Dictionary<string, MethodInfo> MemberSetters { get; private set; }
        internal Dictionary<Type, object> Services { get; private set; }
        internal Dictionary<string, Type> MemberType { get; private set; }
        internal Dictionary<string, Type> TypesDict { get; private set; }
        public Game Game { get; protected set; }

        /**
         * Factory method for loading files.  We don't need to expose XMLGameLoader this way 
         */
        public static GameLoader GetGameLoader(string fileName)
        {
            string extention = fileName.Substring(fileName.IndexOf('.'));
            if (".xml" == extention)
            {
                return new XMLGameLoader(fileName);
            }
            throw new ArgumentException("File " + fileName + " could not be loaded");
        }

        protected GameLoader()
        {
            TypesDict = new Dictionary<string, Type>();
            MemberType = new Dictionary<string, Type>();
            Services = new Dictionary<Type, object>();
            MemberSetters = new Dictionary<string, MethodInfo>();
            TypesDict["IncrementAction"] = typeof(IncrementAction);
            TypesDict["AttributeChangesEvent"] = typeof(AttributeChangesEvent);
            TypesDict["AttributeEqualsEvent"] = typeof(AttributeEqualsEvent);
            TypesDict["SetAttributeAction"] = typeof(SetAttributeAction);
            TypesDict["PushSceneAction"] = typeof(PushSceneAction);
            TypesDict["PopSceneAction"] = typeof(PopSceneAction);
            TypesDict["ChangeSceneAction"] = typeof(ChangeSceneAction);
            TypesDict["SceneStartEvent"] = typeof(SceneStartsEvent);
            TypesDict["FireTriggerAction"] = typeof(FireTriggerAction);
            TypesDict["TriggerOccursEvent"] = typeof(TriggerOccursEvent);
            Game = new Game(this);
        }

        public void RegisterService(Type type, object service)
        {
            Services.Add(type, service);
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
            TypesDict[named] =
                assembly.GetType(fullName);
        }
    }
}
