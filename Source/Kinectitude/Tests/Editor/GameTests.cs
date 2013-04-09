//-----------------------------------------------------------------------
// <copyright file="GameTests.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Linq;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Exceptions;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class GameTests
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
            Game game = new Game("Test Game");

            Assert.AreEqual(game.Width, 800);
        }

        [TestMethod]
        public void SetHeight()
        {
            Game game = new Game("Test Game");

            Assert.AreEqual(game.Height, 600);
        }

        [TestMethod]
        public void SetFirstScene()
        {
            bool propertyChanged = false;

            Scene scene = new Scene("Test Scene");
            
            Game game = new Game("Test Game");
            game.PropertyChanged += (o, e) => propertyChanged = (e.PropertyName == "FirstScene");

            game.FirstScene = scene;

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual(game.FirstScene.Name, "Test Scene");
        }

        [TestMethod]
        public void AddUsing()
        {
            bool collectionChanged = false;

            Game game = new Game("Test Game");
            game.Usings.CollectionChanged += (o, e) => collectionChanged = true;

            Using use = new Using() { File = "Test.dll" };
            game.AddUsing(use);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(game.Usings.Count(), 1);
            Assert.AreEqual(game.Usings.First().File, "Test.dll");
        }

        [TestMethod]
        public void RemoveUsing()
        {
            int eventsFired = 0;

            Game game = new Game("Test Game");
            game.Usings.CollectionChanged += (o, e) => eventsFired++;

            Using use = new Using() { File = "Test.dll" };
            game.AddUsing(use);
            game.RemoveUsing(use);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(game.Usings.Count(), 0);
        }

        [TestMethod]
        public void AddComponentWithoutExistingDefine()
        {
            bool firedPluginUsed = false;
            bool firedDefineAdded = false;

            Game game = new Game("Test Game");
            game.AddHandler<PluginUsed>(n => firedPluginUsed = true);
            game.AddHandler<DefineAdded>(n => firedDefineAdded = true);

            Entity entity = new Entity() { Name = "entity" };
            game.AddPrototype(entity);

            Component component = new Component(game.GetPlugin(TransformComponentType));
            entity.AddComponent(component);

            Assert.IsTrue(firedPluginUsed);
            Assert.IsTrue(firedDefineAdded);
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
            bool collectionChanged = false;

            Game game = new Game("Test Game");
            game.Prototypes.CollectionChanged += (o, e) => collectionChanged = true;

            Entity entity = new Entity() { Name = "TestPrototype" };
            game.AddPrototype(entity);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(game.Prototypes.Count(), 1);
            Assert.AreEqual(game.Prototypes.First().Name, "TestPrototype");
        }

        [TestMethod]
        public void RemovePrototype()
        {
            int eventsFired = 0;

            Game game = new Game("Test Game");
            game.Prototypes.CollectionChanged += (o, e) => eventsFired++;

            Entity entity = new Entity() { Name = "TestPrototype" };
            game.AddPrototype(entity);
            game.RemovePrototype(entity);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(game.Prototypes.Count(), 0);
        }

        [TestMethod]
        public void AddAttribute()
        {
            bool collectionChanged = false;

            Game game = new Game("Test Game");
            game.Attributes.CollectionChanged += (o, e) => collectionChanged = true;

            Attribute attribute = new Attribute("test");
            game.AddAttribute(attribute);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(game.Attributes.Count(x => x.Name == "test"), 1);
        }

        [TestMethod]
        public void RemoveAttribute()
        {
            int eventsFired = 0;

            Game game = new Game("Test Game");
            game.Attributes.CollectionChanged += (o, e) => eventsFired++;

            Attribute attribute = new Attribute("test");
            game.AddAttribute(attribute);
            game.RemoveAttribute(attribute);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(game.Attributes.Count(x => x.Name == "test"), 0);
        }

        [TestMethod]
        public void AddScene()
        {
            bool collectionChanged = false;

            Game game = new Game("Test Game");
            game.Scenes.CollectionChanged += (o, e) => collectionChanged = true;

            Scene scene = new Scene("Test Scene");
            game.AddScene(scene);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(game.Scenes.Count(), 1);
            Assert.AreEqual(game.Scenes.First().Name, "Test Scene");
        }

        [TestMethod]
        public void RemoveScene()
        {
            int eventsFired = 0;

            Game game = new Game("Test Game");
            game.Scenes.CollectionChanged += (o, e) => eventsFired++;

            Scene scene = new Scene("Test Scene");
            game.AddScene(scene);
            game.RemoveScene(scene);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(game.Scenes.Count(), 0);
        }

        //[TestMethod]
        //public void AddAsset()
        //{
        //    Game game = new Game("Test Game");
            
        //    Asset asset = new Asset("Test Asset");
        //    game.AddAsset(asset);

        //    Assert.AreEqual(game.Assets.Count(x => x.FileName == "Test Asset"), 1);
        //}

        //[TestMethod]
        //public void RemoveAsset()
        //{
        //    Game game = new Game("Test Game");
            
        //    Asset asset = new Asset("Test Asset");
        //    game.AddAsset(asset);
        //    game.RemoveAsset(asset);

        //    Assert.AreEqual(game.Assets.Count(x => x.FileName == "Test Asset"), 0);
        //}

        //[TestMethod]
        //public void GameAttributeCannotInherit()
        //{
        //    Game game = new Game("Test Game");
            
        //    Attribute attribute = new Attribute("test");
        //    game.AddAttribute(attribute);

        //    Assert.IsFalse(attribute.CanInherit);
        //}

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

        [TestMethod, ExpectedException(typeof(InvalidPrototypeNameException))]
        public void PrototypeMustHaveName()
        {
            bool collectionChanged = false;

            Game game = new Game("Test Game");
            game.Prototypes.CollectionChanged += (o, e) => collectionChanged = true;

            game.AddPrototype(new Entity());

            Assert.IsFalse(collectionChanged);
            Assert.AreEqual(0, game.Prototypes.Count);
        }

        [TestMethod, ExpectedException(typeof(PrototypeExistsException))]
        public void CannotAddDuplicatePrototypeName()
        {
            int eventsFired = 0;

            Game game = new Game("Test Game");
            game.Prototypes.CollectionChanged += (o, e) => eventsFired++;

            game.AddPrototype(new Entity() { Name = "prototype" });
            game.AddPrototype(new Entity() { Name = "prototype" });

            Assert.AreEqual(1, eventsFired);
            Assert.AreEqual(1, game.Prototypes.Count(x => x.Name == "prototype" ));
        }
    }
}
