using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kinectitude.Editor.Models
{
    internal sealed class Using : GameModel<IScope>
    {
        private string file;

        public string File
        {
            get { return file; }
            set
            {
                if (file != value)
                {
                    file = value;
                    NotifyPropertyChanged("File");
                }
            }
        }

        public ObservableCollection<Define> Defines { get; private set; }

        public Using()
        {
            Defines = new ObservableCollection<Define>();
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void AddDefine(Define define)
        {
            define.Scope = this;
            Defines.Add(define);

            Broadcast(new DefineAdded(define));
        }

        public void RemoveDefine(Define define)
        {
            define.Scope = null;
            Defines.Remove(define);
        }

        public Define GetDefineByName(string name)
        {
            return Defines.FirstOrDefault(x => x.Name == name);
        }

        public Define GetDefineByClass(string name)
        {
            return Defines.FirstOrDefault(x => x.Class == name);
        }

        public bool HasDefineWithName(string name)
        {
            return Defines.Any(x => x.Name == name);
        }

        public bool HasDefineWithClass(string name)
        {
            return Defines.Any(x => x.Class == name);
        }
    }
}
