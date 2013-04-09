//-----------------------------------------------------------------------
// <copyright file="IGameVisitor.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using Kinectitude.Editor.Models.Values;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage
{
    internal interface IGameVisitor
    {
        void Visit(Action action);
        void Visit(Assignment assignment);
        void Visit(Attribute attribute);
        void Visit(BasicCondition condition);
        void Visit(Component component);
        void Visit(ConditionGroup group);
        void Visit(Define define);
        void Visit(Entity entity);
        void Visit(Event evt);
        void Visit(ExpressionCondition condition);
        void Visit(ForLoop loop);
        void Visit(Game game);
        void Visit(Project project);
        void Visit(ReadOnlyAction action);
        void Visit(ReadOnlyAssignment assignment);
        void Visit(ReadOnlyBasicCondition condition);
        void Visit(ReadOnlyConditionGroup group);
        void Visit(ReadOnlyEvent evt);
        void Visit(ReadOnlyExpressionCondition condition);
        void Visit(ReadOnlyForLoop loop);
        void Visit(ReadOnlyProperty property);
        void Visit(ReadOnlyWhileLoop loop);
        void Visit(Manager manager);
        void Visit(Property property);
        void Visit(Scene scene);
        void Visit(Service service);
        void Visit(Using use);
        void Visit(Value val);
        void Visit(WhileLoop loop);
    }
}
