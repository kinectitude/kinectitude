using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class ClassFactoryTests
    {
        private static Game game = new Game(new GameLoaderMock(), 1, 1, new Func<Tuple<int, int>>(() => new Tuple<int, int>(0, 0)));
        private static Scene scene = new Scene(new SceneLoaderMock(new GameLoaderMock(), new LoaderUtilityMock()), game);
        private static EventMock evt = new EventMock();
        private static ActionMock action = new ActionMock();

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
            Assert.AreEqual(100, component.I);
            ClassFactory.SetParam(component, "D", "10.1", null, null);
            Assert.AreEqual(10.1, component.D);
            ClassFactory.SetParam(component, "F", "11.2", null, null);
            Assert.AreEqual(11.2f, component.F);
            ClassFactory.SetParam(component, "B", "True", null, null);
            Assert.IsTrue(component.B);
        }

        [TestMethod]
        public void SetExpressionTest()
        {
            Entity e = new Entity(10);
            ClassFactory.SetParam(evt, "Expression", "this", evt, e);
            Assert.AreEqual("this", evt.Expression.GetValue());
            ClassFactory.SetParam(action, "Expression", "{" + TypeMatcher.ParentChar +"Expression}", evt, e);
            Assert.AreEqual(evt.Expression.GetValue(), action.Expression.GetValue());
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
            ClassFactory.SetParam(action, "TypeMatcher", "@TypeMatcher", evt, entity);
            Assert.IsTrue(evt.TypeMatcher == action.TypeMatcher);
            ClassFactory.SetParam(action, "Expression", "{@TypeMatcher.x}", evt, entity);
            Assert.AreEqual("lol", action.Expression.GetValue());
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
            Game game = new Game(new GameLoaderMock(), 1, 1, new Func<Tuple<int, int>>(() => new Tuple<int, int>(0, 0)));
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
