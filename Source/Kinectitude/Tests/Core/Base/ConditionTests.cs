using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Tests.Core.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class ConditionTests
    {

        static Scene scene;
        static Entity one = new Entity(0);

        static ConditionTests()
        {
            SceneLoaderMock slm = new SceneLoaderMock(new GameLoaderMock(), new LoaderUtilityMock());
            one["true"] = "true";
            one["one"] = "one";
            one["false"] = "false";
            scene = new Scene(slm, new Game(new GameLoaderMock()));
            scene.EntityByName["one"] = one;
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
