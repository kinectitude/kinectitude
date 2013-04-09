//-----------------------------------------------------------------------
// <copyright file="PresetAttribute.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Kinectitude.Core.Attributes
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
