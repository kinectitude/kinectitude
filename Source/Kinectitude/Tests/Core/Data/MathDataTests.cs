using System;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core.Data
{
    [TestClass]
    public class MathDataTests
    {
        Game game = new Game(new GameLoaderMock());
        Scene scene;
        Entity entity = new Entity(0);
        ComponentMock component = new ComponentMock();
        Event evt = new EventMock();
        ManagerMock manager = new ManagerMock();

        static MathDataTests()
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
                ClassFactory.RegisterType("manager", typeof(ManagerMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
        }

        public MathDataTests()
        {
            scene = new Scene(new SceneLoaderMock(new GameLoaderMock(), new LoaderUtilityMock()), game);
            entity["one"] = "1";
            entity["onePOne"] = "1.1";
            entity["two"] = "2";
            entity["true"] = "true";
            entity["lol"] = "lol";
            entity["fales"] = "false";
            entity.Scene = scene;
            try
            {
                ClassFactory.RegisterType("component", typeof(ComponentMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
            entity.AddComponent(component, "component");
            evt.Entity = entity;
        }

        [TestMethod]
        public void BasicMath()
        {
            string expr = "this.one + this.one";
            DoubleExpressionReader der = new DoubleExpressionReader(expr, null, entity);
            IntExpressionReader ier = new IntExpressionReader(expr, null, entity);
            Assert.IsTrue(der.GetValue() == 2);
            Assert.IsTrue(ier.GetValue() == 2);

            expr = "this.onePOne + this.onePOne";
            der = new DoubleExpressionReader(expr, null, entity);
            ier = new IntExpressionReader(expr, null, entity);
            Assert.IsTrue(der.GetValue() == 2.2);
            Assert.IsTrue(ier.GetValue() == 2);

            expr = "this.two + this.onePOne * this.two";
            der = new DoubleExpressionReader(expr, null, entity);
            ier = new IntExpressionReader(expr, null, entity);
            Assert.IsTrue(der.GetValue() == 4.2);
            Assert.IsTrue(ier.GetValue() == 4);
        }

        [TestMethod]
        public void BoolMath()
        {
            string expr = "this.true + this.false";
            DoubleExpressionReader der = new DoubleExpressionReader(expr, null, entity);
            IntExpressionReader ier = new IntExpressionReader(expr, null, entity);
            Assert.IsTrue(der.GetValue() == 1);
            Assert.IsTrue(ier.GetValue() == 1);
        }

        [TestMethod]
        public void InvalidMath()
        {
            string expr = "this.lol + this.lol";
            DoubleExpressionReader der = new DoubleExpressionReader(expr, null, entity);
            IntExpressionReader ier = new IntExpressionReader(expr, null, entity);
            Assert.IsTrue(der.GetValue() == 0);
            Assert.IsTrue(ier.GetValue() == 0);
        }
    }
}
