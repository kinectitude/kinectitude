using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
{
    internal class AndCondition:Condition
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
