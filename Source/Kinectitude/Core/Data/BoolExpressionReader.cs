using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal class BoolExpressionReader : IBoolExpressionReader
    {

        private readonly ExpressionEval expression;

        internal BoolExpressionReader(string expressionStr, Event evt, Entity entity)
        {
            expression = new ExpressionEval(expressionStr, evt, entity);
        }

        public bool GetValue()
        {
            return expression.ToBool();
        }

        public void notifyOfChange(Action<string> callback)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
