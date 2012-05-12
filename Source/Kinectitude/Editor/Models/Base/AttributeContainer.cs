using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;

namespace Kinectitude.Editor.Models.Base
{
    public class AttributeContainer
    {
        private readonly SortedDictionary<string, Attribute> attributes;

        public IEnumerable<Attribute> Attributes
        {
            get { return attributes.Values; }
        }

        protected AttributeContainer()
        {
            attributes = new SortedDictionary<string, Attribute>();
        }

        public void AddAttribute(Attribute attribute)
        {
            if (attributes.ContainsKey(attribute.Key))
            {
                RemoveAttribute(attributes[attribute.Key]);
            }
            attribute.Parent = this;
            attributes.Add(attribute.Key, attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attribute.Parent = null;
            attributes.Remove(attribute.Key);
        }

        public virtual Attribute GetAttribute(string key)
        {
            Attribute ret = null;
            attributes.TryGetValue(key, out ret);
            return ret;
        }

        public void SetAttribute(string key, dynamic value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes[key] = new Attribute(key, value);
            }
            else
            {
                attributes[key].Value = value;
            }
        }
    }
}
