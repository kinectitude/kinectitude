using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Editor
{
    public class Game : AttributeContainer, IEntityContainer
    {
        private string name;
        private string description;
        private int width;
        private int height;
        private bool fullScreen;
        private Scene firstScene;

        private readonly List<Scene> _scenes;
        private readonly List<Entity> _prototypes;
        private readonly ReadOnlyCollection<Scene> scenes;
        private readonly ReadOnlyCollection<Entity> prototypes;

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

        public ReadOnlyCollection<Scene> Scenes
        {
            get { return scenes; }
        }

        public ReadOnlyCollection<Entity> Prototypes
        {
            get { return prototypes; }
        }

        public Game()
        {
            _scenes = new List<Scene>();
            scenes = new ReadOnlyCollection<Scene>(_scenes);

            _prototypes = new List<Entity>();
            prototypes = new ReadOnlyCollection<Entity>(_prototypes);
        }

        public void AddScene(Scene scene)
        {
            scene.Parent = this;
            _scenes.Add(scene);
        }

        public void RemoveScene(Scene scene)
        {
            scene.Parent = null;
            _scenes.Remove(scene);
        }

        public Entity GetPrototypeEntityByName(string name)
        {
            return _prototypes.FirstOrDefault(x => x.Name == name);
        }

        public void AddEntity(Entity entity)
        {
            entity.Parent = this;
            _prototypes.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Parent = null;
            _prototypes.Remove(entity);
        }

        public override string ToString()
        {
            return string.Format("Name: '{0}', Width: {1}, Height: {2}", Name, Width, Height);
        }
    }
}
