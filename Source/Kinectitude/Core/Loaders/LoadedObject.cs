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
        internal readonly PropertyHolder Values;
        protected readonly LoaderUtility LoaderUtil;

        protected LoadedObject(PropertyHolder values, LoaderUtility loaderUtil)
        {
            Values = values;
            LoaderUtil = loaderUtil;
        }

        protected void setValues(object obj, Event evt, Entity entity)
        {
            foreach (Tuple<string, object> val in Values)
            {
                IAssignable assignable = LoaderUtil.MakeAssignable(val.Item2, entity.Scene, entity, evt);
                assignable.SetParam(obj, val.Item1);
            }
        }
    }
}
