using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Loops
    {
        [TestMethod]
        [DeploymentItem("core/loops.kgl")]
        public void LoopTests()
        {
            Setup.StartGame("loops.kgl");
            AssertionAction.CheckValue("basic for", 10);
            AssertionAction.CheckValue("for no first", 10);
            AssertionAction.CheckValue("while", 10);
            AssertionAction.CheckValue("while part in", 2);
        }
    }
}
