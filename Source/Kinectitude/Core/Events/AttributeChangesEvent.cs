//-----------------------------------------------------------------------
// <copyright file="AttributeChangesEvent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using System;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Events
{
    // Use this as a base class for any attribute change.  It will always fire

    [Plugin("when the value of {Target} changes", "A value changes")]
    internal sealed class AttributeChangesEvent : Event, IChanges
    {
        [PluginProperty("Expression", "")]
        public ValueReader Target { get; set; }

        public AttributeChangesEvent() { }

        public override void OnInitialize()
        {
            Target.NotifyOfChange(this);
        }

        void IChanges.Prepare() { }

        void IChanges.Change() { DoActions(); }
    }
}
