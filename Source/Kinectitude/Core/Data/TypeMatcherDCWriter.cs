//-----------------------------------------------------------------------
// <copyright file="TypeMatcherDCWriter.cs" company="Kinectitude">
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
    internal sealed class TypeMatcherDCWriter : ValueWriter
    {
        private readonly TypeMatcherDCReader reader;
        internal TypeMatcherDCWriter(TypeMatcherDCReader reader) : base(reader)
        {
            this.reader = reader;
        }

        public override void SetValue(ValueReader value)
        {
            reader.Watcher.GetTypeMatcher()[reader.Param] = value;
        }
    }
}
