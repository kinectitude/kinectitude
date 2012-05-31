﻿using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true)]
    public class PluginAttribute : Attribute
    {
        private readonly string name;
        private readonly string description;

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public PluginAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}