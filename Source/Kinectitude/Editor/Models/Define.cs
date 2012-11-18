
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Storage;
namespace Kinectitude.Editor.Models
{
    internal sealed class Define : VisitableModel
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

        public Define(string name, string className)
        {
            this.name = name;
            this.className = className;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
