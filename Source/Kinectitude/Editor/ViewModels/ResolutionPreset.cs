
namespace Kinectitude.Editor.ViewModels
{
    internal sealed class ResolutionPreset
    {
        private readonly string name;
        private readonly int width;
        private readonly int height;

        public string Name
        {
            get { return name; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public ResolutionPreset(string name, int width, int height)
        {
            this.name = name;
            this.width = width;
            this.height = height;
        }
    }
}
