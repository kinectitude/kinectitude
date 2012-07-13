using System;
using Kinectitude.Core.Base;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    class IntExpressionReader : IIntExpressionReader
    {
        private readonly ExpressionEval expression;
        private List<Action<string>> callbacks = new List<Action<string>>();
        private int lastIntVal;
        bool hasCallback = false;

        internal IntExpressionReader(string expressionStr, Event evt, Entity entity)
        {
            expression = new ExpressionEval(expressionStr, evt, entity);
        }

        public int GetValue()
        {
            return expression.ToNumber<int>();
        }

        private void callbackChcek(string str)
        {
            int newVal = expression.ToNumber<int>();
            if (lastIntVal != newVal)
            {
                foreach (Action<string> callback in callbacks)
                {
                    callback(str);
                }
                lastIntVal = newVal;
            }
        }

        public void notifyOfChange(Action<string> callback)
        {
            if (!hasCallback) expression.notifyOfChange(callbackChcek);
            hasCallback = true;
        }
    }
}
