using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Attributes
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
