using Kinectitude.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
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
