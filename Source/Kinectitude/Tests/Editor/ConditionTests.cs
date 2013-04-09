//-----------------------------------------------------------------------
// <copyright file="ConditionTests.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Tests
{
    [TestClass]
    public class ConditionTests
    {
        private static readonly string TriggerOccursEventType = typeof(Kinectitude.Core.Events.TriggerOccursEvent).FullName;
        private static readonly string FireTriggerActionType = typeof(Kinectitude.Core.Actions.FireTriggerAction).FullName;

        [TestMethod]
        public void AddConditionGroup()
        {
            bool collectionChanged = false;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.Statements.CollectionChanged += (o, e) => collectionChanged = true;

            ConditionGroup group = new ConditionGroup();
            evt.AddStatement(group);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, evt.Statements.Count);
            Assert.IsNotNull(group.If);
            Assert.IsNull(group.Else);
        }

        [TestMethod]
        public void RemoveConditionGroup()
        {
            int eventsFired = 0;

            Event evt = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            evt.Statements.CollectionChanged += (o, e) => eventsFired++;

            ConditionGroup group = new ConditionGroup();
            evt.AddStatement(group);
            evt.RemoveStatement(group);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, evt.Statements.Count);
        }

        [TestMethod]
        public void AddElseIfCondition()
        {
            bool collectionChanged = false;

            ConditionGroup group = new ConditionGroup();
            group.Statements.CollectionChanged += (o, e) => collectionChanged = true;

            ExpressionCondition condition = new ExpressionCondition() { Expression = "test > 1" };
            group.AddStatement(condition);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual("test > 1", condition.Expression);
            Assert.AreEqual(1, group.Statements.Count);
        }

        [TestMethod]
        public void RemoveElseIfCondition()
        {
            int eventsFired = 0;

            ConditionGroup group = new ConditionGroup();
            group.Statements.CollectionChanged += (o, e) => eventsFired++;

            ExpressionCondition condition = new ExpressionCondition() { Expression = "test > 1" };
            group.AddStatement(condition);
            group.RemoveStatement(condition);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, group.Statements.Count);
        }

        [TestMethod]
        public void AddAction()
        {
            bool collectionChanged = false;

            ExpressionCondition condition = new ExpressionCondition() { Expression = "test > 1" };
            condition.Statements.CollectionChanged += (o, e) => collectionChanged = true;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddStatement(action);

            Assert.IsTrue(collectionChanged);
            Assert.AreEqual(1, condition.Statements.Count);
        }

        [TestMethod]
        public void RemoveAction()
        {
            int eventsFired = 0;

            ExpressionCondition condition = new ExpressionCondition() { Expression = "test > 1" };
            condition.Statements.CollectionChanged += (o, e) => eventsFired++;

            Action action = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            condition.AddStatement(action);
            condition.RemoveStatement(action);

            Assert.AreEqual(2, eventsFired);
            Assert.AreEqual(0, condition.Statements.Count);
        }

        [TestMethod]
        public void AddReadOnlyConditionGroup()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);

            parentEvent.AddStatement(new ConditionGroup());

            Assert.AreEqual(1, childEvent.Statements.Count);

            ReadOnlyConditionGroup childGroup = childEvent.Statements.OfType<ReadOnlyConditionGroup>().Single();

            Assert.IsNotNull(childGroup);
        }

        [TestMethod]
        public void AddReadOnlyElseIfCondition()
        {
            Event parentEvent = new Event(Workspace.Instance.GetPlugin(TriggerOccursEventType));
            ReadOnlyEvent childEvent = new ReadOnlyEvent(parentEvent);

            ConditionGroup parentGroup = new ConditionGroup();
            parentEvent.AddStatement(parentGroup);

            parentGroup.AddStatement(new ExpressionCondition() { Expression = "test > 1" });

            Assert.AreEqual(1, childEvent.Statements.Count);

            ReadOnlyConditionGroup childGroup = childEvent.Statements.OfType<ReadOnlyConditionGroup>().Single();

            Assert.IsNotNull(childGroup);

            ReadOnlyExpressionCondition childCondition = childGroup.Statements.OfType<ReadOnlyExpressionCondition>().Single();

            Assert.AreEqual("test > 1", childCondition.Expression);
        }

        [TestMethod]
        public void AddReadOnlyAction()
        {
            ExpressionCondition parentCondition = new ExpressionCondition() { Expression = "test > 1" };
            ReadOnlyExpressionCondition childCondition = new ReadOnlyExpressionCondition(parentCondition);

            parentCondition.AddStatement(new Action(Workspace.Instance.GetPlugin(FireTriggerActionType)));

            Assert.AreEqual(1, childCondition.Statements.Count);
        }

        [TestMethod]
        public void RemoveReadOnlyAction()
        {
            ExpressionCondition parentCondition = new ExpressionCondition() { Expression = "test > 1" };
            ReadOnlyExpressionCondition childCondition = new ReadOnlyExpressionCondition(parentCondition);

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddStatement(parentAction);
            parentCondition.RemoveStatement(parentAction);

            Assert.AreEqual(0, childCondition.Statements.Count);
        }

        [TestMethod]
        public void CannotRemoveInheritedActionFromInheritingCondition()
        {
            ExpressionCondition parentCondition = new ExpressionCondition() { Expression = "test > 1" };
            ReadOnlyExpressionCondition childCondition = new ReadOnlyExpressionCondition(parentCondition);

            Action parentAction = new Action(Workspace.Instance.GetPlugin(FireTriggerActionType));
            parentCondition.AddStatement(parentAction);

            AbstractStatement childAction = childCondition.Statements.Single();
            childCondition.RemoveStatement(childAction);

            Assert.AreEqual(1, childCondition.Statements.Count);
        }

        [TestMethod]
        public void ReadOnlyConditionFollowsRuleChange()
        {
            bool propertyChanged = false;

            ExpressionCondition parentCondition = new ExpressionCondition() { Expression = "test > 1" };
            
            ReadOnlyExpressionCondition childCondition = new ReadOnlyExpressionCondition(parentCondition);
            childCondition.PropertyChanged += (o, e) => propertyChanged = (e.PropertyName == "Expression");

            Assert.AreEqual("test > 1", childCondition.Expression);

            parentCondition.Expression = "test <= 1";

            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("test <= 1", childCondition.Expression);
        }
    }
}
