//-----------------------------------------------------------------------
// <copyright file="Event.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
                var statement = parameter as AbstractStatement;
                if (null == statement)
                {
                    var factory = parameter as StatementFactory;
                    if (null != factory)
                    {
                        statement = factory.CreateStatement();
                    }
                }

                if (null != statement)
                {
                    int oldIdx = -1;
                    var oldParent = statement.Scope;
                    if (null != oldParent)
                    {
                        oldIdx = oldParent.IndexOf(statement);
                    }

                    AddStatement(statement);

                    Workspace.Instance.CommandHistory.Log(
                        "insert/move statement",
                        () => AddStatement(statement),
                        () =>
                        {
                            statement.RemoveFromParent();
                            if (null != oldParent)
                            {
                                oldParent.InsertAt(oldIdx, statement);
                            }
                        }
                    );
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
