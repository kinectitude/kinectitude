using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.ViewModels
{
    public class ResolutionPreset
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
