﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal abstract class LoadedObject
    {
        protected List<Tuple<string, string>> Values { get; private set; }

        protected LoadedObject(List<Tuple<string, string>> values)
        {
            Values = values;
        }

        protected void setValues(object obj, Event evt, Entity entity)
        {
            foreach (Tuple<string, string> val in Values)
            {
                string param = val.Item1;
                string value = val.Item2;
                ClassFactory.SetParam(obj, param, value, evt, entity);
            }
        }

        protected void setValues(DataContainer dc)
        {
            foreach (Tuple<string, string> val in Values)
            {
                string param = val.Item1;
                string value = val.Item2;
                dc[param] = value;
            }
        }

    }
}
