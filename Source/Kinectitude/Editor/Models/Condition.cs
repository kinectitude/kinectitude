using Kinectitude.Editor.Base;
using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal class Condition : AbstractCondition
    {
        private string rule;

        public override string If
        {
            get { return rule; }
            set
            {
                if (rule != value)
                {
                    rule = value;
                    NotifyPropertyChanged("If");
                }
            }
        }

        public override Plugin Plugin
        {
            get { return null; }
        }

        public override string Type
        {
            get { return null; }
        }

        public override string DisplayName
        {
            get { return null; }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public override bool IsInherited
        {
            get { return false; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Distinct(); }
        }

        public ICommand AddActionCommand { get; private set; }

        public Condition()
        {
            AddActionCommand = new DelegateCommand(null,
                (parameter) =>
                {
                    Plugin actionPlugin = parameter as Plugin;
                    if (null != actionPlugin)
                    {
                        Action action = new Action(actionPlugin);
                        AddAction(action);
                    }
                }
            );
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool InheritsFrom(AbstractAction action)
        {
            return false;
        }

        public override AbstractAction DeepCopy()
        {
            Condition copy = new Condition() { If = this.If };

            foreach (AbstractAction action in this.Actions)
            {
                copy.AddAction(action.DeepCopy());
            }

            return copy;
        }
    }
}
