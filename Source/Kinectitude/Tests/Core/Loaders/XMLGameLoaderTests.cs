using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Loaders;
using System.Xml.Linq;

namespace Kinectitude.Tests.Core.Loaders
{
    [TestClass]
    public class XMLGameLoaderTests
    {

        private const string sampleFile = "sample.xml";

        private static readonly XMLGameLoader xmlGameLoader = new XMLGameLoader(sampleFile);

        [TestMethod]
        public void TestMethod1()
        {
            //first prototype
            XElement prototype = xmlGameLoader.Prototypes["prototype1"];
            Assert.IsTrue("100" == (string)prototype.Attribute("score"));
            Assert.IsTrue(1 == prototype.Elements().Where(input => XMLGameLoader.EventName == input.Name).Count());
            Assert.IsTrue(1 == prototype.Elements().Where(input => XMLGameLoader.ComponentName == input.Name).Count());
            Assert.IsTrue("1000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property1"));
            Assert.IsTrue("2000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property2"));

            //second prototype
            prototype = xmlGameLoader.Prototypes["prototype2"];
            Assert.IsTrue("50" == (string)prototype.Attribute("score"));
            Assert.IsTrue(2 == prototype.Elements().Where(input => XMLGameLoader.EventName == input.Name).Count());
            Assert.IsTrue(1 == prototype.Elements().Where(input => XMLGameLoader.ComponentName == input.Name).Count());
            Assert.IsTrue("1000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property1"));
            Assert.IsTrue("5000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property2"));
            Assert.IsTrue("5900" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property3"));

            //third prototype
            prototype = xmlGameLoader.Prototypes["prototype3"];
            Assert.IsTrue("50" == (string)prototype.Attribute("score"));
            Assert.IsTrue(2 == prototype.Elements().Where(input => XMLGameLoader.EventName == input.Name).Count());
            Assert.IsTrue(1 == prototype.Elements().Where(input => XMLGameLoader.ComponentName == input.Name).Count());
            Assert.IsTrue("1000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property1"));
            Assert.IsTrue("5000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property2"));
            Assert.IsTrue("10" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property3"));


            //fourth prototype
            prototype = xmlGameLoader.Prototypes["prototype4"];
            Assert.IsTrue("100" == (string)prototype.Attribute("score"));
            Assert.IsTrue(3 == prototype.Elements().Where(input => XMLGameLoader.EventName == input.Name).Count());
            Assert.IsTrue(1 == prototype.Elements().Where(input => XMLGameLoader.ComponentName == input.Name).Count());
            Assert.IsTrue("1000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property1"));
            Assert.IsTrue("2000" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property2"));
            Assert.IsTrue("5900" == (string)prototype.Element(XMLGameLoader.ComponentName).Attribute("Property3"));
        }
    }
}
