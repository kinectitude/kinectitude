using System;
using System.Collections.Generic;
using System.Linq;
using Kinectitude.Editor.Models.Interfaces;
using System.Text.RegularExpressions;
using Kinectitude.Editor.Base;
using System.Windows.Input;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Models.Notifications;

namespace Kinectitude.Editor.Models
{
    internal sealed class Event : AbstractEvent
    {
        private readonly Plugin plugin;
        private readonly Header header;

        public override Plugin Plugin
        {
            get { return plugin; }
        }

        public override Header Header
        {
            get { return header; }
        }

        public ICommand AddStatementCommand { get; private set; }

        public Event(Plugin plugin)
        {
            if (plugin.Type != PluginType.Event)
            {
                throw new ArgumentException("Plugin is not an event");
            }

            this.plugin = plugin;

            foreach (PluginProperty property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }

            header = new Header(Plugin.Header, Properties, false);

            AddStatementCommand = new DelegateCommand(null, (parameter) =>
            {
                StatementFactory factory = parameter as StatementFactory;
                if (null != factory)
                {
                    AddStatement(factory.CreateStatement());
                }
            });

            AddDependency<ScopeChanged>("Type");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
