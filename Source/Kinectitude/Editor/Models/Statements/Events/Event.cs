using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Storage;
using System;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Statements.Events
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

            header = new Header(Plugin.Header, Properties, true);

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
