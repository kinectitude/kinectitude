using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : Attribute
    {
        private readonly string header;
        private readonly string description;

        public string Header
        {
            get { return header; }
        }

        public string Description
        {
            get { return description; }
        }

        public PluginAttribute(string header, string description)
        {
            this.header = header;
            this.description = description;
        }
    }
}
