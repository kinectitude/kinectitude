//-----------------------------------------------------------------------
// <copyright file="Conditions.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Conditions
    {
        [TestMethod]
        public void conditionTests()
        {
            Setup.StartGame("Core/conditions.kgl");
            AssertionAction.CheckValue("Should not run", 0);
            AssertionAction.CheckValue("Always run");
            AssertionAction.CheckValue("if ran");
            AssertionAction.CheckValue("else if ran", 2);
            AssertionAction.CheckValue("else ran", 2);
        }
    }
}
