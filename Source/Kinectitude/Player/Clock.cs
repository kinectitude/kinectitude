//-----------------------------------------------------------------------
// <copyright file="Clock.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;

namespace Kinectitude.Player
{
    /// <summary>
    /// Provides a clock for the game and framerate
    /// </summary>
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
