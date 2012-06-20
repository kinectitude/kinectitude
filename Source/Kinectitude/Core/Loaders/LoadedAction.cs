using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedAction : LoadedBaseAction
    {
        private readonly string type;

        internal LoadedAction(string type, List<Tuple<string, string>> values)
            : base(values)
        {
            this.type = type;
        }

        internal override Action Create(Event evt)
        {
            Action action = ClassFactory.Create<Action>(type);
            setValues(action, evt);
            action.Event = evt;
            return action;
        }
    }
}
