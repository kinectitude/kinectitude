using Kinectitude.Editor.Models;
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
        
        private static readonly Func<AbstractProperty, bool> validProperties = (property => property.HasOwnValue);
        private static readonly Func<GameModel, bool> allValid = (model => true);
        private static readonly Func<Component, bool> validComponent = (component => component.IsRoot || component.HasOwnValues);
        private static readonly Func<AbstractEvent, bool> validEvt = (evt => evt.IsLocal);

        private int numTabs = 0;
        private string result;

        public string Apply(GameModel model)
        {
            model.Accept(this);
            return result;
        }

        public void Visit(Action action)
        {
            result = tabs() + action.Type + properties<AbstractProperty>(action.Properties);
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
            result = attribute.Name + " = " + attribute.Value.Initializer;
        }

        public void Visit(Component component)
        {
            //Three tabs, game, scene, entity then component
            result = new StringBuilder("\t\t\tComponent ")
                .Append(component.Type).Append(properties<Property>(component.Properties)).ToString();
        }

        public void Visit(Condition condition)
        {
            string tabStr = tabs();
            StringBuilder conditionBuilder = new StringBuilder(tabStr)
                .Append("if(").Append(condition.If).Append(')').Append(openDef());
            conditionBuilder.Append(visitMembers<AbstractStatement>(condition.Statements, "\n", allValid));
            conditionBuilder.Append(closeDef());
            result = conditionBuilder.ToString();
        }

        public void Visit(Define define)
        {
            //Tab from using
            result = "\tdefine " + define.Name + " as " + define.Class;
        }

        public void Visit(Entity entity)
        {
            //tab from game and scene for entity and game for prototype.
            string tabstr = entity.IsPrototype ? "\t" : "\t\t";
            StringBuilder entityBuilder = new StringBuilder(tabstr)
                .Append(entity.IsPrototype? "Prototype " :  "Entity ").Append(entity.Name ?? "");

            if(entity.Prototypes.Count != 0){
                entityBuilder.Append(" : ");
                List<string> prototypes = new List<string>();
                foreach(Entity prototype in entity.Prototypes) prototypes.Add(prototype.Name);
                entityBuilder.Append(string.Join(", ", prototypes));
            }

            entityBuilder.Append(actions(entity.Attributes)).Append(openDef());

            if(entity.Components.Count != 0) entityBuilder.Append(visitMembers<Component>(entity.Components, "\n", validComponent)).Append('\n');
            if (entity.Events.Count != 0) entityBuilder.Append(visitMembers<AbstractEvent>(entity.Events, "\n", validEvt)).Append('\n');

            result = entityBuilder.Append(closeDef()).ToString();
        }

        public void Visit(Event evt)
        {
            result = new StringBuilder("\t\t\tEvent ").Append(evt.Type)
                .Append(properties<AbstractProperty>(evt.Properties)).Append(openDef())
                .Append(visitMembers<AbstractStatement>(evt.Statements, "\n", allValid)).Append("\t\t\t").Append(closeDef()).ToString();
        }

        public void Visit(ForLoop loop)
        {
            string tabin = tabs();
            StringBuilder sb = new StringBuilder(tabin).Append("for(").Append(loop.PreExpression).Append("; ")
                .Append(loop.Expression).Append("; ").Append(loop.PostExpression).Append(")").Append(openDef());

            sb.Append(visitMembers<GameModel>(loop.Children, "\n", allValid));

            sb.Append("\n").Append(tabin).Append(closeDef());
            result = sb.ToString();
        }

        public void Visit(Game game)
        {
            result = new StringBuilder(visitMembers<Using>(game.Usings, "", allValid))
                .Append("Game(").Append(visitMembers<Attribute>(game.Attributes, ",", allValid)).Append(")")
                .Append(openDef()).Append(visitMembers<Entity>(game.Prototypes, "", allValid))
                .Append(visitMembers<Scene>(game.Scenes, "", allValid)).Append(closeDef()).ToString();
        }

        public void Visit(ReadOnlyAction action)
        {
            result = "";
        }

        public void Visit(ReadOnlyAssignment assignment)
        {
            result = "";
        }

        public void Visit(ReadOnlyCondition condition)
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
            result = new StringBuilder("\t\tManager ")
                .Append(manager.Type).Append(properties<Property>(manager.Properties)).ToString();
        }

        public void Visit(Property property)
        {
            //TODO change this when it is a value
            result = property.Name + '=' + Apply(property.Value);
        }

        public void Visit(Scene scene)
        {
            //tab from game
            StringBuilder sceneBuilder = new StringBuilder("\tScene ").Append(scene.Name).Append(actions(scene.Attributes)).Append(openDef());

            if (scene.Managers.Count != 0)
                sceneBuilder.Append(visitMembers<Manager>(scene.Managers, "\n", allValid)).Append('\n');

            sceneBuilder.Append(visitMembers<Entity>(scene.Entities, "", allValid)).Append(closeDef());
            result = sceneBuilder.ToString();
        }

        public void Visit(Service service)
        {
            result = new StringBuilder("\t\tService ")
                .Append(service.Type).Append(properties<Property>(service.Properties)).ToString();
        }

        public void Visit(Using use)
        {
            if (use.File == null) return;
            result = new StringBuilder("using ").Append(use.File).Append(openDef())
                .Append(visitMembers<Define>(use.Defines, "\n", allValid)).Append(closeDef()).ToString();
        }

        public void Visit(Value val)
        {
            result = val.Initializer;
        }

        public void Visit(WhileLoop loop)
        {
            StringBuilder sb = new StringBuilder(tabs()).Append("while(").Append(loop.Expression).Append(")")
                .Append(openDef()).Append(visitMembers<GameModel>(loop.Children, "\n", allValid)).Append(closeDef());

            result = sb.ToString();
        }

        private string properties<T>(IEnumerable<T> properties) where T : AbstractProperty
        {
            return '(' + visitMembers<T>(properties, ", ", validProperties) + ')';
        }

        private string actions(IEnumerable<Attribute> properties)
        {
            return '(' + visitMembers<Attribute>(properties, ", ", allValid) + ')';
        }

        private string visitMembers<T>(IEnumerable<T> members, string joinWith, Func<T, bool> valid) where T : GameModel
        {
            List<string> memberStrings = new List<string>();
            foreach (T member in members.Where(member => valid(member)))
            {
                member.Accept(this);
                memberStrings.Add(result);
            }
            return string.Join(joinWith, memberStrings);
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
            numTabs++;
            return "\n" + tabs() + "{\n";
        }

        private string closeDef()
        {
            numTabs--;
            return "\n" + tabs() + "}\n" ;
        }

    }
}
