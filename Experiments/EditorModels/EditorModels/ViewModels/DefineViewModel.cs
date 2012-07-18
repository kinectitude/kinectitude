
namespace EditorModels.ViewModels
{
    internal sealed class DefineViewModel : BaseViewModel
    {
        private string name;
        private string className;

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

        public string Class
        {
            get { return className; }
            set
            {
                if (className != value)
                {
                    className = value;
                    NotifyPropertyChanged("Class");
                }
            }
        }

        public DefineViewModel(string name, string className)
        {
            this.name = name;
            this.className = className;
        }
    }
}
