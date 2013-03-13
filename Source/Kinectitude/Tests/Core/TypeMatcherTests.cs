﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class TypeMatcherTests
    {
        [TestMethod]
        public void TestTypeMatcher()
        {
            Game game = Setup.StartGame("Core/typeMatcher.kgl");
            game.OnUpdate(1/60);
            AssertionAction.CheckValue("$run");
            AssertionAction.CheckValue("$p1", 3);
            AssertionAction.CheckValue("$p2", 3);
            AssertionAction.CheckValue("$p3");
            AssertionAction.CheckValue("$p4");
            AssertionAction.CheckValue("#run");
            AssertionAction.CheckValue("#p1", 2);
            AssertionAction.CheckValue("#p2", 2);
            AssertionAction.CheckValue("#p3");
            AssertionAction.CheckValue("#p4");
            AssertionAction.CheckValue("^e1.value");
        }
    }
}
