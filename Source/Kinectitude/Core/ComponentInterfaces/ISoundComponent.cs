using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.ComponentInterfaces
{
    public interface ISound : IUpdateable
    {
        String Filename { get; set; }

        bool Looping { get; set; }

        float Volume { get; set; }
    }
}
