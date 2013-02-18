using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.ValueReaders
{
    internal sealed class ServiceValueReader : RepeatReader
    {
        private readonly ServiceChangeable changeable;
        private readonly string parameter;

        protected override ValueReader Reader
        {
            get
            {
                var service = changeable.Service;
                if (null != service)
                {
                    var property = service.GetProperty(parameter);
                    if (null != property)
                    {
                        return property.Value.Reader;
                    }
                }

                return ConstantReader.NullValue;
            }
        }

        public ServiceValueReader(ServiceChangeable changeable, string parameter)
        {
            this.changeable = changeable;
            this.parameter = parameter;
        }

        internal override ValueWriter ConvertToWriter()
        {
            throw new NotSupportedException();
        }
    }
}
