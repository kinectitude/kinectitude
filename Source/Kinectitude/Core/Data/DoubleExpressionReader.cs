using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    class DoubleExpressionReader : IDoubleExpressionReader
    {
        private readonly ExpressionEval expression;

        internal DoubleExpressionReader(string expressionStr, Event evt, Entity entity)
        {
            expression = new ExpressionEval(expressionStr, evt, entity);
        }

        public double GetValue()
        {
            return expression.ToNumber<double>();
        }

        public void notifyOfChange(Action<string> callback)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
