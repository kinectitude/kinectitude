using System.Collections.Generic;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kinectitude.Tests.Core.Base
{

    [TestClass]
    public class EventTests
    {

        public List<ActionMock> actionList = new List<ActionMock>();

        static EventTests()
        {

            try
            {
                ClassFactory.RegisterType("event", typeof(EventMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }

            try
            {
                ClassFactory.RegisterType("action", typeof(ActionMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
        }

        [TestMethod]
        public void TestAllActionsExecuted()
        {
            Entity entity = new Entity(0);
            Event evt = new EventMock();
            evt.Entity = entity;
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
            Entity entity = new Entity(0);
            EventMock test = new EventMock();
            test.Entity = entity;
            test.Initialize();
            Assert.IsTrue(test.hasInit);
        }
    }
}
