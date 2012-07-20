using System.IO;
using System.Linq;
using Kinectitude.Core.Components;
using Kinectitude.Editor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            GameViewModel game = new GameViewModel("Test Game");

            Assert.AreEqual(game.Name, "Test Game");
        }

        [TestMethod]
        public void SetWidth()
        {
            GameViewModel game = new GameViewModel("Test Game") { Width = 800 };

            Assert.AreEqual(game.Width, 800);
        }

        [TestMethod]
        public void SetHeight()
        {
            GameViewModel game = new GameViewModel("Test Game") { Height = 600 };

            Assert.AreEqual(game.Height, 600);
        }

        [TestMethod]
        public void SetIsFullScreen()
        {
            GameViewModel game = new GameViewModel("Test Game") { IsFullScreen = true };

            Assert.AreEqual(game.IsFullScreen, true);
        }

        [TestMethod]
        public void SetFirstScene()
        {
            SceneViewModel scene = new SceneViewModel("Test Scene");
            
            GameViewModel game = new GameViewModel("Test Game");
            game.FirstScene = scene;

            Assert.AreEqual(game.FirstScene.Name, "Test Scene");
        }

        [TestMethod]
        public void AddUsing()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            UsingViewModel use = new UsingViewModel() { File = "Test.dll" };
            game.AddUsing(use);

            Assert.AreEqual(game.Usings.Count(), 1);
            Assert.AreEqual(game.Usings.First().File, "Test.dll");
        }

        [TestMethod]
        public void RemoveUsing()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            UsingViewModel use = new UsingViewModel() { File = "Test.dll" };
            game.AddUsing(use);
            game.RemoveUsing(use);

            Assert.AreEqual(game.Usings.Count(), 0);
        }

        [TestMethod]
        public void AddComponentWithoutExistingDefine()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            EntityViewModel entity = new EntityViewModel() { Name = "entity" };
            
            ComponentViewModel component = new ComponentViewModel(game.GetPlugin(TransformComponentType));
            entity.AddComponent(component);
            game.AddPrototype(entity);

            Assert.AreEqual(1, game.Usings.Count);
            Assert.AreEqual(1, game.Usings.Single().Defines.Count(x => x.Name == TransformComponentShort && x.Class == TransformComponentType));
            Assert.AreEqual(TransformComponentShort, component.Type);
        }

        [TestMethod]
        public void AddComponentWithExistingDefine()
        {
            GameViewModel game = new GameViewModel("Test Game");

            UsingViewModel use = new UsingViewModel() { File = Path.GetFileName(typeof(TransformComponent).Assembly.Location) };
            game.AddUsing(use);

            DefineViewModel define = new DefineViewModel(TransformComponentShort, TransformComponentType);
            use.AddDefine(define);

            EntityViewModel entity = new EntityViewModel() { Name = "entity" };
            
            ComponentViewModel component = new ComponentViewModel(game.GetPlugin(TransformComponentShort));
            entity.AddComponent(component);
            game.AddPrototype(entity);
            
            Assert.AreEqual(1, game.Usings.Count);
            Assert.AreEqual(1, game.Usings.Single().Defines.Count(x => x.Name == TransformComponentShort && x.Class == TransformComponentType));
            Assert.AreEqual(TransformComponentShort, component.Type);
        }

        [TestMethod]
        public void AddPrototype()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            EntityViewModel entity = new EntityViewModel() { Name = "TestPrototype" };
            game.AddPrototype(entity);

            Assert.AreEqual(game.Prototypes.Count(), 1);
            Assert.AreEqual(game.Prototypes.First().Name, "TestPrototype");
        }

        [TestMethod]
        public void RemovePrototype()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            EntityViewModel entity = new EntityViewModel() { Name = "TestPrototype" };
            game.AddPrototype(entity);
            game.RemovePrototype(entity);

            Assert.AreEqual(game.Prototypes.Count(), 0);
        }

        [TestMethod]
        public void AddAttribute()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            game.AddAttribute(attribute);

            Assert.AreEqual(game.Attributes.Count(x => x.Key == "test"), 1);
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            game.AddAttribute(attribute);
            game.RemoveAttribute(attribute);

            Assert.AreEqual(game.Attributes.Count(x => x.Key == "test"), 0);
        }

        [TestMethod]
        public void AddScene()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            SceneViewModel scene = new SceneViewModel("Test Scene");
            game.AddScene(scene);

            Assert.AreEqual(game.Scenes.Count(), 1);
            Assert.AreEqual(game.Scenes.First().Name, "Test Scene");
        }

        [TestMethod]
        public void RemoveScene()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            SceneViewModel scene = new SceneViewModel("Test Scene");
            game.AddScene(scene);
            game.RemoveScene(scene);

            Assert.AreEqual(game.Scenes.Count(), 0);
        }

        [TestMethod]
        public void AddAsset()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            AssetViewModel asset = new AssetViewModel("Test Asset");
            game.AddAsset(asset);

            Assert.AreEqual(game.Assets.Count(x => x.Name == "Test Asset"), 1);
        }

        [TestMethod]
        public void RemoveAsset()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            AssetViewModel asset = new AssetViewModel("Test Asset");
            game.AddAsset(asset);
            game.RemoveAsset(asset);

            Assert.AreEqual(game.Assets.Count(x => x.Name == "Test Asset"), 0);
        }

        [TestMethod]
        public void GameAttributeCannotInherit()
        {
            GameViewModel game = new GameViewModel("Test Game");
            
            AttributeViewModel attribute = new AttributeViewModel("test");
            game.AddAttribute(attribute);

            Assert.IsFalse(attribute.CanInherit);
        }

        [TestMethod]
        public void GetDefinedPlugin()
        {
            GameViewModel game = new GameViewModel("Test Game");

            UsingViewModel use = new UsingViewModel() { File = "Kinectitude.Core.dll" };
            use.AddDefine(new DefineViewModel(TransformComponentShort, TransformComponentType));
            game.AddUsing(use);

            PluginViewModel plugin = game.GetPlugin(TransformComponentShort);

            Assert.IsNotNull(plugin);
            Assert.AreEqual(TransformComponentType, plugin.ClassName);
        }

        [TestMethod]
        public void GetPluginByFullName()
        {
            GameViewModel game = new GameViewModel("Test Game");
            PluginViewModel plugin = game.GetPlugin(TransformComponentType);

            Assert.IsNotNull(plugin);
            Assert.AreEqual(TransformComponentType, plugin.ClassName);
        }

        [TestMethod]
        public void PrototypeMustHaveName()
        {
            GameViewModel game = new GameViewModel("Test Game");
            game.AddPrototype(new EntityViewModel());

            Assert.AreEqual(0, game.Prototypes.Count);
        }

        [TestMethod]
        public void CannotAddDuplicatePrototypeName()
        {
            GameViewModel game = new GameViewModel("Test Game");
            game.AddPrototype(new EntityViewModel() { Name = "prototype" });
            game.AddPrototype(new EntityViewModel() { Name = "prototype" });

            Assert.AreEqual(1, game.Prototypes.Count(x => x.Name == "prototype" ));
        }
    }
}
