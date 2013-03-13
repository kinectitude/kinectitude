﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class ManagerTest
    {
        [TestMethod]
        public void SetManagerProperty()
        {
            Game game = Setup.StartGame("Core/managerSetting.kgl");
            game.OnUpdate(1 / 60);
            AssertionAction.CheckValue("MockMan.boolean");
        }

        [TestMethod]
        public void ManagerNoNameProperty()
        {
            MockManager.Added = 0;
            Game game = Setup.StartGame("Core/managerNoDeclare.kgl");
            game.OnUpdate(1 / 60);
            AssertionAction.CheckValue("no crash");
            Assert.AreEqual(MockManager.Added, 1);
        }
    }
}
