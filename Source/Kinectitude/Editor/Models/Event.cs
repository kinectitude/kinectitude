using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.Models.Interfaces;
using System.Text.RegularExpressions;
using Kinectitude.Editor.Base;
using System.Windows.Input;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class Event : AbstractEvent
    {
        private readonly Plugin plugin;

        public override event DefineAddedEventHandler DefineAdded;
        public override event DefinedNameChangedEventHandler DefineChanged;

        public override Plugin Plugin
        {
            get { return plugin; }
        }

        [DependsOn("Scope")]
        public override string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public override string Header
        {
            get { return plugin.Header; }
        }

        public IEnumerable<object> Tokens { get; private set; }

        public override string Description
        {
            get { return plugin.Description; }
        }

        [DependsOn("IsInherited")]
        public override bool IsLocal
        {
            get { return !IsInherited; }
        }

        public override bool IsInherited
        {
            get { return false; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(plugin, 1)).Distinct(); }
        }

        public ICommand AddActionCommand { get; private set; }

        public Event(Plugin plugin)
        {
            if (plugin.Type != PluginType.Event)
            {
                throw new ArgumentException("Plugin is not an event");
            }

            this.plugin = plugin;

            foreach (string property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }

            string[] splitHeader = Regex.Split(plugin.Header, "({.*?})");
            List<object> tokens = new List<object>();

            foreach (string token in splitHeader)
            {
                if (token.StartsWith("{", StringComparison.Ordinal))
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

        public override bool InheritsFrom(AbstractEvent evt)
        {
            return false;
        }
    }
}
