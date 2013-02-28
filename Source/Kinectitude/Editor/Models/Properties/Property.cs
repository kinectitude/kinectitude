﻿using Kinectitude.Core.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kinectitude.Editor.Views.Utils;

namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class Property : AbstractProperty, IValueScope
    {
        private sealed class DataListener : IChanges
        {
            private readonly Property property;

            public DataListener(Property property)
            {
                this.property = property;
            }

            public void Prepare()
            {
                // TODO confirm I can leave this empty
            }

            public void Change()
            {
                property.NotifyEffectiveValueChanged();
            }
        }

        private readonly PluginProperty pluginProperty;
        private readonly DataListener callback;
        private Value val;

        public override PluginProperty PluginProperty
        {
            get { return pluginProperty; }
        }

        public override Value Value
        {
            get { return val ?? GetInheritedValue(); }
            set
            {
                if (val != value)
                {
                    var oldVal = val;

                    if (null != val)
                    {
                        val.Scope = null;
                        val.Unsubscribe(callback);
                    }

                    Workspace.Instance.CommandHistory.Log(
                        "change property value",
                        () => Value = value,
                        () => Value = oldVal
                    );

                    val = value;

                    if (null != val)
                    {
                        val.Scope = this;
                        val.Subscribe(callback);
                    }

                    NotifyEffectiveValueChanged();
                    NotifyPropertyChanged("Value");
                }
            }
        }

        [DependsOn("Value")]
        public override bool HasOwnValue
        {
            get { return null != val; }
        }

        [DependsOn("Value")]
        public bool HasOwnOrInheritedValue
        {
            get { return HasOwnValue || InheritsNonDefaultValue(); }
        }

        public override IEnumerable<object> AvailableValues
        {
            get { return PluginProperty.AvailableValues; }
        }

        public override ICommand ClearValueCommand { get; protected set; }

        public ICommand DisplayFileChooserCommand { get; protected set; }

        public Property(PluginProperty pluginProperty)
        {
            this.pluginProperty = pluginProperty;

            ClearValueCommand = new DelegateCommand(parameter => HasOwnValue, parameter => Value = null);
            callback = new DataListener(this);
            DisplayFileChooserCommand = new DelegateCommand(null, parameter =>
            {
                DialogService.ShowFileChooserDialog((result, pathName) =>
                {
                    if (result == true)
                    {
                        string fileName = System.IO.Path.GetFileName(pathName);
                        string localProject = System.IO.Path.Combine(Workspace.Instance.Project.Location, Workspace.Instance.Project.GameRoot);
                        string targetPath = @localProject;
                        string destFile = System.IO.Path.Combine(targetPath, fileName);

                        if (!System.IO.File.Exists(destFile))
                        {
                            System.IO.File.Copy(pathName, destFile, true);
                        }

                        Value = new Value("\"" + fileName + "\"");
                    }
                }, this.PluginProperty.FileFilter, this.PluginProperty.FileChooserTitle);
            });
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private bool InheritsNonDefaultValue()
        {
            return null != Scope ? Scope.HasInheritedNonDefaultValue(PluginProperty) : false;
        }
    }
}
