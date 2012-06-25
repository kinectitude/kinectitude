using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal interface IPropertyContainer
    {
        IEnumerable<Property> Properties { get; }

        void AddProperty(Property property);
        void RemoveProperty(Property property);
    }
}
