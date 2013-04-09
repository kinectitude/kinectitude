//-----------------------------------------------------------------------
// <copyright file="Property.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Data;
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

            public void Prepare() { }

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

                        if (!Workspace.Instance.Project.HasAssetWithFileName(fileName))
                        {
                            Notify(new AssetUsed(pathName));
                            Value = new Value("\"" + fileName + "\"");
                        }
                        else
                        {
                            Workspace.Instance.DialogService.Warn("Asset Already Exists", "This project already contains an asset named " + fileName + ".", System.Windows.MessageBoxButton.OK);
                        }
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
