using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Actions2
    {
        private const string timerFile = "core/timers.kgl";
        private const string destroyEndGame = "core/destroyEndGame.kgl";
        private const string alreadySet = "core/alreadySet.kgl";
        private static Game destroyGame;

        [TestMethod]
        [DeploymentItem(destroyEndGame)]
        public void DestroyAction()
        {
            destroyGame = Setup.StartGame("destroyEndGame.kgl");
            destroyGame.OnUpdate(1 / 60);
            AssertionAction.CheckValue("run trigger");
        }

        [TestMethod]
        [DeploymentItem(destroyEndGame)]
        public void EndGameAction()
        {
            destroyGame = Setup.StartGame("destroyEndGame.kgl");
            destroyGame.OnUpdate(1 / 60);
            Assert.IsFalse(destroyGame.Running);
        }

        [TestMethod]
        [DeploymentItem(timerFile)]
        public void TimerAndTriggerActions()
        {
            Game game = Setup.StartGame("timers.kgl");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t1");
            AssertionAction.CheckValue("t2");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t2", 2);
            AssertionAction.CheckValue("t3");
            AssertionAction.CheckValue("ty");
            game.OnUpdate(1);
            game.OnUpdate(1);
            game.OnUpdate(1);
            game.OnUpdate(1);
            AssertionAction.CheckValue("t2", 2);
            AssertionAction.CheckValue("ty");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t4");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t2", 3);
            game.OnUpdate(1);
            AssertionAction.CheckValue("t1");
            AssertionAction.CheckValue("t2", 4);
            AssertionAction.CheckValue("t3");
            AssertionAction.CheckValue("t4");
        }

        [TestMethod]
        [DeploymentItem(alreadySet)]
        public void alreadySetTest()
        {
            Game game = Setup.StartGame("alreadySet.kgl");
            AssertionAction.CheckValue("changes", 2);
        }
    }
}
