using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    class IntExpressionReader : IIntExpressionReader
    {
        private readonly ExpressionEval expression;

        internal IntExpressionReader(string expressionStr, Event evt, Entity entity)
        {
            expression = new ExpressionEval(expressionStr, evt, entity);
        }

        public int GetValue()
        {
            return expression.ToNumber<int>();
        }

        public void notifyOfChange(Action<string> callback)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
