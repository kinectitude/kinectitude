using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace EditorModels.ViewModels
{
    internal delegate void DefineChangedEventHandler(DefineViewModel define);

    internal sealed class UsingViewModel : BaseViewModel
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

        public ObservableCollection<DefineViewModel> Defines
        {
            get;
            private set;
        }

        public UsingViewModel()
        {
            Defines = new ObservableCollection<DefineViewModel>();
        }

        public void AddDefine(DefineViewModel define)
        {
            define.PropertyChanged += OnDefinePropertyChanged;
            Defines.Add(define);

            if (null != DefineAdded)
            {
                DefineAdded(define);
            }
        }

        public void RemoveDefine(DefineViewModel define)
        {
            define.PropertyChanged -= OnDefinePropertyChanged;
            Defines.Remove(define);
        }

        public DefineViewModel GetDefineByName(string name)
        {
            return Defines.FirstOrDefault(x => x.Name == name);
        }

        public DefineViewModel GetDefineByClass(string name)
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
                    DefineViewModel define = sender as DefineViewModel;
                    DefineChanged(define);
                }
            }
        }
    }
}
