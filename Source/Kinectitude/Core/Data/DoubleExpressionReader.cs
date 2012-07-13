using System;
using Kinectitude.Core.Base;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    class DoubleExpressionReader : IDoubleExpressionReader
    {
        private readonly ExpressionEval expression;
        private readonly List<Action<string>> callbacks = new List<Action<string>>();
        private bool isnNotified = false;
        private double oldVal;

        internal DoubleExpressionReader(string expressionStr, Event evt, Entity entity)
        {
            expression = new ExpressionEval(expressionStr, evt, entity);
        }

        public double GetValue()
        {
            return expression.ToNumber<double>();
        }


        public void changeOccured(string change)
        {
            double result = expression.ToNumber<double>();
            if (oldVal != result)
            {
                foreach (Action<string> callback in callbacks)
                {
                    callback(change);
                }
                oldVal = result;
            }
        }

        public void notifyOfChange(Action<string> callback)
        {
            callbacks.Add(callback);
            if (!isnNotified)
            {
                isnNotified = true;
                expression.notifyOfChange(changeOccured);
                oldVal = expression.ToNumber<double>();
            }
        }
    }
}
