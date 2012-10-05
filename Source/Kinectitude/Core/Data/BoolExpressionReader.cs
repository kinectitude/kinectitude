using System;
using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal class BoolExpressionReader : IBoolExpressionReader
    {
        private readonly List<Action<string>> callbacks = new List<Action<string>>();
        private bool isnNotified = false;
        private bool oldVal;

        private readonly ExpressionEval expression;

        internal BoolExpressionReader(string expressionStr, Event evt, Entity entity)
        {
            expression = new ExpressionEval(expressionStr, evt, entity);
        }

        public bool GetValue()
        {
            return expression.ToBool();
        }
        public void changeOccured(string change)
        {
            bool result = expression.ToBool();
            if (oldVal != result)
            {
                foreach (Action<string> callback in callbacks)
                {
                    callback(change);
                }
            }
        }

        public void notifyOfChange(Action<string> callback)
        {
            callbacks.Add(callback);
            if (!isnNotified)
            {
                isnNotified = true;
                expression.notifyOfChange(changeOccured);
                oldVal = expression.ToBool();
            }
        }
    }
}
