using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Actions;
using Kinectitude.Tests.Core.TestMocks;
using Kinectitude.Core.Events;

namespace Kinectitude.Tests.Core.Actions
{
    [TestClass]
    public class CoreActionTests
    {

        static CoreActionTests()
        {
            try
            {
                ClassFactory.RegisterType("component", typeof(TransformComponent));
            }
            catch (ArgumentException) { }
            try
            {
                ClassFactory.RegisterType("action", typeof(SetPositionAction));
            }
            catch (ArgumentException) { }
            try
            {
                ClassFactory.RegisterType("event", typeof(EventMock));
            }
            catch (ArgumentException) { }
            try
            {
                ClassFactory.RegisterType("writer", typeof(WriterMock));
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void SetPosition()
        {
            Entity entity = new Entity(0);
            TransformComponent tc = new TransformComponent();
            tc.X = 10;
            tc.Y = 10;
            entity.AddComponent(tc, "TransformComponent");
            SetPositionAction setPositionAction = new SetPositionAction();
            EventMock evtMock = new EventMock();
            evtMock.Entity = entity;
            setPositionAction.Event = evtMock;
            setPositionAction.X = 100;
            setPositionAction.Y = 100;
            evtMock.AddAction(setPositionAction);
            evtMock.DoActions();
            Assert.IsTrue(tc.X == 100);
            Assert.IsTrue(tc.Y == 100);
        }

        [TestMethod]
        public void SetAttribute()
        {
            SetAction setAction = new SetAction();
            setAction.Value = new ExpressionMock("100");
            WriterMock mock = new WriterMock();
            setAction.Target = mock;
            setAction.Run();
            Assert.IsTrue(mock.Value == "100");
        }

        [TestMethod]
        public void CreateEntity()
        {
            GameLoaderMock gameLoader = new GameLoaderMock();
            Game game = new Game(gameLoader);
            SceneLoaderMock sceneLoader = new SceneLoaderMock(gameLoader);
            sceneLoader.callPrototypeMaker("CreateX");
            Scene scene = new Scene(sceneLoader, game);
            Entity entity = new Entity(0);
            entity.Scene = scene;
            EventMock evtMock = new EventMock();
            evtMock.Entity = entity;
            CreateEntityAction createEntityAction = new CreateEntityAction();
            createEntityAction.Event = evtMock;
            createEntityAction.Prototype = "CreateX";
            createEntityAction.Run();
            Assert.IsTrue(sceneLoader.EntityCreated == "CreateX");
        }
    }
}
