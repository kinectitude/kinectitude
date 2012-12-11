using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Defaults
    {
        [TestMethod]
        [DeploymentItem("core/defaults.kgl")]
        public void DefaultTest()
        {
            Setup.StartGame("defaults.kgl");
            AssertionAction.CheckValue("Int");
            AssertionAction.CheckValue("Double");
            AssertionAction.CheckValue("Str");
            AssertionAction.CheckValue("Enum");
            AssertionAction.CheckValue("Bool");
            AssertionAction.CheckValue("Reader");
        }
    }
}
