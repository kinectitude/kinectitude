using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class RequiresProvides
    {
        [TestMethod]
        [DeploymentItem("core/validProvidesRequires.kgl")]
        public void ValidProvidesAndRequires()
        {
            Setup.StartGame("validProvidesRequires.kgl");
        }

        [TestMethod]
        [DeploymentItem("core/invalidRequires.kgl")]
        public void InvalidRequires()
        {
            List<String> dieMsgs = new List<string>();
            Setup.StartGame("invalidRequires.kgl", new Action<string>(s => dieMsgs.Add(s)));
            Assert.AreEqual<int>(1, dieMsgs.Count);
            Assert.AreEqual<string>("An unnamed entity is missing required component(s): Kinectitude.Core.Components.TransformComponent", dieMsgs[0]);
        }

        [TestMethod]
        [DeploymentItem("core/invalidProvides.kgl")]
        public void InvalidProvides()
        {
            List<String> dieMsgs = new List<string>();
            Setup.StartGame("invalidProvides.kgl", new Action<string>(s => dieMsgs.Add(s)));
            Assert.AreEqual<int>(1, dieMsgs.Count);
            Assert.AreEqual<string>("InvalidProvides can't provide Kinectitude.Core.Components.TransformComponent", dieMsgs[0]);
        }
    }
}
