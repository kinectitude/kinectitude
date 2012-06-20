using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class RequiresAttribute : Attribute
    {
        internal readonly Type Type;

        public RequiresAttribute(Type type)
        {
            Type = type;
        }
    }
}
