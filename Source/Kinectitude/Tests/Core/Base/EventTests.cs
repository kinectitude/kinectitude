using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Tests.Core.Base
{

    public class TestEvent : Event
    {
        public bool hasInit = false;
        public override void OnInitialize() { hasInit = true; }
    }

    [TestClass]
    public class EventTests
    {

        public List<TestAction> actionList = new List<TestAction>();

        [TestMethod]
        public void TestAllActionsExecuted()
        {
            Event evt = new TestEvent();
            for (int i = 0; i < 10; i++)
            {
                TestAction action = new TestAction();
                actionList.Add(action);
                evt.AddAction(action);
            }

            evt.DoActions();

            foreach (TestAction action in actionList)
            {
                Assert.IsTrue(action.hasRun);
            }
        }

        [TestMethod]
        public void TestOnInitialize()
        {
            TestEvent test = new TestEvent();
            test.Initialize();
            Assert.IsTrue(test.hasInit);
        }
    }
}
