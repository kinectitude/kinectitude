using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.Models.Plugins
{
    public class Event : Plugin, IActionContainer
    {
        private IEventContainer parent;

        private readonly List<IAction> actions;

        public IEventContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public IEnumerable<IAction> Actions
        {
            get { return actions; }
        }

        public Event(PluginDescriptor descriptor) : base(descriptor)
        {
            //_actions = new List<IAction>();
            //actions = new ReadOnlyCollection<IAction>(_actions);

            actions = new List<IAction>();
        }

        public void AddAction(IAction action)
        {
            action.Parent = this;
            actions.Add(action);
        }

        public void RemoveAction(IAction action)
        {
            action.Parent = null;
            actions.Remove(action);
        }
    }
}
