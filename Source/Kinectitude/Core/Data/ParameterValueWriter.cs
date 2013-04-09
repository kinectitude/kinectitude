//-----------------------------------------------------------------------
// <copyright file="ParameterValueWriter.cs" company="Kinectitude">
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
    internal sealed class ParameterValueWriter : ValueWriter
    {
        private object Obj;
        private string Param;
        private IDataContainer Owner;

        public ParameterValueWriter(ParameterValueReader reader) : base(reader)
        {
            Obj = reader.Obj;
            Param = reader.Param;
            Owner = reader.Owner;
        }

        public override void SetValue(ValueReader value) { ClassFactory.SetParam(Obj, Param, value); }
    }
}
