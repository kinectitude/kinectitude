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
            result = tabs() + action.Type + ApplyToProperties(action.Properties);
        }

        public void Visit(Assignment assignment)
        {
            StringBuilder sb = new StringBuilder(tabs()).Append(assignment.Key).Append(" ");

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
            result = new StringBuilder(tabs()).Append("Component ")
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
            string tabStr = tabs();
            StringBuilder stmt = new StringBuilder(tabStr).Append("if (").Append(group.If.Expression).Append(')')
                .Append(Apply(group.If));

            foreach (var elseif in group.Statements)
            {
                stmt.Append(tabStr).Append("else if (").Append(group.If.Expression).Append(')');
                stmt.Append(Apply(elseif));
            }
            if (group.Else != null)
            {
                stmt.Append(tabStr).Append("else");
                stmt.Append(Apply(group.Else));
            }
            result = stmt.ToString();
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
            StringBuilder entityBuilder = new StringBuilder(tabstr)
                .Append(entity.IsPrototype? "Prototype " :  "Entity ").Append(entity.Name ?? "");

            if(entity.Prototypes.Count != 0){
                entityBuilder.Append(" : ").Append(string.Join(", ", entity.Prototypes.Select(p => p.Name)));
            }

            entityBuilder.Append(ApplyToAttributes(entity.Attributes)).Append(openDef());

            if(entity.Components.Count != 0) entityBuilder.Append(ApplyToAll<Component>(entity.Components, "\n", ValidComponent)).Append('\n');
            if (entity.Events.Count != 0) entityBuilder.Append(ApplyToAll<AbstractEvent>(entity.Events, "\n", ValidEvent)).Append('\n');

            result = entityBuilder.Append(closeDef()).ToString();
        }

        public void Visit(Event evt)
        {
            result = new StringBuilder("            Event ").Append(evt.Type)
                .Append(ApplyToProperties(evt.Properties)).Append(openDef())
                .Append(ApplyToAll<AbstractStatement>(evt.Statements, "\n", AllValid)).Append("            ").Append(closeDef()).ToString();
        }

        public void Visit(ForLoop loop)
        {
            string tabin = tabs();
            StringBuilder sb = new StringBuilder(tabin).Append("for (").Append(loop.PreExpression).Append("; ")
                .Append(loop.Expression).Append("; ").Append(loop.PostExpression).Append(")").Append(openDef());

            sb.Append(ApplyToAll<AbstractStatement>(loop.Statements, "\n", AllValid));

            sb.Append("\n").Append(tabin).Append(closeDef());
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

            result = new StringBuilder(ApplyToAll<Using>(game.Usings, "", AllValid))
                .Append("Game").Append(ApplyToAttributes(pairs))
                .Append(openDef()).Append(ApplyToAll<Entity>(game.Prototypes, "", AllValid))
                .Append(ApplyToAll<Service>(game.Services, "", AllValid))
                .Append(ApplyToAll<Scene>(game.Scenes, "", AllValid)).Append(closeDef()).ToString();
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
            StringBuilder sceneBuilder = new StringBuilder("    Scene ").Append(scene.Name).Append(ApplyToAttributes(scene.Attributes)).Append(openDef());

            if (scene.Managers.Count != 0)
                sceneBuilder.Append(ApplyToAll<Manager>(scene.Managers, "\n", AllValid)).Append('\n');

            sceneBuilder.Append(ApplyToAll<Entity>(scene.Entities, "", AllValid)).Append(closeDef());
            result = sceneBuilder.ToString();
        }

        public void Visit(Service service)
        {
            result = new StringBuilder("        Service ")
                .Append(service.Type).Append(ApplyToProperties(service.Properties)).ToString();
        }

        public void Visit(Using use)
        {
            if (use.File == null) return;
            result = new StringBuilder("using ").Append(use.File).Append(openDef())
                .Append(ApplyToAll<Define>(use.Defines, "\n", AllValid)).Append(closeDef()).Append("\n").ToString();
        }

        public void Visit(Value val)
        {
            result = val.Initializer;
        }

        public void Visit(WhileLoop loop)
        {
            StringBuilder sb = new StringBuilder(tabs()).Append("while (").Append(loop.Expression).Append(")")
                .Append(openDef()).Append(ApplyToAll<AbstractStatement>(loop.Statements, "\n", AllValid)).Append(closeDef());

            result = sb.ToString();
        }

        private void VisitCondition(AbstractCondition condition)
        {
            StringBuilder conditionBuilder = new StringBuilder(openDef());
            conditionBuilder.Append(ApplyToAll<AbstractStatement>(condition.Statements, "\n", AllValid));
            conditionBuilder.Append(closeDef());
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
        private string tabs()
        {
            StringBuilder tabBuilder = new StringBuilder();
            for (int i = 0; i < numTabs; i++) tabBuilder.Append("    ");
            return tabBuilder.ToString();
        }

        private string openDef()
        {
            string ret = "\n" + tabs() + "{\n";
            numTabs++;
            return ret;
        }

        private string closeDef()
        {
            numTabs--;
            return "\n" + tabs() + "}\n" ;
        }

    }
}
