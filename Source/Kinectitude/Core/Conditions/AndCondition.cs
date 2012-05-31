using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Conditions
{
    internal class AndCondition : Condition
    {
        private readonly List<Condition> conditions;
        
        internal AndCondition(List<Condition> conditions, Event evt) : base(evt)
        {
            this.conditions = conditions;
        }

        internal override bool ShouldRun()
        {
            foreach (Condition condition in conditions)
            {
                if (!condition.ShouldRun())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
