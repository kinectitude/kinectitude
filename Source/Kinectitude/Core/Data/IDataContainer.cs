//-----------------------------------------------------------------------
// <copyright file="IDataContainer.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal interface IDataContainer
    {
        ValueReader this[string key] { get; set; }
        IChangeable GetChangeable(string name);

        void NotifyOfChange(string key, IChanges callback);
        void NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback);
    }
}
