//-----------------------------------------------------------------------
// <copyright file="TypeMatcherTests.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
