using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using System.Moles;
using Kinectitude.Tests.Core.TestMocks;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class ClassFactoryTests
    {

        private Scene scene = new Scene(new SceneLoaderMock(new GameLoaderMock()), new Game(new GameLoaderMock()));
        private EventMock evt = new EventMock();
        private ActionMock action = new ActionMock();


        static ClassFactoryTests()
        {
            ClassFactory.RegisterType("component", typeof(ComponentMock));
            ClassFactory.RegisterType("evt", typeof(EventMock));
            ClassFactory.RegisterType("act", typeof(ActionMock));
        }

        public ClassFactoryTests()
        {
            evt.AddAction(action);
        }

        [TestMethod]
        public void CreateTypeTest()
        {
            Component component = ClassFactory.Create<Component>("component");
            Assert.IsNotNull(component as ComponentMock);
        }

        [TestMethod]
        public void SetBasicParamsTest()
        {
            ComponentMock component = ClassFactory.Create<ComponentMock>("component");
            ClassFactory.SetParam(component, "I", "100", null, null);
            Assert.IsTrue(component.I == 100);
            ClassFactory.SetParam(component, "D", "10.1", null, null);
            Assert.IsTrue(component.D == 10.1);
            ClassFactory.SetParam(component, "F", "11.2", null, null);
            Assert.IsTrue(component.F == 11.2f);
            ClassFactory.SetParam(component, "B", "True", null, null);
            Assert.IsTrue(component.B);
        }

        [TestMethod]
        public void SetExpressionTest()
        {
            ClassFactory.SetParam(evt, "Expression", "this", evt, null);
            Assert.IsTrue(evt.Expression.GetValue() == "this");
            ClassFactory.SetParam(action, "Expression", "{!Expression}", evt, null);
            Assert.IsTrue(evt.Expression.GetValue() == action.Expression.GetValue());
        }

        [TestMethod]
        public void SetTypeExpressionTest()
        {
            

            scene.IsType = new Dictionary<string,HashSet<int>>()
            {
                { "ball", new HashSet<int>() { 0 } }
            };
            evt.AddAction(action);

            Entity entity = new Entity(0);
            entity.Scene = scene;
            entity["x"] = "lol";
            
            ClassFactory.SetParam(evt, "TypeMatcher", "$ball", evt, entity);
            Assert.IsTrue(evt.TypeMatcher.MatchAndSet(entity));
            ClassFactory.SetParam(action, "TypeMatcher", "!TypeMatcher", evt, entity);
            Assert.IsTrue(evt.TypeMatcher == action.TypeMatcher);
            ClassFactory.SetParam(action, "Expression", "{!TypeMatcher.x}", evt, entity);
            Assert.IsTrue(action.Expression.GetValue() == "lol");
        }

        [TestMethod]
        public void SetValueWriter()
        {
            Entity entity = new Entity(0);
            entity.Scene = scene;
            entity["x"] = "lol";
            ClassFactory.SetParam(action, "Writer", "this.health", evt, entity);
            action.Writer.Value = "100";
            Assert.IsTrue(entity["health"] == "100");
        }

    }
}
