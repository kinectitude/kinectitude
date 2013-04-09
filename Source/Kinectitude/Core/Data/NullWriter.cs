//-----------------------------------------------------------------------
// <copyright file="NullWriter.cs" company="Kinectitude">
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
    /// <summary>
    /// Used to keep the game from crashing when an invalid writer is used.
    /// </summary>
    internal sealed class NullWriter : ValueWriter
    {
        public NullWriter(ValueReader reader) : base(reader) { }
        public override void SetValue(ValueReader value) 
        {
            Game.CurrentGame.Die("The target of a write must be a single attribute or property name");
        }
    }
}
