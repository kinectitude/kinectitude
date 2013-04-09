//-----------------------------------------------------------------------
// <copyright file="Define.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class Define : GameModel<IScope>
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    var oldName = name;
                    name = value;

                    Broadcast(new DefinedNameChanged(oldName, Workspace.Instance.GetPlugin(Class)));
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Class { get; private set; }

        public Define(string name, string className)
        {
            this.name = name;
            Class = className;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
