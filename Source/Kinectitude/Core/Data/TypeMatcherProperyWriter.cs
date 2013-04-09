//-----------------------------------------------------------------------
// <copyright file="TypeMatcherProperyWriter.cs" company="Kinectitude">
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

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherProperyWriter : ValueWriter
    {
        private readonly TypeMatcherProperyReader reader;
        internal TypeMatcherProperyWriter(TypeMatcherProperyReader reader) : base(reader)
        {
            this.reader = reader;
        }

        public override void SetValue(ValueReader value)
        {
            reader.Watcher.GetTypeMatcher().setComponentValue(reader.ComponentName, reader.Param, value);
        }
    }
}
