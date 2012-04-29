using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kinectitude.Core
{
    internal abstract class Condition : Action
    {
        private static List<Condition> conditionBuilder(string[] conditionStrings, Event e, SceneLoader s)
        {
            List<Condition> conditions = new List<Condition>();
            foreach (string condition in conditionStrings)
            {
                conditions.Add(new SimpleCondition(SpecificReadable.CreateSpecificReadable(condition, e, s), e));
            }
            return conditions;
        }

        internal static Condition CreateCondition(string value, Event evt, SceneLoader sceneLoader)
        {
            value = value.Trim();
            //always and before or, this is like C/C++/C#/Java
            if (value.Contains(" and ") && value.Contains(" or "))
            {
                int orLoc = value.LastIndexOf(" or ");
                Condition firstPart = Condition.CreateCondition(value.Substring(0, orLoc), evt, sceneLoader);
                Condition secondPart = Condition.CreateCondition(value.Substring(orLoc + 4), evt, sceneLoader);
                return new OrCondition(new List<Condition> { firstPart, secondPart }, evt);
            }
            else if (value.Contains(" and "))
            {
                string[] conditionStrs = Regex.Split(value, " and ");
                return new AndCondition(conditionBuilder(conditionStrs, evt, sceneLoader), evt);
            }
            else if (value.Contains(" or "))
            {
                string[] conditionStrs = Regex.Split(value, " or ");
                return new OrCondition(conditionBuilder(conditionStrs, evt, sceneLoader), evt);
            }
            else
            {
                return new SimpleCondition(SpecificReadable.CreateSpecificReadable(value, evt, sceneLoader), evt);
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
