//-----------------------------------------------------------------------
// <copyright file="INameValue.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface INameValue
    {
        string Name { get; }
        Value Value { get; }

        bool IsEditable { get; }
        bool HasOwnValue { get; }
        bool HasFileChooser { get; }

        ICommand ClearValueCommand { get; }
    }
}
