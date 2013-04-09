//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Base
{

    internal class Game : DataContainer, IUpdateable
    {
        internal readonly GameLoader GameLoader;
        internal static Game CurrentGame { get; private set; }
        internal bool Running { get; private set; }

        private readonly Stack<Scene> currentScenes = new Stack<Scene>();

        internal readonly Action<string> Die;
        
        private readonly Dictionary<Type, Service> services = new Dictionary<Type, Service>();

        internal Game(GameLoader gameLoader, Action<string> die) : base(-2)
        {
            this.GameLoader = gameLoader;
            CurrentGame = this;
            Running = true;
            Die = die;
        }

        internal void Start()
        {
            //TODO check if the are us
            foreach (Service service in services.Values)
            {
                if (service.AutoStart()) service.Start();
            }

            Scene main = GameLoader.GetScene(GameLoader.FirstScene);
            Running = true;
            currentScenes.Push(main);
            main.Running = true;
        }

        public void OnUpdate(float frameDelta)
        {
            Scene currentScene = currentScenes.Peek();
            if (Running) currentScene.OnUpdate(frameDelta);
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
                if (service.Running) service.Stop();
            }
            Running = false;
        }

        internal void PopScene()
        {
            Scene currentScene = currentScenes.Peek();
            currentScene.Running = false;
            currentScene.Destroy();
            currentScenes.Pop();
            if (0 == currentScenes.Count) Quit();
            else currentScenes.Peek().Running = true;
        }

        internal void SetService(Service service)
        {
            services[service.GetType()] = service;
        }

        internal T GetService<T>() where T : Service
        {
            return services[typeof(T)] as T;
        }


        internal override Changeable GetChangeable(string name)
        {
            Type type;
            Service s = null;
            if (ClassFactory.TypesDict.TryGetValue(name, out type)) services.TryGetValue(type, out s);
            return s;
        }
    }
}
