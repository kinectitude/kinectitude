//-----------------------------------------------------------------------
// <copyright file="ServicesTest.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class ServicesTest
    {
        static ServicesTest()
        {
            ServiceAuto.started = ServiceAuto.stopped = ServiceNoAuto.started = ServiceNoAuto.stopped =
                ServiceAutoSelfStop.started = ServiceAutoSelfStop.stopped = ServiceAuto.setVal =
                ServiceAutoSelfStop.setVal = ServiceNoAuto.setVal = 0;
            Setup.RunGame("Core/ServiceTests.kgl");
        }
        
        [TestMethod]
        public void ServiceStartingStopping()
        {
            Assert.AreEqual<int>(1, ServiceAuto.started);
            Assert.AreEqual<int>(1, ServiceAuto.stopped);
            Assert.AreEqual<int>(1, ServiceAutoSelfStop.started);
            Assert.AreEqual<int>(1, ServiceAutoSelfStop.stopped);
            Assert.AreEqual<int>(0, ServiceNoAuto.started);
            Assert.AreEqual<int>(0, ServiceNoAuto.stopped);
        }

        [TestMethod]
        public void ServiceValueSet()
        {
            AssertionAction.CheckValue("service value");
        }
    }
}
