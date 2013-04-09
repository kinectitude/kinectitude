//-----------------------------------------------------------------------
// <copyright file="IValueMaker.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Storage
{
    interface IValueMaker
    {
        bool HasErrors(string value);
        ValueReader CreateValueReader(string value, IScene scene, IDataContainer entity, Event evt = null);
    }
}
