//-----------------------------------------------------------------------
// <copyright file="InvalidAttributeNameException.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class InvalidAttributeNameException : EditorException
    {
        public InvalidAttributeNameException(string name) : base(string.Format("'{0}' is not a valid attribute name.", name)) { }
    }
}
