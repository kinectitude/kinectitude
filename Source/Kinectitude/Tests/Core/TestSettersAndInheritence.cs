using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using System.Reflection;
using Kinectitude.Tests.Core.TestMocks;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class TestSettersAndInheritence
    {
        [TestMethod]
        public void Inheritence()
        {
            string testFile = "core/inheritence.kgl";
            GameLoader gameLoader = new GameLoader(testFile, new Assembly[] { typeof(TestSettersAndInheritence).Assembly }, 1, 1, null);
            Game game = gameLoader.CreateGame();
            game.Start();
            while (game.Running) game.OnUpdate(1 / 60f);
            AssertionAction.CheckValue("Prototype1 X");
            AssertionAction.CheckValue("Prototype1 Y");
            AssertionAction.CheckValue("Prototype1 score");
            AssertionAction.CheckValue("Prototype1 mock2 double val");
            AssertionAction.CheckValue("Prototype1 inheritance Z", 5);
            AssertionAction.CheckValue("Prototype1 mock int val");
            AssertionAction.CheckValue("Prototype2 X");
            AssertionAction.CheckValue("Prototype2 Y");
            AssertionAction.CheckValue("Prototype2 score");
            AssertionAction.CheckValue("Prototype2 mock2 double val");
            AssertionAction.CheckValue("Prototype2 mock int val");
            AssertionAction.CheckValue("Prototype2 test val");
            AssertionAction.CheckValue("Prototype3 X");
            AssertionAction.CheckValue("Prototype3 Y");
            AssertionAction.CheckValue("Prototype3 score");
            AssertionAction.CheckValue("Prototype3 mock2 double val");
            AssertionAction.CheckValue("Prototype3 mock int val");
            AssertionAction.CheckValue("Prototype3 test val");
            AssertionAction.CheckValue("e4 X");
            AssertionAction.CheckValue("e4 Y");
            AssertionAction.CheckValue("e4 score");
            AssertionAction.CheckValue("e4 mock2 double val");
            AssertionAction.CheckValue("e4 mock int val");
            AssertionAction.CheckValue("e4 test val");
        }
    }
}
