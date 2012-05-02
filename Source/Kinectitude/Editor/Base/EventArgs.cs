using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Base
{
    public class EventArgs<T> : EventArgs
    {
        private readonly T value;

        public T Value
        {
            get { return value; }
        }

        public EventArgs(T value)
        {
            this.value = value;
        }
    }
}
