//-----------------------------------------------------------------------
// <copyright file="PluginAttribute.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
