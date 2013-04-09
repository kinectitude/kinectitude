//-----------------------------------------------------------------------
// <copyright file="ReadOnlyProperty.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class ReadOnlyProperty : AbstractProperty
    {
        private readonly AbstractProperty inheritedProperty;

        public override PluginProperty PluginProperty
        {
            get { return inheritedProperty.PluginProperty; }
        }

        public override Value Value
        {
            get { return inheritedProperty.Value; }
            set { }
        }

        public override bool HasOwnValue
        {
            get { return inheritedProperty.HasOwnValue; }
        }

        public override IEnumerable<object> AvailableValues
        {
            get { return inheritedProperty.AvailableValues; }
        }

        public override ICommand ClearValueCommand { get; protected set; }

        public ReadOnlyProperty(AbstractProperty inheritedProperty) : base(inheritedProperty)
        {
            this.inheritedProperty = inheritedProperty;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
