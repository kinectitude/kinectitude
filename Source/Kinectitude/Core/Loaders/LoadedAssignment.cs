//-----------------------------------------------------------------------
// <copyright file="LoadedAssignment.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Actions;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedAssignment : LoadedBaseAction
    {
        private readonly object Target;
        private readonly object Value;
        private readonly object Type;
        private readonly LoaderUtility Loader;

        internal LoadedAssignment(object target, object type, object value, LoaderUtility loader) : base(null, loader)
        {
            Target = target;
            Value = value;
            Type = type;
            Loader = loader;
        }

        internal override Action Create(Event evt)
        {
            ValueReader LS = Loader.MakeAssignable(Target, evt.Entity.Scene, evt.Entity, evt) as ValueReader;
            ValueReader RS = Loader.MakeAssignable(Value, evt.Entity.Scene, evt.Entity, evt) as ValueReader;
            ValueReader value = LoaderUtil.MakeAssignmentValue(LS, Type, RS);
            return new SetAction(LS.GetValueWriter(), value, evt);
        }
    }
}
