
namespace Kinectitude.Editor.ViewModels
{
    class AssetViewModel : BaseViewModel
    {
        private string name;
        private string fileName;

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

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

        public AssetViewModel(string name)
        {
            this.name = name;
        }
    }
}
