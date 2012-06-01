using System.Collections.Generic;
using Kinectitude.Editor.Models.Plugins;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Game : AttributeContainer, IEntityContainer
    {
        private string name;
        private string description;
        private int width;
        private int height;
        private bool fullScreen;
        private Scene firstScene;
        private readonly IPluginNamespace pluginNamespace;
        private readonly SortedDictionary<string, Using> usings;
        private readonly SortedDictionary<string, Entity> prototypes;
        private readonly SortedDictionary<string, Scene> scenes;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public bool IsFullScreen
        {
            get { return fullScreen; }
            set { fullScreen = value; }
        }

        public Scene FirstScene
        {
            get { return firstScene; }
            set { firstScene = value; }
        }

        public IEnumerable<Using> Usings
        {
            get { return usings.Values; }
        }

        public IEnumerable<Scene> Scenes
        {
            get { return scenes.Values; }
        }

        public IEnumerable<Entity> Entities
        {
            get { return prototypes.Values; }
        }

        public Game(IPluginNamespace pluginNamespace)
        {
            this.pluginNamespace = pluginNamespace;
            usings = new SortedDictionary<string, Using>();
            prototypes = new SortedDictionary<string, Entity>();
            scenes = new SortedDictionary<string, Scene>();
        }

        public void AddScene(Scene scene)
        {
            scene.Parent = this;
            scenes.Add(scene.Name, scene);
        }

        public void RemoveScene(Scene scene)
        {
            scene.Parent = null;
            scenes.Remove(scene.Name);
        }

        public Entity GetPrototype(string name)
        {
            Entity ret = null;
            prototypes.TryGetValue(name, out ret);
            return ret;
        }

        public void AddEntity(Entity entity)
        {
            entity.Parent = this;
            prototypes.Add(entity.Name, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Parent = null;
            prototypes.Remove(entity.Name);
        }

        public void AddUsing(Using use)
        {
            usings.Add(use.File, use);
        }

        public void RemoveUsing(Using use)
        {
            usings.Remove(use.File);
        }

        public Scene GetScene(string name)
        {
            Scene ret = null;
            scenes.TryGetValue(name, out ret);
            return ret;
        }

        public PluginDescriptor GetPluginDescriptor(string name)
        {
            foreach (Using use in Usings)
            {
                Define define = use.GetDefine(name);
                if (null != define)
                {
                    name = define.Class;
                    break;
                }
            }
            return pluginNamespace.GetPluginDescriptor(name);
        }
    }
}
