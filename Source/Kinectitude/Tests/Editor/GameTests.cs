using System.IO;
using System.Linq;
using Kinectitude.Core.Components;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class GameViewModelTests
    {
        private static readonly string TransformComponentType = typeof(TransformComponent).FullName;
        private static readonly string TransformComponentShort = typeof(TransformComponent).Name;

        [TestMethod]
        public void SetName()
        {
            Game game = new Game("Test Game");

            Assert.AreEqual(game.Name, "Test Game");
        }

        [TestMethod]
        public void SetWidth()
        {
            Game game = new Game("Test Game") { Width = 800 };

            Assert.AreEqual(game.Width, 800);
        }

        [TestMethod]
        public void SetHeight()
        {
            Game game = new Game("Test Game") { Height = 600 };

            Assert.AreEqual(game.Height, 600);
        }

        [TestMethod]
        public void SetIsFullScreen()
        {
            Game game = new Game("Test Game") { IsFullScreen = true };

            Assert.AreEqual(game.IsFullScreen, true);
        }

        [TestMethod]
        public void SetFirstScene()
        {
            Scene scene = new Scene("Test Scene");
            
            Game game = new Game("Test Game");
            game.FirstScene = scene;

            Assert.AreEqual(game.FirstScene.Name, "Test Scene");
        }

        [TestMethod]
        public void AddUsing()
        {
            Game game = new Game("Test Game");
            
            Using use = new Using() { File = "Test.dll" };
            game.AddUsing(use);

            Assert.AreEqual(game.Usings.Count(), 1);
            Assert.AreEqual(game.Usings.First().File, "Test.dll");
        }

        [TestMethod]
        public void RemoveUsing()
        {
            Game game = new Game("Test Game");
            
            Using use = new Using() { File = "Test.dll" };
            game.AddUsing(use);
            game.RemoveUsing(use);

            Assert.AreEqual(game.Usings.Count(), 0);
        }

        [TestMethod]
        public void AddComponentWithoutExistingDefine()
        {
            Game game = new Game("Test Game");
            
            Entity entity = new Entity() { Name = "entity" };
            
            Component component = new Component(game.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            game.AddPrototype(entity);

            Assert.AreEqual(1, game.Usings.Count);
            Assert.AreEqual(1, game.Usings.Single().Defines.Count(x => x.Name == TransformComponentShort && x.Class == TransformComponentType));
            Assert.AreEqual(TransformComponentShort, component.Type);
        }

        [TestMethod]
        public void AddComponentWithExistingDefine()
        {
            Game game = new Game("Test Game");

            Using use = new Using() { File = Path.GetFileName(typeof(TransformComponent).Assembly.Location) };
            game.AddUsing(use);

            Define define = new Define(TransformComponentShort, TransformComponentType);
            use.AddDefine(define);

            Entity entity = new Entity() { Name = "entity" };
            
            Component component = new Component(game.GetPlugin(TransformComponentShort));
            entity.AddComponent(component);
            game.AddPrototype(entity);
            
            Assert.AreEqual(1, game.Usings.Count);
            Assert.AreEqual(1, game.Usings.Single().Defines.Count(x => x.Name == TransformComponentShort && x.Class == TransformComponentType));
            Assert.AreEqual(TransformComponentShort, component.Type);
        }

        [TestMethod]
        public void AddPrototype()
        {
            Game game = new Game("Test Game");
            
            Entity entity = new Entity() { Name = "TestPrototype" };
            game.AddPrototype(entity);

            Assert.AreEqual(game.Prototypes.Count(), 1);
            Assert.AreEqual(game.Prototypes.First().Name, "TestPrototype");
        }

        [TestMethod]
        public void RemovePrototype()
        {
            Game game = new Game("Test Game");
            
            Entity entity = new Entity() { Name = "TestPrototype" };
            game.AddPrototype(entity);
            game.RemovePrototype(entity);

            Assert.AreEqual(game.Prototypes.Count(), 0);
        }

        [TestMethod]
        public void AddAttribute()
        {
            Game game = new Game("Test Game");
            
            Attribute attribute = new Attribute("test");
            game.AddAttribute(attribute);

            Assert.AreEqual(game.Attributes.Count(x => x.Key == "test"), 1);
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            Game game = new Game("Test Game");
            
            Attribute attribute = new Attribute("test");
            game.AddAttribute(attribute);
            game.RemoveAttribute(attribute);

            Assert.AreEqual(game.Attributes.Count(x => x.Key == "test"), 0);
        }

        [TestMethod]
        public void AddScene()
        {
            Game game = new Game("Test Game");
            
            Scene scene = new Scene("Test Scene");
            game.AddScene(scene);

            Assert.AreEqual(game.Scenes.Count(), 1);
            Assert.AreEqual(game.Scenes.First().Name, "Test Scene");
        }

        [TestMethod]
        public void RemoveScene()
        {
            Game game = new Game("Test Game");
            
            Scene scene = new Scene("Test Scene");
            game.AddScene(scene);
            game.RemoveScene(scene);

            Assert.AreEqual(game.Scenes.Count(), 0);
        }

        [TestMethod]
        public void AddAsset()
        {
            Game game = new Game("Test Game");
            
            Asset asset = new Asset("Test Asset");
            game.AddAsset(asset);

            Assert.AreEqual(game.Assets.Count(x => x.FileName == "Test Asset"), 1);
        }

        [TestMethod]
        public void RemoveAsset()
        {
            Game game = new Game("Test Game");
            
            Asset asset = new Asset("Test Asset");
            game.AddAsset(asset);
            game.RemoveAsset(asset);

            Assert.AreEqual(game.Assets.Count(x => x.FileName == "Test Asset"), 0);
        }

        [TestMethod]
        public void GameAttributeCannotInherit()
        {
            Game game = new Game("Test Game");
            
            Attribute attribute = new Attribute("test");
            game.AddAttribute(attribute);

            Assert.IsFalse(attribute.CanInherit);
        }

        [TestMethod]
        public void GetDefinedPlugin()
        {
            Game game = new Game("Test Game");

            Using use = new Using() { File = "Kinectitude.Core.dll" };
            use.AddDefine(new Define(TransformComponentShort, TransformComponentType));
            game.AddUsing(use);

            Plugin plugin = game.GetPlugin(TransformComponentShort);

            Assert.IsNotNull(plugin);
            Assert.AreEqual(TransformComponentType, plugin.ClassName);
        }

        [TestMethod]
        public void GetPluginByFullName()
        {
            Game game = new Game("Test Game");
            Plugin plugin = game.GetPlugin(TransformComponentType);

            Assert.IsNotNull(plugin);
            Assert.AreEqual(TransformComponentType, plugin.ClassName);
        }

        [TestMethod]
        public void PrototypeMustHaveName()
        {
            Game game = new Game("Test Game");
            game.AddPrototype(new Entity());

            Assert.AreEqual(0, game.Prototypes.Count);
        }

        [TestMethod]
        public void CannotAddDuplicatePrototypeName()
        {
            Game game = new Game("Test Game");
            game.AddPrototype(new Entity() { Name = "prototype" });
            game.AddPrototype(new Entity() { Name = "prototype" });

            Assert.AreEqual(1, game.Prototypes.Count(x => x.Name == "prototype" ));
        }
    }
}
