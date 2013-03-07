﻿using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.ValueReaders
{
    internal sealed class ManagerValueReader : RepeatReader
    {
        private readonly ManagerChangeable changeable;
        private readonly string parameter;

        protected override ValueReader Reader
        {
            get
            {
                var manager = changeable.Manager;
                if (null != manager)
                {
                    var property = manager.GetProperty(parameter);
                    if (null != property)
                    {
                        return property.Value.Reader;
                    }
                }

                return ConstantReader.NullValue;
            }
        }

        public ManagerValueReader(ManagerChangeable changeable, string parameter)
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