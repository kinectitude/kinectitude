using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Tests.Core.TestMocks;
using Kinectitude.Core.Base;
using Kinectitude.Core.Events;
using Kinectitude.Core.Actions;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class Timer
    {
        [TestMethod]
        public void BasicTimer()
        {
            GameLoaderMock gameLoader = new GameLoaderMock();
            Game game = new Game(gameLoader);
            SceneLoaderMock sceneLoader = new SceneLoaderMock(gameLoader, new LoaderUtilityMock());
            Scene scene = new Scene(sceneLoader, game);
            Entity entity = new Entity(0);
            entity.Scene = scene;
            TriggerOccursEvent trigger = new TriggerOccursEvent();
            ActionMock actionMock = new ActionMock();
            trigger.AddAction(actionMock);
            trigger.Entity = entity;
            ExpressionMock expression = new ExpressionMock("Trigger");
            trigger.Trigger = expression;
            trigger.Initialize();
            CreateTimerAction createTimer = new CreateTimerAction();
            createTimer.Event = trigger;
            createTimer.Duration = new DoubleMock(10);
            createTimer.Name = new ExpressionMock("Timer");
            createTimer.Trigger = expression;
            createTimer.Run();
            scene.OnUpdate(9);
            Assert.IsFalse(actionMock.hasRun);
            scene.OnUpdate(1.1f);
            Assert.IsTrue(actionMock.hasRun);
        }

        [TestMethod]
        public void PauseAndResumeTimer()
        {
            GameLoaderMock gameLoader = new GameLoaderMock();
            Game game = new Game(gameLoader);
            SceneLoaderMock sceneLoader = new SceneLoaderMock(gameLoader, new LoaderUtilityMock());
            Scene scene = new Scene(sceneLoader, game);
            Entity entity = new Entity(0);
            entity.Scene = scene;
            TriggerOccursEvent trigger = new TriggerOccursEvent();
            ActionMock actionMock = new ActionMock();
            trigger.AddAction(actionMock);
            trigger.Entity = entity;
            ExpressionMock expression = new ExpressionMock("Trigger");
            trigger.Trigger = expression;
            trigger.Initialize();
            CreateTimerAction createTimer = new CreateTimerAction();
            createTimer.Event = trigger;
            createTimer.Duration = new DoubleMock(10);
            ExpressionMock timer = new ExpressionMock("Timer");
            createTimer.Name = timer; ;
            createTimer.Trigger = expression;
            createTimer.Run();
            scene.OnUpdate(9);
            Assert.IsFalse(actionMock.hasRun);
            PauseTimersAction pause = new PauseTimersAction();
            pause.Name = timer;
            pause.Event = trigger;
            pause.Run();
            scene.OnUpdate(9);
            Assert.IsFalse(actionMock.hasRun);
            ResumeTimersAction resume = new ResumeTimersAction();
            resume.Name = timer;
            resume.Event = trigger;
            resume.Run();
            scene.OnUpdate(1.1f);
            Assert.IsTrue(actionMock.hasRun);
        }

    }
}
