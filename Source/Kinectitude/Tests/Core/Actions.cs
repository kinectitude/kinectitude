using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Actions
    {
        private const string actionFile = "core/actions.kgl";
        private const string timerFile = "core/timers.kgl";

        static Actions()
        {
            Setup.RunGame("actions.kgl");
        }
        
        [TestMethod]
        [DeploymentItem(actionFile)]
        public void CreateEntityAction()
        {
            //Once to check OnCreate, once to see it is created
            AssertionAction.CheckValue("Created new entity", 2);
        }

        //TODO
        [TestMethod]
        public void DestroyAction()
        {
        }

        [TestMethod]
        [DeploymentItem(actionFile)]
        public void SetAndIncrementActions()
        {
            AssertionAction.CheckValue("game's val is correct");
        }

        [TestMethod]
        [DeploymentItem(timerFile)]
        [DeploymentItem(actionFile)]
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
        [DeploymentItem(actionFile)]
        public void SceneActions()
        {
            AssertionAction.CheckValue("Got back from scene 2");
            AssertionAction.CheckValue("Got back from scene 3");
        }
    }
}
