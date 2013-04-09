//-----------------------------------------------------------------------
// <copyright file="LoadedObject.cs" company="Kinectitude">
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

        protected void setValues(object obj, Event evt, Entity entity, Scene scene)
        {
            foreach (Tuple<string, object> val in Values)
            {
                object assignable = LoaderUtil.MakeAssignable(val.Item2, scene, entity, evt);
                ClassFactory.SetParam(obj, val.Item1, assignable);
            }
        }
    }
}
