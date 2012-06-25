using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Event : IPropertyContainer
    {
        private readonly List<Property> properties;
        private readonly List<Action> actions;

        public string Type
        {
            get;
            set;
        }

        public IEnumerable<Property> Properties
        {
            get { return properties; }
        }

        public IEnumerable<Action> Actions
        {
            get { return actions; }
        }

        public Event()
        {
            properties = new List<Property>();
            actions = new List<Action>();
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
