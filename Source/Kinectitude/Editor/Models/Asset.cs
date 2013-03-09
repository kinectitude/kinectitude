using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Models
{
    class Asset : BaseModel
    {
        public string File { get; private set; }

        public Asset(string file)
        {
            File = file;
        }
    }
}
