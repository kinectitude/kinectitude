using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal class Action : AbstractAction
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

        public Action(Plugin plugin)
        {
            if (plugin.Type != PluginType.Action)
            {
                throw new ArgumentException("Plugin is not an action");
            }

            this.plugin = plugin;

            foreach (PluginProperty property in plugin.Properties)
            {
                AddProperty(new Property(property));
            }

            header = new Header(Plugin.Header, Properties, false);

            AddDependency<ScopeChanged>("Type");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
