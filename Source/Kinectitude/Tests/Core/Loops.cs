//-----------------------------------------------------------------------
// <copyright file="Loops.cs" company="Kinectitude">
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
    public class Loops
    {
        [TestMethod]
        public void LoopTests()
        {
            Setup.StartGame("Core/loops.kgl");
            AssertionAction.CheckValue("basic for", 10);
            AssertionAction.CheckValue("for no first", 10);
            AssertionAction.CheckValue("while", 10);
            AssertionAction.CheckValue("while part in", 2);
        }
    }
}
