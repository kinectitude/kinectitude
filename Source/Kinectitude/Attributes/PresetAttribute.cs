using System;

namespace Kinectitude.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PresetAttribute : Attribute
    {
        private readonly string name;
        private readonly object value;

        public string Name
        {
            get { return name; }
        }

        public object Value
        {
            get { return value; }
        }

        public PresetAttribute(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
