using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Action = Kinectitude.Core.Base.Action;
using Kinectitude.Tests.Core.TestMocks;

namespace Kinectitude.Tests.Core.Base
{

    [TestClass]
    public class EventTests
    {

        public List<ActionMock> actionList = new List<ActionMock>();

        [TestMethod]
        public void TestAllActionsExecuted()
        {
            Event evt = new EventMock();
            for (int i = 0; i < 10; i++)
            {
                ActionMock action = new ActionMock();
                actionList.Add(action);
                evt.AddAction(action);
            }

            evt.DoActions();

            foreach (ActionMock action in actionList)
            {
                Assert.IsTrue(action.hasRun);
            }
        }

        [TestMethod]
        public void TestOnInitialize()
        {
            EventMock test = new EventMock();
            test.Initialize();
            Assert.IsTrue(test.hasInit);
        }
    }
}
