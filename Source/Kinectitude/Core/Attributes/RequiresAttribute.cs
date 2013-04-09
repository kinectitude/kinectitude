//-----------------------------------------------------------------------
// <copyright file="RequiresAttribute.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Kinectitude.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class RequiresAttribute : Attribute
    {
        private readonly Type type;

        public Type Type
        {
            get { return type; }
        }

        public RequiresAttribute(Type type)
        {
            this.type = type;
        }
    }
}
