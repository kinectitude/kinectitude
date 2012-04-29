using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
{
    internal class SimpleCondition : Condition
    {
        private readonly SpecificReadable specificReadable;
        
        internal SimpleCondition(SpecificReadable specificReadable, Event evt) : base(evt)
        {
            this.specificReadable = specificReadable;
        }

        internal override bool ShouldRun()
        {
            return null != specificReadable.GetValue() && specificReadable.GetValue().ToLower() != "false" && "" != specificReadable.GetValue();
        }
    }
}
