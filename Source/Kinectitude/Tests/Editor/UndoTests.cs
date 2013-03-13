using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Loops;
using Kinectitude.Core.Events;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Values;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class UndoTests
    {
        public UndoTests()
        {
            Workspace.Instance.DialogService = MockDialogService.Instance;
        }

        [TestMethod]
        public void Game_Name()
        {
            var game = new Game("Test Game");

            CommandHelper.TestUndoableCommand(
                () => Assert.AreEqual("Test Game", game.Name),
                () => game.Name = "Test Game 2",
                () => Assert.AreEqual("Test Game 2", game.Name)
            );
        }

        //[TestMethod]
        //public void Game_Width()
        //{
        //    var game = new Game("Test Game");

        //    CommandHelper.TestUndoableCommand(
        //        () => Assert.AreEqual(0, game.Width),
        //        () => game.Width = 800,
        //        () => Assert.AreEqual(800, game.Width)
        //    );
        //}

        //[TestMethod]
        //public void Game_Height()
        //{
        //    var game = new Game("Test Game");

        //    CommandHelper.TestUndoableCommand(
        //        () => Assert.AreEqual(0, game.Height),
        //        () => game.Height = 600,
        //        () => Assert.AreEqual(600, game.Height)
        //    );
        //}

        [TestMethod]
        public void Game_FirstScene()
        {
            var game = new Game("Test Game");
            var scene1 = new Scene("Scene1");
            game.FirstScene = scene1;
            var scene2 = new Scene("Scene2");

            CommandHelper.TestUndoableCommand(
                () => Assert.AreEqual(scene1, game.FirstScene),
                () => game.FirstScene = scene2,
                () => Assert.AreEqual(scene2, game.FirstScene)
            );
        }

        [TestMethod]
        public void Scene_Name()
        {
            var scene = new Scene("TestScene");

            CommandHelper.TestUndoableCommand(
                () => Assert.AreEqual("TestScene", scene.Name),
                () => scene.Name = "TestScene2",
                () => Assert.AreEqual("TestScene2", scene.Name)
            );
        }

        [TestMethod]
        public void Entity_Name()
        {
            var entity = new Entity();

            CommandHelper.TestUndoableCommand(
                () => Assert.IsNull(entity.Name),
                () => entity.Name = "testEntity",
                () => Assert.AreEqual("testEntity", entity.Name)
            );
        }

        [TestMethod]
        public void ExpressionCondition_Expression()
        {
            var group = new ConditionGroup();

            CommandHelper.TestUndoableCommand(
                () => Assert.IsNull(group.If.Expression),
                () => group.If.Expression = "true",
                () => Assert.AreEqual("true", group.If.Expression)
            );
        }

        [TestMethod]
        public void WhileLoop_Expression()
        {
            var loop = new WhileLoop();

            CommandHelper.TestUndoableCommand(
                () => Assert.IsNull(loop.Expression),
                () => loop.Expression = "true",
                () => Assert.AreEqual("true", loop.Expression)
            );
        }

        [TestMethod]
        public void ForLoop_PreExpression()
        {
            var loop = new ForLoop();

            CommandHelper.TestUndoableCommand(
                () => Assert.IsNull(loop.PreExpression),
                () => loop.PreExpression = "i = 0",
                () => Assert.AreEqual("i = 0", loop.PreExpression)
            );
        }

        [TestMethod]
        public void ForLoop_Expression()
        {
            var loop = new ForLoop();

            CommandHelper.TestUndoableCommand(
                () => Assert.IsNull(loop.Expression),
                () => loop.Expression = "i < 5",
                () => Assert.AreEqual("i < 5", loop.Expression)
            );
        }

        [TestMethod]
        public void ForLoop_PostExpression()
        {
            var loop = new ForLoop();

            CommandHelper.TestUndoableCommand(
                () => Assert.IsNull(loop.PostExpression),
                () => loop.PostExpression = "i += 1",
                () => Assert.AreEqual("i += 1", loop.PostExpression)
            );
        }

        [TestMethod]
        public void Property_Value()
        {
            var evt = new Event(Workspace.Instance.GetPlugin(typeof(TriggerOccursEvent).FullName));
            var property = evt.Properties.First();
            var val = new Value("5");

            CommandHelper.TestUndoableCommand(
                () => Assert.AreEqual(property.PluginProperty.DefaultValue, property.Value),
                () => property.Value = val,
                () => Assert.AreEqual(val, property.Value)
            );
        }

        [TestMethod]
        public void Attribute_Name()
        {
            var attribute = new Attribute("test");

            CommandHelper.TestUndoableCommand(
                () => Assert.AreEqual("test", attribute.Name),
                () => attribute.Name = "test2",
                () => Assert.AreEqual("test2", attribute.Name)
            );
        }

        [TestMethod]
        public void Attribute_Value()
        {
            var attribute = new Attribute("test");
            var val = new Value("5");

            CommandHelper.TestUndoableCommand(
                () => Assert.AreEqual(Attribute.DefaultValue, attribute.Value),
                () => attribute.Value = val,
                () => Assert.AreEqual(val, attribute.Value)
            );
        }
    }
}
