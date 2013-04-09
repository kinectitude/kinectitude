//-----------------------------------------------------------------------
// <copyright file="UniOpReader.cs" company="Kinectitude">
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
    internal abstract class UniOpReader : ValueReader
    {
        protected readonly ValueReader Reader;

        protected UniOpReader(ValueReader reader) { Reader = reader; }

        internal override void SetupNotifications() { Reader.NotifyOfChange(this); }

        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}
