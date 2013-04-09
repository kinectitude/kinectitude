//-----------------------------------------------------------------------
// <copyright file="KglGameVisitor.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KglGameVisitor : IGameVisitor
    {
        private static readonly Predicate<AbstractProperty> ValidProperty = (property => property.HasOwnValue);
        private static readonly Predicate<GameModel> AllValid = (model => true);
        private static readonly Predicate<Component> ValidComponent = (component => component.IsRoot || component.HasOwnValues);
        private static readonly Predicate<AbstractEvent> ValidEvent = (evt => evt.IsEditable);
        private static readonly Predicate<Attribute> ValidAttribute = (attribute => attribute.HasOwnValue);

        private int numTabs = 0;
        private string result;

        public string Apply(GameModel model)
        {
            model.Accept(this);
            return result;
        }

        public void Visit(Action action)
        {
            result = Tabs() + action.Type + ApplyToProperties(action.Properties);
        }

        public void Visit(Assignment assignment)
        {
            StringBuilder sb = new StringBuilder(Tabs()).Append(assignment.Key).Append(" ");

            switch (assignment.Operator)
            {
                case AssignmentOperator.And:
                    sb.Append("&");
                    break;
                case AssignmentOperator.Decrement:
                    sb.Append("-");
                    break;
                case AssignmentOperator.Divide:
                    sb.Append("/");
                    break;
                case AssignmentOperator.Increment:
                    sb.Append("+");
                    break;
                case AssignmentOperator.Multiply:
                    sb.Append("*");
                    break;
                case AssignmentOperator.Or:
                    sb.Append("|");
                    break;
                case AssignmentOperator.Power:
                    sb.Append("^");
                    break;
                case AssignmentOperator.Remainder:
                    sb.Append("%");
                    break;
                case AssignmentOperator.ShiftLeft:
                    sb.Append("<<");
                    break;
                case AssignmentOperator.ShiftRight:
                    sb.Append(">>");
                    break;
            }
            sb.Append("= ").Append(assignment.Value);
            result = sb.ToString();
        }

        public void Visit(Attribute attribute)
        {
            VisitNameValue(attribute);
        }

        public void Visit(Component component)
        {
            //Three tabs, game, scene, entity then component
            result = new StringBuilder(Tabs()).Append("Component ")
                .Append(component.Type).Append(ApplyToProperties(component.Properties)).ToString();
        }

        public void Visit(BasicCondition condition)
        {
            VisitCondition(condition);
        }

        public void Visit(ExpressionCondition condition)
        {
            VisitCondition(condition);
        }

        public void Visit(ConditionGroup group)
        {
            string tabStr = Tabs();
            StringBuilder builder = new StringBuilder(tabStr).Append("if (")
                .Append(group.If.Expression).Append(')').Append(Apply(group.If));

            if (group.Statements.Count != 0 || group.Else != null)
            {
                builder.AppendLine();
            }

            int i = 0;
            foreach (var stmt in group.Statements)
            {
                var condition = (ExpressionCondition)stmt;
                builder.Append(tabStr).Append("else if (").Append(condition.Expression).Append(')');
                builder.Append(Apply(stmt));

                if (i < group.Statements.Count - 1 || group.Else != null)
                {
                    builder.AppendLine();
                }

                i++;
            }
            
            if (group.Else != null)
            {
                builder.Append(tabStr).Append("else");
                builder.Append(Apply(group.Else));
            }
            
            result = builder.ToString();
        }

        public void Visit(Define define)
        {
            //Tab from using
            result = "    define " + define.Name + " as " + define.Class;
        }

        public void Visit(Entity entity)
        {
            //tab from game and scene for entity and game for prototype.
            string tabstr = entity.IsPrototype ? "    " : "        ";
            StringBuilder entityBuilder = new StringBuilder(tabstr).Append(entity.IsPrototype ? "Prototype " : "Entity ");
            
            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                entityBuilder.Append(entity.Name);

                if (entity.Prototypes.Count != 0)
                {
                    entityBuilder.Append(' ');
                }
            }

            if(entity.Prototypes.Count != 0)
            {
                entityBuilder.Append(": ").Append(string.Join(", ", entity.Prototypes.Select(p => p.Name)));
            }

            entityBuilder.Append(ApplyToAttributes(entity.Attributes)).Append(OpenDef());

            if (Enumerable.Any(entity.Components, x => ValidComponent(x)))
            {
                entityBuilder.Append(ApplyToAll<Component>(entity.Components, Environment.NewLine, ValidComponent)).AppendLine();

                if (Enumerable.Any(entity.Events, x => ValidEvent(x)))
                {
                    entityBuilder.Append(Tabs()).AppendLine();
                }
            }

            if (Enumerable.Any(entity.Events, x => ValidEvent(x)))
            {
                entityBuilder.Append(ApplyToAll<AbstractEvent>(entity.Events, Environment.NewLine, ValidEvent));
            }

            result = entityBuilder.Append(CloseDefLine()).ToString();
        }

        public void Visit(Event evt)
        {
            result = new StringBuilder(Tabs()).Append("Event ").Append(evt.Type)
                .Append(ApplyToProperties(evt.Properties)).Append(OpenDef())
                .Append(ApplyToAll<AbstractStatement>(evt.Statements, Environment.NewLine, AllValid)).AppendLine().Append(CloseDefLine()).ToString();
        }

        public void Visit(ForLoop loop)
        {
            string tabin = Tabs();
            StringBuilder sb = new StringBuilder(tabin).Append("for (").Append(loop.PreExpression).Append("; ")
                .Append(loop.Expression).Append("; ").Append(loop.PostExpression).Append(")").Append(OpenDef());

            sb.Append(ApplyToAll<AbstractStatement>(loop.Statements, Environment.NewLine, AllValid));

            sb.AppendLine().Append(CloseDef());
            result = sb.ToString();
        }

        public void Visit(Game game)
        {
            var pairs = Enumerable.Concat(
                new[]
                {
                    new Attribute("Name") { Value = new Value(game.Name, true) },
                    new Attribute("FirstScene") { Value = new Value(game.FirstScene.Name, true) }
                },
                game.Attributes
            );

            var builder = new StringBuilder(ApplyToAll<Using>(game.Usings, "", AllValid))
                .Append("Game").Append(ApplyToAttributes(pairs)).Append(OpenDef());
            
            if (game.Prototypes.Count > 0)
            {
                builder.Append(ApplyToAll<Entity>(game.Prototypes, Tabs() + Environment.NewLine, AllValid)).Append(Tabs()).AppendLine();
            }
             
            if (game.Services.Count > 0)
            {
                builder.Append(ApplyToAll<Service>(game.Services, Tabs() + Environment.NewLine, AllValid)).AppendLine().Append(Tabs()).AppendLine();
            }

            builder.Append(ApplyToAll<Scene>(game.Scenes, Tabs() + Environment.NewLine, AllValid)).Append(CloseDef());
            result = builder.ToString();
        }

        public void Visit(Project project)
        {
            result = "";
        }

        public void Visit(ReadOnlyAction action)
        {
            result = "";
        }

        public void Visit(ReadOnlyAssignment assignment)
        {
            result = "";
        }

        public void Visit(ReadOnlyBasicCondition condition)
        {
            result = "";
        }

        public void Visit(ReadOnlyExpressionCondition condition)
        {
            result = "";
        }

        public void Visit(ReadOnlyConditionGroup group)
        {
            result = "";
        }

        public void Visit(ReadOnlyEvent evt)
        {
            result = "";
        }

        public void Visit(ReadOnlyForLoop loop)
        {
            result = "";
        }

        public void Visit(ReadOnlyProperty property)
        {
            result = "";
        }

        public void Visit(ReadOnlyWhileLoop loop)
        {
            result = "";
        }

        public void Visit(Manager manager)
        {
            //Tab from scene, game
            result = new StringBuilder("        Manager ")
                .Append(manager.Type).Append(ApplyToProperties(manager.Properties)).ToString();
        }

        public void Visit(Property property)
        {
            VisitNameValue(property);
        }

        public void Visit(Scene scene)
        {
            //tab from game
            StringBuilder sceneBuilder = new StringBuilder("    Scene ").Append(scene.Name).Append(ApplyToAttributes(scene.Attributes)).Append(OpenDef());

            if (scene.Managers.Count != 0)
            {
                sceneBuilder.Append(ApplyToAll<Manager>(scene.Managers, Tabs() + Environment.NewLine, AllValid)).Append(Environment.NewLine);

                if (scene.Entities.Count != 0)
                {
                    sceneBuilder.Append(Tabs()).AppendLine();
                }
            }

            sceneBuilder.Append(ApplyToAll<Entity>(scene.Entities, Tabs() + Environment.NewLine, AllValid)).Append(CloseDefLine());
            result = sceneBuilder.ToString();
        }

        public void Visit(Service service)
        {
            result = new StringBuilder("    Service ")
                .Append(service.Type).Append(ApplyToProperties(service.Properties)).ToString();
        }

        public void Visit(Using use)
        {
            if (use.File == null) return;
            result = new StringBuilder("using ").Append(use.File).Append(OpenDef())
                .Append(ApplyToAll<Define>(use.Defines, Environment.NewLine, AllValid)).AppendLine().Append(CloseDefLine()).AppendLine().ToString();
        }

        public void Visit(Value val)
        {
            result = val.Initializer;
        }

        public void Visit(WhileLoop loop)
        {
            StringBuilder sb = new StringBuilder(Tabs()).Append("while (").Append(loop.Expression).Append(")")
                .Append(OpenDef()).Append(ApplyToAll<AbstractStatement>(loop.Statements, Environment.NewLine, AllValid))
                .AppendLine().Append(CloseDef());

            result = sb.ToString();
        }

        private void VisitCondition(AbstractCondition condition)
        {
            StringBuilder conditionBuilder = new StringBuilder(OpenDef())
                .Append(ApplyToAll<AbstractStatement>(condition.Statements, Environment.NewLine, AllValid))
                .AppendLine().Append(CloseDef());

            result = conditionBuilder.ToString();
        }

        private void VisitNameValue(INameValue pair)
        {
            result = pair.Name + " = " + Apply(pair.Value);
        }

        private string ApplyToProperties(IEnumerable<AbstractProperty> properties)
        {
            return '(' + ApplyToAll<AbstractProperty>(properties, ", ", ValidProperty) + ')';
        }

        private string ApplyToAttributes(IEnumerable<Attribute> attributes)
        {
            return '(' + ApplyToAll<Attribute>(attributes, ", ", ValidAttribute) + ')';
        }

        private string ApplyToAll<T>(IEnumerable<T> members, string joinWith, Predicate<T> valid) where T : GameModel
        {
            return string.Join(joinWith, members.Where(m => valid(m)).Select(x => Apply(x)));
        }
        
        //Used for items where tabs can't be determined by what the item is.
        private string Tabs()
        {
            StringBuilder tabBuilder = new StringBuilder();
            for (int i = 0; i < numTabs; i++) tabBuilder.Append("    ");
            return tabBuilder.ToString();
        }

        private string OpenDef()
        {
            string ret = Environment.NewLine + Tabs() + "{" + Environment.NewLine;
            numTabs++;
            return ret;
        }

        private string CloseDefLine()
        {
            numTabs--;
            return Tabs() + "}" + Environment.NewLine;
        }

        private string CloseDef()
        {
            numTabs--;
            return Tabs() + "}";
        }
    }
}
