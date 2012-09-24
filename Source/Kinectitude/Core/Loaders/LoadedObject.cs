using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

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

                string value = ClassFactory.PreEvaluate(obj, param) ? 
                    ExpressionReader.CreateExpressionReader(val.Item2, evt, entity).GetValue() : val.Item2;

                ClassFactory.SetParam(obj, param, value, evt, entity);
            }
        }
    }
}
