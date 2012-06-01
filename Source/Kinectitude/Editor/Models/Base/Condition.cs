using System.Collections.Generic;
using Kinectitude.Editor.Models.Expressions;

namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Condition : IAction, IActionContainer
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
