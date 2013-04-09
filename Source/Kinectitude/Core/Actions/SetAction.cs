//-----------------------------------------------------------------------
// <copyright file="SetAction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    internal sealed class SetAction : Action
    {
        private readonly ValueReader Value;
        private readonly ValueWriter Target;

        public SetAction(ValueWriter target, ValueReader value, Event evt) 
        {
            Target = target;
            Value = value;
            Event = evt;
        }

        public override void Run()
        {
            Target.SetValue(Value);
        }
    }
}
