﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PluginPropertyAttribute : Attribute
    {
        private readonly string name;
        private readonly string description;
        private readonly object defaultValue;

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

        public PluginPropertyAttribute(string name, string description, object defaultValue = null)
        {
            this.name = name;
            this.description = description;
            this.defaultValue = defaultValue;
        }
    }
}