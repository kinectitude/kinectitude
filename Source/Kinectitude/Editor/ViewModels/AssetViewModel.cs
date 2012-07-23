
namespace Kinectitude.Editor.ViewModels
{
    class AssetViewModel : BaseViewModel
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    string oldName = name;

                    Workspace.Instance.CommandHistory.Log(
                        "rename asset to '" + value + "'",
                        () => Name = value,
                        () => Name = oldName
                    );

                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string FileName
        {
            get;
            private set;
        }

        public AssetViewModel(string fileName)
        {
            FileName = fileName;
        }
    }
}
