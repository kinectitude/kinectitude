using System;
using MessagePassing.Public;

namespace MessagePassing.Components
{
    public class TransformComponent : Component
    {
        private double x;
        private double y;

        public double X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                    Messenger.Publish("X", x);
                }
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    Messenger.Publish("Y", y);
                }
            }
        }

        [Action]
        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
