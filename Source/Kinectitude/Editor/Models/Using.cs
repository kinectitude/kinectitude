using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal delegate void DefineChangedEventHandler(Define define);

    internal sealed class Using : VisitableModel
    {
        private string file;

        public event DefineAddedEventHandler DefineAdded;
        public event DefineChangedEventHandler DefineChanged;

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

        public ObservableCollection<Define> Defines
        {
            get;
            private set;
        }

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
            define.PropertyChanged += OnDefinePropertyChanged;
            Defines.Add(define);

            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        public void RemoveDefine(Define define)
        {
            define.PropertyChanged -= OnDefinePropertyChanged;
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

        private void OnDefinePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Name")
            {
                if (null != DefineChanged)
                {
                    Define define = sender as Define;
                    DefineChanged(define);
                }
            }
        }
    }
}
