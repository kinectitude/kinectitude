using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Game : IEntityContainer, IAttributeContainer
    {
        private readonly List<Using> usings;
        private readonly List<Asset> assets;
        private readonly List<Entity> prototypes;
        private readonly List<Scene> scenes;
        private readonly List<Attribute> attributes;

        public string Name
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public bool IsFullScreen
        {
            get;
            set;
        }

        public string FirstScene
        {
            get;
            set;
        }

        public IEnumerable<Using> Usings
        {
            get { return usings; }
        }

        public IEnumerable<Asset> Assets
        {
            get { return assets; }
        }

        public IEnumerable<Entity> Entities
        {
            get { return prototypes; }
        }

        public IEnumerable<Attribute> Attributes
        {
            get { return attributes; }
        }

        public IEnumerable<Scene> Scenes
        {
            get { return scenes; }
        }

        public Game()
        {
            usings = new List<Using>();
            assets = new List<Asset>();
            prototypes = new List<Entity>();
            attributes = new List<Attribute>();
            scenes = new List<Scene>();
        }

        public void AddUsing(Using use)
        {
            usings.Add(use);
        }

        public void RemoveUsing(Using use)
        {
            usings.Remove(use);
        }

        public void AddAsset(Asset asset)
        {
            assets.Add(asset);
        }

        public void RemoveAsset(Asset asset)
        {
            assets.Remove(asset);
        }

        public void AddAttribute(Attribute attribute)
        {
            attributes.Add(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attributes.Remove(attribute);
        }

        public void AddScene(Scene scene)
        {
            scenes.Add(scene);
        }

        public void RemoveScene(Scene scene)
        {
            scenes.Remove(scene);
        }

        public void AddEntity(Entity entity)
        {
            prototypes.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            prototypes.Remove(entity);
        }
    }
}
