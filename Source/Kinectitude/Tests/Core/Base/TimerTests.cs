using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Tests.Core.TestMocks;
using Kinectitude.Core.Base;
using Kinectitude.Core.Events;
using Kinectitude.Core.Actions;
using Kinectitude.Core.Data;

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
            Assert.IsFalse(actionMock.HasRun);
            scene.OnUpdate(1.1f);
            Assert.IsTrue(actionMock.HasRun);
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
            Assert.IsFalse(actionMock.HasRun);
            PauseTimersAction pause = new PauseTimersAction();
            pause.Name = timer;
            pause.Event = trigger;
            pause.Run();
            scene.OnUpdate(9);
            Assert.IsFalse(actionMock.HasRun);
            ResumeTimersAction resume = new ResumeTimersAction();
            resume.Name = timer;
            resume.Event = trigger;
            resume.Run();
            scene.OnUpdate(1.1f);
            Assert.IsTrue(actionMock.HasRun);
        }

        //This test was added because stoping a timer in a timer threw an exception
        [TestMethod]
        public void StopTimerInTimer()
        {
            GameLoaderMock gameLoader = new GameLoaderMock();
            Game game = new Game(gameLoader);
            SceneLoaderMock sceneLoader = new SceneLoaderMock(gameLoader, new LoaderUtilityMock());
            Scene scene = new Scene(sceneLoader, game);
            Entity entity = new Entity(0);
            entity.Scene = scene;

            SceneStartsEvent startEvt = new SceneStartsEvent();
            startEvt.Entity = entity;
            CreateTimerAction start = new CreateTimerAction();
            start.Duration = new DoubleExpressionReader("5", startEvt, entity);
            start.Name = new ConstantExpressionReader("timer");
            start.Trigger = new ConstantExpressionReader("timer");
            start.Event = startEvt;
            entity.Scene = scene;
            startEvt.AddAction(start);

            PauseTimersAction pause = new PauseTimersAction();
            pause.Name = new ConstantExpressionReader("timer");

            ActionMock am = new ActionMock();

            TriggerOccursEvent triggerOccurs = new TriggerOccursEvent();
            triggerOccurs.Entity = entity;
            triggerOccurs.Trigger = new ConstantExpressionReader("timer");
            triggerOccurs.AddAction(pause);
            triggerOccurs.AddAction(am);

            pause.Event = triggerOccurs;

            scene.RegisterTrigger("timer", triggerOccurs);
            scene.OnStart.Add(startEvt);

            scene.Running = true;
            scene.OnUpdate(5);

            Assert.IsTrue(am.HasRun);
            am.HasRun = false;
            scene.OnUpdate(5);
            //It should not run again because it should be paused
            Assert.IsFalse(am.HasRun);
        }
    }
}
