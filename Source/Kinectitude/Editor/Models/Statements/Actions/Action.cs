//-----------------------------------------------------------------------
// <copyright file="Action.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Storage;
using System;

namespace Kinectitude.Editor.Models.Statements.Actions
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

            header = new Header(Plugin.Header, Properties, true);

            AddDependency<ScopeChanged>("Type");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
