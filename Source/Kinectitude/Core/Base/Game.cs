using System;
using System.Collections.Generic;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Base;
using Kinectitude.Attributes;

namespace Kinectitude.Core.Base
{

    public class Game : DataContainer, IUpdateable
    {
        private readonly GameLoader gameLoader;
        private readonly Stack<Scene> currentScenes = new Stack<Scene>();
        private readonly Dictionary<Type, Service> services = new Dictionary<Type, Service>();

        internal static Game CurrentGame { get; private set; }

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

        internal Game(GameLoader gameLoader) : base(-2)
        {
            this.gameLoader = gameLoader;
            CurrentGame = this;
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

        internal void RunScene(string name)
        {
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

        internal void Quit()
        {
            foreach (Service service in services.Values)
            {
                if (service.Running)
                {
                    service.Stop();
                }
            }
            Environment.Exit(0);
        }

        internal void PopScene()
        {
            currentScenes.Peek().Running = false;
            currentScenes.Pop();
            if (0 == currentScenes.Count)
            {
                Quit();
            }
            currentScenes.Peek().Running = true;
        }

        internal void SetService(Service service)
        {
            services[service.GetType()] = service;
        }

        public T GetService<T>() where T : Service
        {
            return services[typeof(T)] as T;
        }

    }
}
