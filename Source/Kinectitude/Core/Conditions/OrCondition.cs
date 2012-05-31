using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Conditions
{
    internal class OrCondition : Condition
    {
        private readonly List<Condition> conditions;

        internal OrCondition(List<Condition> conditions, Event evt) : base(evt)
        {
            this.conditions = conditions;
        }

        internal override bool ShouldRun()
        {
            foreach (Condition condition in conditions)
            {
                if (condition.ShouldRun())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
