using System;

namespace Kinectitude.Core.Data
{
    internal class EvalExpressionReader : ExpressionReader
    {
        private readonly ExpressionEval expression;
        internal EvalExpressionReader(ExpressionEval expr)
        {
            expression = expr;
        }

        public override string GetValue()
        {
            return expression.ToStr();
        }

        public override void notifyOfChange(Action<string> callback)
        {
            expression.notifyOfChange(callback);
        }
    }
}
