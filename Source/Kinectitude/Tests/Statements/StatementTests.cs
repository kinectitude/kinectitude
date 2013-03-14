using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Tests.Core;

namespace Kinectitude.Tests.Statements
{
    [TestClass]
    public class StatementTests
    {
        private static void TestOne(string name)
        {
            Setup.StartGame("Statements/Scripts/" + name + ".kgl");
            AssertionAction.CheckValue(name, 1);
        }

        [TestMethod]
        public void ForLoop()
        {
            TestOne("ForLoop");
        }

        [TestMethod]
        public void NestedForLoop()
        {
            TestOne("NestedForLoop");
        }

        [TestMethod]
        public void OrCondition()
        {
            TestOne("OrCondition");

            int a = 1;

            Assert.IsTrue(a == 1 || a == 2);
        }
    }
}
