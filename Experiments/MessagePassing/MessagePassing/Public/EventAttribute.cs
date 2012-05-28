using System;

namespace MessagePassing.Public
{
    /// <summary>
    /// Used to indicate that a method is a subscription handler for an event available to Kinectitude script
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : Attribute { }
}
