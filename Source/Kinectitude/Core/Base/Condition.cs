using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kinectitude.Core.Conditions;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal abstract class Condition : Action
    {
        private static List<Condition> conditionBuilder(string[] conditionStrings, Event evt, Entity entity)
        {
            List<Condition> conditions = new List<Condition>();
            foreach (string condition in conditionStrings)
            {
                conditions.Add(new SimpleCondition(ExpressionReader.CreateExpressionReader(condition, evt, entity), evt));
            }
            return conditions;
        }

        internal static Condition CreateCondition(string value, Event evt, Entity entity)
        {
            value = value.Trim();
            //always and before or, this is like C/C++/C#/Java
            if (value.Contains(" and ") && value.Contains(" or "))
            {
                int orLoc = value.LastIndexOf(" or ");
                Condition firstPart = Condition.CreateCondition(value.Substring(0, orLoc), evt, entity);
                Condition secondPart = Condition.CreateCondition(value.Substring(orLoc + 4), evt, entity);
                return new OrCondition(new List<Condition> { firstPart, secondPart }, evt);
            }
            else if (value.Contains(" and "))
            {
                string[] conditionStrs = Regex.Split(value, " and ");
                return new AndCondition(conditionBuilder(conditionStrs, evt, entity), evt);
            }
            else if (value.Contains(" or "))
            {
                string[] conditionStrs = Regex.Split(value, " or ");
                return new OrCondition(conditionBuilder(conditionStrs, evt, entity), evt);
            }
            else
            {
                return new SimpleCondition(ExpressionReader.CreateExpressionReader(value, evt, entity), evt);
            }
        }
        
        private List<Action> actions;

        protected Condition(Event evt)
        {
            actions = new List<Action>();
            Event = evt;
        }

        internal void AddAction(Action action)
        {
            action.Event = Event;
            actions.Add(action);
        }
        
        internal abstract bool ShouldRun();
        
        public override void Run()
        {
            if (ShouldRun())
            {
                foreach (Action a in actions)
                {
                    a.Run();
                }
            }
        }
    }
}
