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
