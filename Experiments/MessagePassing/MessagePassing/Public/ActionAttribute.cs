using System;

namespace MessagePassing.Public
{
    /// <summary>
    /// Used to indicate that a method is an action available to Kinectitude script
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute { }
}
