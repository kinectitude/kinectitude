using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal class Action : IPropertyContainer
    {
        private readonly List<Property> properties;

        public string Type
        {
            get;
            set;
        }

        public IEnumerable<Property> Properties
        {
            get { return properties; }
        }

        public Action()
        {
            properties = new List<Property>();
        }

        public void AddProperty(Property property)
        {
            properties.Add(property);
        }

        public void RemoveProperty(Property property)
        {
            properties.Remove(property);
        }
    }
}
