//-----------------------------------------------------------------------
// <copyright file="Actions.cs" company="Kinectitude">
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
    public class Actions
    {
        static Actions()
        {
            Setup.RunGame("Core/actions.kgl");
        }
        
        [TestMethod]
        public void CreateEntityAction()
        {
            //Once to check OnCreate, once to see it is created
            AssertionAction.CheckValue("Created new entity", 2);
        }

        [TestMethod]
        public void SetAndIncrementActions()
        {
            AssertionAction.CheckValue("game's val is correct");
        }

        [TestMethod]
        public void SceneActions()
        {
            AssertionAction.CheckValue("Got back from scene 2");
            AssertionAction.CheckValue("Got back from scene 3");
        }
    }
}
