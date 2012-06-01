
namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Define
    {
        private string name;
        private string className;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Class
        {
            get { return className; }
            set { className = value; }
        }

        public Define() { }
    }
}
