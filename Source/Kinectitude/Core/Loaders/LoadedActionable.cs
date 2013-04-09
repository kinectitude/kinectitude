//-----------------------------------------------------------------------
// <copyright file="LoadedActionable.cs" company="Kinectitude">
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

namespace Kinectitude.Core.Loaders
{
    internal abstract class LoadedActionable : LoadedBaseAction
    {
        protected  List<LoadedBaseAction> Actions {get; private set;}
        protected readonly object ConditianlExpression;

        protected LoadedActionable(object conditionalExpr, LoaderUtility loader) : base(null, loader)
        {
            Actions = new List<LoadedBaseAction>();
            ConditianlExpression = conditionalExpr;
        }

        internal void AddAction(LoadedBaseAction action)
        {
            Actions.Add(action);
        }

        internal virtual void Ready() { }
    }
}
