using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Tests.Core.Loaders
{
    [TestClass]
    public class XMLGameLoaaderTests
    {

        private readonly string sampleFile = "sample.xml";

        [TestMethod]
        public void TestPrototypeIs()
        {
            XMLGameLoader xmlGameLoader = new XMLGameLoader("sample.xml");
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype1"].Count == 1);
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype1"][0] == "prototype1");
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype2"].Count == 2);
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype2"].Contains("prototype2"));
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype2"].Contains("prototype1"));
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype3"].Count == 3);
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype2"].Contains("prototype3"));
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype2"].Contains("prototype2"));
            Assert.IsTrue(xmlGameLoader.PrototypeIs["prototype2"].Contains("prototype1"));
        }
    }
}
