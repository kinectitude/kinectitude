//-----------------------------------------------------------------------
// <copyright file="defaults.cs" company="Kinectitude">
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
    public class Defaults
    {
        [TestMethod]
        public void DefaultTest()
        {
            Setup.StartGame("Core/defaults.kgl");
            AssertionAction.CheckValue("Int");
            AssertionAction.CheckValue("Double");
            AssertionAction.CheckValue("Str");
            AssertionAction.CheckValue("Enum");
            AssertionAction.CheckValue("Bool");
            AssertionAction.CheckValue("Reader");
        }
    }
}
