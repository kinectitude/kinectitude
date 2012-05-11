using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Action = Kinectitude.Editor.Models.Plugins.Action;
using Kinectitude.Editor.Models.Expressions;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.Models.Base
{
    public class Condition : IAction, IActionContainer
    {
        private IActionContainer parent;
        private BooleanExpression when;
        private readonly List<IAction> actions;

        public IActionContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public BooleanExpression When
        {
            get { return when; }
            set { when = value; }
        }

        public IEnumerable<IAction> Actions
        {
            get { return actions; }
        }

        public Condition()
        {
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
