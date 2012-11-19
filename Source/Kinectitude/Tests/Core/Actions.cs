using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Actions
    {
        private const string actionFile = "core/actions.kgl";

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

        [TestMethod]
        [DeploymentItem(actionFile)]
        public void SetAndIncrementActions()
        {
            AssertionAction.CheckValue("game's val is correct");
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
