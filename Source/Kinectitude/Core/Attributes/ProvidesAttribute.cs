using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ProvidesAttribute : Attribute
    {
        internal readonly Type Type;

        public ProvidesAttribute(Type type)
        {
            Type = type;
        }
    }
}
