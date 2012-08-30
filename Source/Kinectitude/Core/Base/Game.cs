﻿using System;
using System.Collections.Generic;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Base
{

    public class Game : DataContainer, IUpdateable
    {
        //public to see if types exist
        public readonly GameLoader GameLoader;
#if TEST
        internal readonly Stack<Scene> currentScenes = new Stack<Scene>();
#else
        private readonly Stack<Scene> currentScenes = new Stack<Scene>();
#endif
        private readonly Dictionary<Type, Service> services = new Dictionary<Type, Service>();

        internal static Game CurrentGame { get; private set; }
        
        public int Width
        {
            get { return null != this["Width"] ? int.Parse(this["Width"]) : 800; }
        }

        public int Height
        {
            get { return null != this["Height"] ? int.Parse(this["Height"]) : 600; }
        }

        internal Game(GameLoader gameLoader) : base(-2)
        {
            this.GameLoader = gameLoader;
            CurrentGame = this;
        }

        public void Start()
        {
            Scene main = GameLoader.GetScene(GameLoader.FirstScene);
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
            Scene run = GameLoader.GetScene(name);
            currentScenes.Push(run);
            run.Running = true;
        }

        internal void PushScene(string name)
        {
            currentScenes.Peek().Running = false;
            Scene run = GameLoader.GetScene(name);
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


        internal override Changeable GetComponentOrManager(string name)
        {
            return null;
        }
    }
}
