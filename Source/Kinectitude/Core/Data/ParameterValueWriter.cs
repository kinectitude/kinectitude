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
