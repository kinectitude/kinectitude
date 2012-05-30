using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProvidesAttribute : Attribute
    {
        private readonly Type type;

        public ProvidesAttribute(Type type)
        {
            this.type = type;
        }
    }
}
