//-----------------------------------------------------------------------
// <copyright file="RequiresProvides.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class RequiresProvides
    {
        [TestMethod]
        public void ValidProvidesAndRequires()
        {
            Setup.StartGame("Core/validProvidesRequires.kgl");
        }

        [TestMethod]
        public void InvalidRequires()
        {
            List<String> dieMsgs = new List<string>();
            Setup.StartGame("Core/invalidRequires.kgl", new Action<string>(s => dieMsgs.Add(s)));
            Assert.AreEqual<int>(1, dieMsgs.Count);
            Assert.AreEqual<string>("An unnamed entity is missing required component(s): Kinectitude.Core.Components.TransformComponent", dieMsgs[0]);
        }

        [TestMethod]
        public void InvalidProvides()
        {
            List<String> dieMsgs = new List<string>();
            Setup.StartGame("Core/invalidProvides.kgl", new Action<string>(s => dieMsgs.Add(s)));
            Assert.AreEqual<int>(1, dieMsgs.Count);
            Assert.AreEqual<string>("InvalidProvides can't provide Kinectitude.Core.Components.TransformComponent", dieMsgs[0]);
        }
    }
}
