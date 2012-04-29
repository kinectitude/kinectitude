using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SlimDxRunner
{
    public class Clock
    {
        private readonly long frequency;
        private bool running;
        private long count;

        public Clock()
        {
            frequency = Stopwatch.Frequency;
        }

        public void Start()
        {
            count = Stopwatch.GetTimestamp();
            running = true;
        }

        public float Update()
        {
            float result = 0.0f;
            if (running)
            {
                long last = count;
                count = Stopwatch.GetTimestamp();
                result = (float)(count - last) / frequency;
            }
            return result;
        }
    }
}
