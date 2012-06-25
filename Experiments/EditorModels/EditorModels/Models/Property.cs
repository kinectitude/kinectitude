using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Property
    {
        public string Name
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public Property() { }
    }
}
