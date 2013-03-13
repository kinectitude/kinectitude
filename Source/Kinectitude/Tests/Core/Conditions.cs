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
