using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Linq;
using SlimDX.Direct2D;

namespace Kinectitude.Core
{
    public delegate void RenderDelegate(RenderTarget renderTarget);

    public class Game : DataContainer, IUpdateable
    {
        private readonly Stack<Scene> currentScenes;
        private readonly GameLoader gameLoader;

        public RenderDelegate OnRender;

        public new string Name
        {
            get { return this["name"] ?? string.Empty; }
        }
        
        public int Width
        {
            get { return null != this["width"] ? int.Parse(this["width"]) : 800; }
        }

        public int Height
        {
            get { return null != this["height"] ? int.Parse(this["height"]) : 600; }
        }

        internal Game(GameLoader gameLoader) : base(-1)
        {
            currentScenes = new Stack<Scene>();
            this.gameLoader = gameLoader;
        }

        public void Start()
        {
            Scene main = gameLoader.GetSceneLoader("main").Scene;
            currentScenes.Push(main);
            main.Running = true;
        }

        public void OnUpdate(double frameDelta)
        {
            Scene currentScene = currentScenes.Peek();
            if (currentScene.Running)
            {
                currentScene.OnUpdate(frameDelta);
            }
        }

        internal Type GetType(string plugin)
        {
            if (!gameLoader.TypesDict.ContainsKey(plugin))
            {
                MessageBox.Show(plugin + " was not loaded!");
            }
            return gameLoader.TypesDict[plugin];
        }

        public void AddService(object obj)
        {
            gameLoader.Services[obj.GetType()] = obj;
        }

        public T GetService<T>() where T : class
        {
            return gameLoader.Services[typeof(T)] as T;
        }

        internal object CreateFromReflection(string name, Type[] constructors, object[] argVals)
        {
            Type componentType = GetType(name);
            ConstructorInfo ci = componentType.GetConstructor(constructors);
            object created = ci.Invoke(argVals);
            return created;
        }
        internal bool SetParam(object obj, string val, string param, Event e = null, SceneLoader s = null)
        {
            Type componentType = obj.GetType();
            //fast to lower with uppercase first letter
            param = param[0].ToString().ToUpper() + param.Substring(1).ToLower();
            MethodInfo mi;
            Type toType;
            if (gameLoader.MemberSetters.ContainsKey(componentType.Name + " " + param))
            {
                mi = gameLoader.MemberSetters[componentType.Name + " " + param];
                toType = gameLoader.MemberType[componentType.Name + " " + param];
            }
            else
            {
                PropertyInfo pi = componentType.GetProperty(param);
                mi = pi.GetSetMethod();
                toType = pi.PropertyType;
                gameLoader.MemberType[componentType.Name + " " + param] = toType;
                gameLoader.MemberSetters[componentType.Name + " " + param] = mi;
            }
            if (toType == typeof(string))
            {
                mi.Invoke(obj, new object[] { val });
            }
            else if (toType == typeof(double))
            {
                double tmp = double.Parse(val);
                mi.Invoke(obj, new object[] { tmp });
            }
            else if (toType == typeof(float))
            {
                //odd Float is not a thing?
                float tmp = float.Parse(val);
                mi.Invoke(obj, new object[] { tmp });
            }
            else if (toType == typeof(int))
            {
                int tmp = int.Parse(val);
                mi.Invoke(obj, new object[] { tmp });
            }
            else if (toType == typeof(long))
            {
                long tmp = int.Parse(val);
                mi.Invoke(obj, new object[] { tmp });
            }
            else if(toType == typeof(SpecificReadable))
            {
                SpecificReadable sr = SpecificReadable.CreateSpecificReadable(val, e, s);
                mi.Invoke(obj, new object[] { sr });
            }
            else if(toType == typeof(ReadableData))
            {
                ReadableData rd = ReadableData.CreateReadableData(val, e, s);
                mi.Invoke(obj, new object[] { rd });
            }
            else if (toType == typeof(SpecificWriter))
            {
                string set;
                WriteableData wd;
                string who;
                if (val.Contains('.'))
                {
                    string[] split = val.Split('.');
                    who = split[0];
                    set = split[1];
                }
                else
                {
                    who = "this";
                    set = val;
                }
                wd = WriteableData.CreateWriteableData(who, e.Entity, this, s.Scene);
                SpecificWriter sw = 
                    new SpecificWriter(wd, set);
                mi.Invoke(obj, new object[] { sw });
            }
            else if (toType == typeof(WriteableData))
            {
                WriteableData wd = WriteableData.CreateWriteableData(val, e.Entity, this, s.Scene);
                mi.Invoke(obj, new object[] { wd });
            }
            //TODO enum
            else
            {
                MessageBox.Show("Error type " + toType + " is not supported");
                return false;
            }
            return true;
        }

        internal void RunScene(string name)
        {
            //should this be pop?  If so you can go back to a menu or something.  But they may not want that.  I think there should be both
            currentScenes.Pop().Running = false;
            Scene run = gameLoader.GetSceneLoader(name).Scene;
            currentScenes.Push(run);
            run.Running = true;
        }

        internal void PushScene(string name)
        {
            currentScenes.Peek().Running = false;
            Scene run = gameLoader.GetSceneLoader(name).Scene;
            currentScenes.Push(run);
            run.Running = true;
        }

        internal void PopScene()
        {
            currentScenes.Peek().Running = false;
            currentScenes.Pop();
            if (0 == currentScenes.Count)
            {
                Environment.Exit(0);
            }
            currentScenes.Peek().Running = true;
        }
    }
}
