using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class ManagerTest
    {
        const string managerSetting = "core/managerSetting.kgl";
        const string managerNotDeclared = "core/managerNoDeclare.kgl";

        [TestMethod]
        [DeploymentItem(managerSetting)]
        public void SetManagerProperty()
        {
            Game game = Setup.StartGame("managerSetting.kgl");
            game.OnUpdate(1 / 60);
            AssertionAction.CheckValue("MockMan.boolean");
        }

        [TestMethod]
        [DeploymentItem(managerNotDeclared)]
        public void ManagerNoNameProperty()
        {
            MockManager.Added = 0;
            Game game = Setup.StartGame("managerNoDeclare.kgl");
            game.OnUpdate(1 / 60);
            AssertionAction.CheckValue("no crash");
            Assert.AreEqual(MockManager.Added, 1);
        }
    }
}
