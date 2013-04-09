//-----------------------------------------------------------------------
// <copyright file="Changeable.cs" company="Kinectitude">
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

namespace Kinectitude.Core.Base
{
    public class Changeable : IChangeable
    {
        internal DataContainer DataContainer { get; set; }

        bool IChangeable.ShouldCheck { get; set; }

        protected void Change(string str)
        {
            if (((IChangeable) this).ShouldCheck) DataContainer.ChangedProperty(new Tuple<IChangeable, string>((IChangeable)this, str));
        }

        public object this[string parameter]
        {
            get { return ClassFactory.GetParam(this, parameter); }
        }
    }
}
