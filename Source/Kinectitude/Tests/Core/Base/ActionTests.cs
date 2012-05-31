using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Kinectitude.Core.Base.Action;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core.Base
{

    public class TestAction : Action
    {
        public bool hasRun { get; private set; }
        public override void Run() { hasRun = true; }
    }

    [TestClass]
    public class ActionTests
    {
        [TestMethod]
        public void TestAction()
        {
            
        }
    }
}
