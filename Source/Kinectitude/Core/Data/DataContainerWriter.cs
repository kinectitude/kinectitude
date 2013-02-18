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
