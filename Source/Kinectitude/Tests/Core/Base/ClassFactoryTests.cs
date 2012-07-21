using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Kinectitude.Core.Components;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class ClassFactoryTests
    {

        private Scene scene = new Scene(new SceneLoaderMock(new GameLoaderMock(), new LoaderUtilityMock()), new Game(new GameLoaderMock()));
        private EventMock evt = new EventMock();
        private ActionMock action = new ActionMock();

        static ClassFactoryTests()
        {
            try
            {
                ClassFactory.RegisterType("component", typeof(ComponentMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
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


            scene.IsType["ball"] = new HashSet<int>() { 0 };
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IllegalProvidesComponent()
        {
            ClassFactory.RegisterType("IllegalProvidesComponent", typeof(IllegalProvidesComponent));
        }

        [TestMethod]
        public void LoadServices()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Game game = new Game(new GameLoaderMock());
            ClassFactory.LoadServices(assembly);
            Assert.IsTrue(game.GetService<MockServiceToRun>().Started);
            Assert.IsFalse(game.GetService<MockServiceNotToRun>().Started);
        }

        [TestMethod]
        public void TestGoodProvide()
        {
            ClassFactory.RegisterType("GoodProvidesComponent", typeof(GoodProvidesComponent));
            Component component = ClassFactory.Create<Component>("GoodProvidesComponent");
            Assert.IsNotNull(component);
            Assert.IsTrue(ClassFactory.GetProvided(typeof(GoodProvidesComponent)).Contains(typeof(TransformComponent)));
        }
    }
}
