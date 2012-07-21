using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal class Timer
    {
        internal IExpressionReader ExpressionReader { get; private set; }
        internal bool Recurring { get; private set; }

        private float time;
        private readonly float initTime;

        internal Timer(IExpressionReader expressionReader, float time, bool recurring)
        {
            ExpressionReader = expressionReader;
            initTime = this.time = time;
            Recurring = recurring;
        }

        internal bool tick(float duration)
        {
            time -= duration;
            bool trigger = time <= 0;
            if (Recurring && trigger)
            {
                time = initTime;
            }
            return trigger;
        }
    }
}
