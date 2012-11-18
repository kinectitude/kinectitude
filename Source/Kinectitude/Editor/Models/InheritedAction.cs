using Kinectitude.Editor.Storage;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedAction : AbstractAction
    {
        private readonly AbstractAction inheritedAction;

        public override Plugin Plugin
        {
            get { return inheritedAction.Plugin; }
        }

        public override string Type
        {
            get { return inheritedAction.Type; }
        }

        public override string DisplayName
        {
            get { return inheritedAction.DisplayName; }
        }

        public IEnumerable<object> Tokens { get; private set; }

        public override bool IsLocal
        {
            get { return inheritedAction.IsLocal; }
        }

        public override bool IsInherited
        {
            get { return inheritedAction.IsInherited; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return inheritedAction.Plugins; }
        }

        public InheritedAction(AbstractAction inheritedAction)
        {
            this.inheritedAction = inheritedAction;

            inheritedAction.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            foreach (AbstractProperty inheritedProperty in inheritedAction.Properties)
            {
                InheritedProperty localProperty = new InheritedProperty(inheritedProperty);
                AddProperty(localProperty);
            }

            string[] splitHeader = Regex.Split(inheritedAction.DisplayName, "({.*?})");
            List<object> tokens = new List<object>();

            foreach (string token in splitHeader)
            {
                if (token.StartsWith("{", System.StringComparison.Ordinal))
                {
                    string property = token.TrimStart('{').TrimEnd('}');
                    tokens.Add(GetProperty(property));
                }
                else if (!string.IsNullOrEmpty(token))
                {
                    tokens.Add(token);
                }
            }

            Tokens = tokens;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool InheritsFrom(AbstractAction action)
        {
            return action == inheritedAction;
        }
    }
}
