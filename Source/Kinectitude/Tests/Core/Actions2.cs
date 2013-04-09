//-----------------------------------------------------------------------
// <copyright file="Actions2.cs" company="Kinectitude">
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
    public class Actions2
    {
        private static Game destroyGame;

        [TestMethod]
        public void DestroyAction()
        {
            destroyGame = Setup.StartGame("Core/destroyEndGame.kgl");
            destroyGame.OnUpdate(1 / 60);
            AssertionAction.CheckValue("run trigger");
        }

        [TestMethod]
        public void EndGameAction()
        {
            destroyGame = Setup.StartGame("Core/destroyEndGame.kgl");
            destroyGame.OnUpdate(1 / 60);
            Assert.IsFalse(destroyGame.Running);
        }

        [TestMethod]
        public void TimerAndTriggerActions()
        {
            Game game = Setup.StartGame("Core/timers.kgl");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t1");
            AssertionAction.CheckValue("t2");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t2", 2);
            AssertionAction.CheckValue("t3");
            AssertionAction.CheckValue("ty");
            game.OnUpdate(1);
            game.OnUpdate(1);
            game.OnUpdate(1);
            game.OnUpdate(1);
            AssertionAction.CheckValue("t2", 2);
            AssertionAction.CheckValue("ty");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t4");
            game.OnUpdate(1);
            AssertionAction.CheckValue("t2", 3);
            game.OnUpdate(1);
            AssertionAction.CheckValue("t1");
            AssertionAction.CheckValue("t2", 4);
            AssertionAction.CheckValue("t3");
            AssertionAction.CheckValue("t4");
        }

        [TestMethod]
        public void alreadySetTest()
        {
            Game game = Setup.StartGame("Core/alreadySet.kgl");
            AssertionAction.CheckValue("changes", 2);
        }
    }
}
