using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class RequiresProvides
    {
        [TestMethod]
        public void ValidProvidesAndRequires()
        {
            Setup.StartGame("Core/validProvidesRequires.kgl");
        }

        [TestMethod]
        public void InvalidRequires()
        {
            List<String> dieMsgs = new List<string>();
            Setup.StartGame("Core/invalidRequires.kgl", new Action<string>(s => dieMsgs.Add(s)));
            Assert.AreEqual<int>(1, dieMsgs.Count);
            Assert.AreEqual<string>("An unnamed entity is missing required component(s): Kinectitude.Core.Components.TransformComponent", dieMsgs[0]);
        }

        [TestMethod]
        public void InvalidProvides()
        {
            List<String> dieMsgs = new List<string>();
            Setup.StartGame("Core/invalidProvides.kgl", new Action<string>(s => dieMsgs.Add(s)));
            Assert.AreEqual<int>(1, dieMsgs.Count);
            Assert.AreEqual<string>("InvalidProvides can't provide Kinectitude.Core.Components.TransformComponent", dieMsgs[0]);
        }
    }
}
