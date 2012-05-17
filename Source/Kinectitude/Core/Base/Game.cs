using System;
using System.Collections.Generic;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Base
{

    public class Game : DataContainer, IUpdateable
    {
        private readonly Stack<Scene> currentScenes;
        private readonly GameLoader gameLoader;

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

        public void OnUpdate(float frameDelta)
        {
            Scene currentScene = currentScenes.Peek();
            if (currentScene.Running)
            {
                currentScene.OnUpdate(frameDelta);
            }
        }

        public void AddService(object obj)
        {
            gameLoader.Services[obj.GetType()] = obj;
        }

        public void RemoveService(Type remove)
        {
            if (null != gameLoader.Services.ContainsKey(remove))
            {
                gameLoader.Services.Remove(remove);
            }
        }

        public T GetService<T>() where T : class
        {
            return gameLoader.Services[typeof(T)] as T;
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
