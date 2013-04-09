//-----------------------------------------------------------------------
// <copyright file="IScene.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal interface IScene : IDataContainer
    {
        IDataContainer Game { get; }
        IDataContainer GetEntity(string name);
        HashSet<int> GetOfPrototype(string prototype, bool exact);
    }
}
