//-----------------------------------------------------------------------
// <copyright file="DataContainerWriter.cs" company="Kinectitude">
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
    internal sealed class DataContainerWriter : ValueWriter
    {
        private readonly string param;

        internal IDataContainer DataContainer { get; set; }

        internal DataContainerWriter(string param, ValueReader reader) : base(reader)
        {
            this.param = param;
        }

        public override void SetValue(ValueReader value) { DataContainer[param] = value; }
    }
}
