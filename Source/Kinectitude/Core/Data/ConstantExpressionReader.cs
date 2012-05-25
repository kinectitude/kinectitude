using System;

namespace Kinectitude.Core.Data
{
    internal sealed class ConstantExpressionReader : ExpressionReader
    {
        private readonly string value;

        internal ConstantExpressionReader(string value)
        {
            this.value = value;
        }
        
        public override string GetValue()
        {
            return value;
        }

        public override void notifyOfChange(Action<string, string> callback) { }
    }
}
