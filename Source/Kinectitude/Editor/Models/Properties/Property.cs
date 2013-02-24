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
        private readonly PluginProperty pluginProperty;
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
                    if (null != val)
                    {
                        val.Scope = null;
                    }

                    val = value;

                    if (null != val)
                    {
                        val.Scope = this;
                    }
                    NotifyPropertyChanged("Value");
                }
            }
        }

        [DependsOn("Value")]
        public override bool HasOwnValue
        {
            get { return null != val; }
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
            DisplayFileChooserCommand = new DelegateCommand(null, parameter =>
            {
                DialogService.ShowFileChooserDialog((result, pathName) =>
                {
                    if (result == true)
                    {
                        string fileName = System.IO.Path.GetFileName(pathName);
                        string targetPath = @Workspace.Instance.Project.Location;
                        string destFile = System.IO.Path.Combine(targetPath, fileName);

                        System.IO.File.Copy(pathName, destFile, true);

                        Value = new Value("\"" + fileName + "\"");
                    }
                }, this.PluginProperty.FileFilter, this.PluginProperty.FileChooserTitle);
            });
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
