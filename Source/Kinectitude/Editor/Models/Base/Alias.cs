using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Base
{
    public class Alias
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

        public Alias() { }
    }
}
