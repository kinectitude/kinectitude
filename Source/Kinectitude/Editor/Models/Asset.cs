
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Storage;
namespace Kinectitude.Editor.Models
{
    class Asset : VisitableModel
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

        public Asset(string fileName)
        {
            FileName = fileName;
        }

        public override void Accept(IGameVisitor visitor)
        {
            
        }
    }
}
