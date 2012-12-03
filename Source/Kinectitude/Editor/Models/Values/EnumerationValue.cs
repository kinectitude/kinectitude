using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Values
{
    internal sealed class EnumerationValue : Value
    {
        public IEnumerable<string> PossibleValues { get; private set; }

        public EnumerationValue(Type type)
        {
            PossibleValues = Enum.GetNames(type);
        }
    }
}
