using System;
using System.Collections.Generic;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Base
{

    public class Game : DataContainer, IUpdateable
    {
        //public to see if types exist
        public readonly GameLoader GameLoader;
        private readonly Stack<Scene> currentScenes = new Stack<Scene>();
        internal static Game CurrentGame { get; private set; }
        public bool Running { get; private set; }
        
        internal readonly Action<string> Die;

        private readonly Dictionary<Type, Service> services = new Dictionary<Type, Service>();

        private readonly Func<Tuple<int, int>> WindowOffset;
        
        public int Width
        {
            get { return null != this["Width"] ? int.Parse(this["Width"]) : 800; }
        }

        public int Height
        {
            get { return null != this["Height"] ? int.Parse(this["Height"]) : 600; }
        }

        internal float ScaleX { get; private set; }
        internal float ScaleY { get; private set; }

        internal Tuple<int, int> GetWidowOffset() { return WindowOffset(); }
        internal Tuple<int, int> GetWinowSize() { return new Tuple<int, int>(Width, Height); }

        internal Game(GameLoader gameLoader, float scaleX, float scaleY, Func<Tuple<int, int>> windowOffset, Action<string> die) : base(-2)
        {
            this.GameLoader = gameLoader;
            CurrentGame = this;
            ScaleX = scaleX;
            ScaleY = scaleY;
            WindowOffset = windowOffset;
            Running = true;
            Die = die;
        }

        public void Start()
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

        public T GetService<T>() where T : Service
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
