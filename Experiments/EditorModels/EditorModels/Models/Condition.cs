using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Condition : Action
    {
        public string If
        {
            get;
            set;
        }

        public Condition() { }
    }
}
