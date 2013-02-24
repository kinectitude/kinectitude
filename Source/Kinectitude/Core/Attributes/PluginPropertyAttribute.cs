using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PluginPropertyAttribute : Attribute
    {
        private readonly string name;
        private readonly string description;
        private readonly object defaultValue;
        private readonly string fileFilter;
        private readonly string fileChooserTitle;

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
        }

        public string FileFilter
        {
            get { return fileFilter; }
        }

        public string FileChooserTitle
        {
            get { return fileChooserTitle; }
        }

        public PluginPropertyAttribute(string name, 
                                       string description, 
                                       object defaultValue = null, 
                                       string fileFilter = null,
                                       string fileChooserTitle = null)
        {
            this.name = name;
            this.description = description;
            this.defaultValue = defaultValue;
            this.fileFilter = fileFilter;
            this.fileChooserTitle = fileChooserTitle;
        }
    }
}
