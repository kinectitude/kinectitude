//-----------------------------------------------------------------------
// <copyright file="LoadedBaseAction.cs" company="Kinectitude">
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
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal abstract class LoadedBaseAction : LoadedObject
    {
        protected LoadedBaseAction(PropertyHolder values, LoaderUtility loaderUtil) : base(values, loaderUtil) { }
        internal abstract Action Create(Event evt);
    }
}
