using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class RequiresAttribute : Attribute
    {
        private readonly Type type;

        public Type Type
        {
            get { return type; }
        }

        public RequiresAttribute(Type type)
        {
            this.type = type;
        }
    }
}
