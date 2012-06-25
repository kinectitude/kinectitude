using System.ComponentModel;

namespace EditorModels.ViewModels
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(params string[] names)
        {
            if (null != PropertyChanged)
            {
                foreach (string name in names)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
