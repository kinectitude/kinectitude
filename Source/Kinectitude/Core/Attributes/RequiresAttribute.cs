using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresAttribute : Attribute
    {
        private readonly Type type;

        public RequiresAttribute(Type type)
        {
            this.type = type;
        }
    }
}
