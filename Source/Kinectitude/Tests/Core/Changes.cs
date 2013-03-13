﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Changes
    {
        [TestMethod]
        public void ChangeTests()
        {
            Game game = Setup.StartGame("Core/changes.kgl");
            AssertionAction.CheckValue("runTests.val is set");
            AssertionAction.CheckValue("IntVal is set, and attribute equals is run first");
            AssertionAction.CheckValue("scene.x");
            AssertionAction.CheckValue("left shift");
            AssertionAction.CheckValue("right shift");
            AssertionAction.CheckValue("balls + rofl", 2);
            AssertionAction.CheckValue("Multiply words");
            AssertionAction.CheckValue("divide");
            AssertionAction.CheckValue("-ve");
            AssertionAction.CheckValue("not");
            AssertionAction.CheckValue("balls - rofl", 2);
            AssertionAction.CheckValue("balls / 1");
            AssertionAction.CheckValue("balls % 2");
            AssertionAction.CheckValue("balls ** 2");
            AssertionAction.CheckValue("or");
            AssertionAction.CheckValue("eql", 2);
            AssertionAction.CheckValue("neq", 2);
            AssertionAction.CheckValue("lt", 3);
            AssertionAction.CheckValue("le", 2);
            AssertionAction.CheckValue("gt", 2);
            AssertionAction.CheckValue("ge", 3);
            AssertionAction.CheckValue("and 2", 4);
            AssertionAction.CheckValue("and", 4);
        }
    }
}
