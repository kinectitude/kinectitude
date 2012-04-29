using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Editor.Base;

namespace Editor
{
    public class Event : Plugin
    {
        private IEventContainer parent;

        private readonly List<Action> _actions;
        private readonly ReadOnlyCollection<Action> actions;

        public IEventContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public ReadOnlyCollection<Action> Actions
        {
            get { return actions; }
        }

        public Event(PluginDescriptor descriptor) : base(descriptor)
        {
            _actions = new List<Action>();
            actions = new ReadOnlyCollection<Action>(_actions);
        }

        public void AddAction(Action action)
        {
            action.Parent = this;
            _actions.Add(action);
        }

        public void RemoveAction(Action action)
        {
            action.Parent = null;
            _actions.Remove(action);
        }

        public override string ToString()
        {
            return string.Format("Type: {0}", Descriptor.Name);
        }
    }
}
