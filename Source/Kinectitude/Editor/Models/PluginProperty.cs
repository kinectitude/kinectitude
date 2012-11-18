using Kinectitude.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal class PluginProperty : BaseModel
    {
        public Type Type { get; private set; }

        public PluginProperty(PropertyInfo info)
        {
            Type = info.PropertyType;
        }

        public Value CreateValue()
        {
            //if (Type.IsEnum)
            //{
            //    return new EnumerationValue(this);
            //}

            //return BasicValue(this);
            return null;
        }
    }
}
