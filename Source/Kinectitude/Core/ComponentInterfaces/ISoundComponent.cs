using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using System;

namespace Kinectitude.Core.ComponentInterfaces
{
    public interface ISound : IUpdateable
    {
        String Filename { get; set; }

        bool Looping { get; set; }

        float Volume { get; set; }
    }
}
