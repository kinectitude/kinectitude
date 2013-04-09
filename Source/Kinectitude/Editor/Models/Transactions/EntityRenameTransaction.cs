//-----------------------------------------------------------------------
// <copyright file="EntityRenameTransaction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Base;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Transactions
{
    internal sealed class EntityRenameTransaction : BaseModel
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Title
        {
            get { return "Rename Entity"; }
        }

        public ICommand CommitCommand { get; private set; }

        public EntityRenameTransaction(Entity entity)
        {
            Name = entity.Name;

            CommitCommand = new DelegateCommand(null, (parameter) =>
            {
                entity.Name = Name;
            });
        }
    }
}
