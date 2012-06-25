using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ProvidesAttribute : Attribute
    {
        private readonly Type type;

        public Type Type
        {
            get { return type; }
        }

        public ProvidesAttribute(Type type)
        {
            this.type = type;
        }
    }
}