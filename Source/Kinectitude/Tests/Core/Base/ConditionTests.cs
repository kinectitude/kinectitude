using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using Kinectitude.Core.Data;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class ConditionTests
    {

        Scene scene;
        Entity one = new Entity(0);

        public ConditionTests()
        {
            SceneLoaderMock slm = new SceneLoaderMock(new GameLoaderMock());
            one["true"] = "true";
            one["one"] = "one";
            one["false"] = "false";
            slm.EntityByName["one"] = one;
            scene = new Scene(slm, new Game(new GameLoaderMock()));
            one.Scene = scene;
        }

        [TestMethod]
        public void BasicConditions()
        {
            BoolExpressionReader bre = new BoolExpressionReader("true", null, null);
            Condition condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("True", null, null);
            condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("false", null, null);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());

            bre = new BoolExpressionReader("False", null, null);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());
        }

        [TestMethod]
        public void BasicOtherConditions()
        {
            BoolExpressionReader bre = new BoolExpressionReader("one.notSet", null, one);
            Condition condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());

            bre = new BoolExpressionReader("this.notSet", null, one);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());

            bre = new BoolExpressionReader("one.true", null, one);
            condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("this.true", null, one);
            condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());
        }

        [TestMethod]
        public void BasicAnd()
        {

            BoolExpressionReader bre = new BoolExpressionReader("true and true", null, null);
            Condition condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("true and True", null, null);
            condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("true and this.notSet", null, one);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());
        }

        [TestMethod]
        public void BasicOr()
        {
            BoolExpressionReader bre = new BoolExpressionReader("true or true", null, null);
            Condition condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("true or True", null, null);
            condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("false or this.notSet", null, one);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());
        }

        [TestMethod]
        public void OrderOfOperations()
        {
            BoolExpressionReader bre = new BoolExpressionReader("True or False and False", null, null);
            Condition condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("True and False or True", null, null);
            condition = new Condition(bre);
            Assert.IsTrue(condition.ShouldRun());

            bre = new BoolExpressionReader("True and False or False", null, one);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());

            bre = new BoolExpressionReader("(True or False) and False", null, null);
            condition = new Condition(bre);
            Assert.IsFalse(condition.ShouldRun());
        }
    }
}
