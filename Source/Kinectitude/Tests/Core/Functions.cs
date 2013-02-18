using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Functions
    {   
        [TestMethod]
        [DeploymentItem(@"Core\functions.kgl")]
        public void FunctionTest()
        {
            Setup.StartGame("functions.kgl");
            AssertionAction.CheckValue("1");
            AssertionAction.CheckValue("params 0", 2);
            AssertionAction.CheckValue("3");
            AssertionAction.CheckValue("params 5");
            AssertionAction.CheckValue("min");
            AssertionAction.CheckValue("max");
            AssertionAction.CheckValue("bool");
            AssertionAction.CheckValue("number");
            AssertionAction.CheckValue("str");
            AssertionAction.CheckValue("ln");
            AssertionAction.CheckValue("log", 2);
            AssertionAction.CheckValue("absolute");
            AssertionAction.CheckValue("random", 3);
        }        
    }
}
