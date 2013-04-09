//-----------------------------------------------------------------------
// <copyright file="Changes.cs" company="Kinectitude">
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
