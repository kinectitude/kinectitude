using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Attributes
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
