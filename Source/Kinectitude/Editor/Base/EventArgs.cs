using System;

namespace Kinectitude.Editor.Base
{
    internal sealed class EventArgs<T> : EventArgs
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
