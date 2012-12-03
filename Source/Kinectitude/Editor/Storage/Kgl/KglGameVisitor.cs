﻿using Kinectitude.Core.Language;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Models.Statements.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Editor.Models.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KglGameVisitor : IGameVisitor
    {
        private const string OpenDef = "{\n";
        private const string CloseDef = "}\n";

        private static readonly Func<AbstractProperty, bool> validProperties = (property => property.IsLocal);
        private static readonly Func<GameModel, bool> allValid = (model => true);
        private static readonly Func<Component, bool> validComponent = (component => component.IsRoot || component.HasLocalProperties);
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
            throw new NotImplementedException();
        }

        public void Visit(Attribute attribute)
        {
            result = attribute.Key + '=' + attribute.Value;
        }

        public void Visit(Component component)
        {
            //Three tabs, game, scene, entity then component
            result = new StringBuilder("\t\t\tComponent ")
                .Append(component.DisplayName).Append(properties<Property>(component.Properties)).ToString();
        }

        public void Visit(Condition condition)
        {
            string tabStr = tabs();
            StringBuilder conditionBuilder = new StringBuilder(tabStr)
                .Append("if(").Append(condition.If).Append(')').Append(OpenDef);
            numTabs++;
            conditionBuilder.Append(visitMembers<AbstractStatement>(condition.Statements, "\n", allValid));
            numTabs--;
            conditionBuilder.Append('\n').Append(tabStr).Append(CloseDef);
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

            entityBuilder.Append(actions(entity.Attributes)).Append(OpenDef);

            if(entity.Components.Count != 0) entityBuilder.Append(visitMembers<Component>(entity.Components, "\n", validComponent)).Append('\n');
            if (entity.Events.Count != 0) entityBuilder.Append(visitMembers<AbstractEvent>(entity.Events, "\n", validEvt)).Append('\n');

            result = entityBuilder.ToString();
        }

        public void Visit(Event evt)
        {
            result = new StringBuilder("\t\t\tEvent ").Append(evt.Type)
                .Append(properties<AbstractProperty>(evt.Properties)).Append(OpenDef)
                .Append(visitMembers<AbstractStatement>(evt.Statements, "\n", allValid)).Append("\t\t\t").ToString();
        }

        public void Visit(ForLoop loop)
        {
            throw new NotImplementedException();
        }

        public void Visit(Game game)
        {
            result = new StringBuilder(visitMembers<Using>(game.Usings, "", allValid))
                .Append(visitMembers<Entity>(game.Prototypes, "", allValid))
                .Append(visitMembers<Scene>(game.Scenes, "", allValid)).ToString();
        }

        public void Visit(InheritedAction action)
        {
            result = "";
        }

        public void Visit(InheritedAssignment assignment)
        {
            result = "";
        }

        public void Visit(InheritedCondition condition)
        {
            result = "";
        }

        public void Visit(InheritedEvent evt)
        {
            result = "";
        }

        public void Visit(InheritedForLoop loop)
        {
            result = "";
        }

        public void Visit(InheritedProperty property)
        {
            result = "";
        }

        public void Visit(InheritedWhileLoop loop)
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
            result = property.Name + '=' + property.Value;
        }

        public void Visit(Scene scene)
        {
            //tab from game
            StringBuilder sceneBuilder = new StringBuilder("\tScene ").Append(scene.Name);
            if (scene.Managers.Count != 0)
                sceneBuilder.Append(visitMembers<Manager>(scene.Managers, "\n", allValid)).Append('\n');

            sceneBuilder.Append(visitMembers<Entity>(scene.Entities, "", allValid));
            result = sceneBuilder.ToString();
        }

        public void Visit(Using use)
        {
            result = new StringBuilder("Using ").Append(use.File).Append(OpenDef)
                .Append(visitMembers<Define>(use.Defines, "\n", allValid)).Append(CloseDef).ToString();
        }

        public void Visit(WhileLoop loop)
        {
            throw new NotImplementedException();
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
            for (int i = 0; i < numTabs; i++) tabBuilder.Append('\t');
            return tabBuilder.ToString();
        }
    }
}