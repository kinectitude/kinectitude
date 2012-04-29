using System;
using System.Xml;
using System.Reflection;

namespace Kinectitude.Core
{
    public abstract class Action
    {
        public Event Event { get; set; }

        public abstract void Run();
    }
}
