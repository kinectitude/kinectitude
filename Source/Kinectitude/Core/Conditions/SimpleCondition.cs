using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Conditions
{
    internal class SimpleCondition : Condition
    {
        private readonly ExpressionReader specificReadable;
        
        internal SimpleCondition(ExpressionReader specificReadable, Event evt) : base(evt)
        {
            this.specificReadable = specificReadable;
        }

        internal override bool ShouldRun()
        {
            return null != specificReadable.GetValue() && specificReadable.GetValue().ToLower() != "false" && "" != specificReadable.GetValue();
        }
    }
}
