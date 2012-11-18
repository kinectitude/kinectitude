using Kinectitude.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal abstract class Value : BaseModel
    {
        public Value() { }

        public abstract string GetValue();
        public abstract void SetValue(string value);
    }
}
