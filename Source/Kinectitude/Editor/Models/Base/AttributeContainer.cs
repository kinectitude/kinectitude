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
        private readonly List<Attribute> attributes;

        public IEnumerable<Attribute> Attributes
        {
            get { return attributes; }
        }

        protected AttributeContainer()
        {
            attributes = new List<Attribute>();
        }

        public void AddAttribute(Attribute attribute)
        {
            Attribute existing = attributes.FirstOrDefault(x => x.Key == attribute.Key);

            if (null == existing)
            {
                attribute.Parent = this;
                attributes.Add(attribute);
            }
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attribute.Parent = null;
            attributes.Remove(attribute);
        }

        public virtual Attribute GetAttribute(string key)
        {
            return attributes.FirstOrDefault(x => x.Key == key);
        }

        public void SetAttribute(string key, dynamic value)
        {
            Attribute attribute = GetAttribute(key);

            if (null == attribute)
            {
                attributes.Add(new Attribute(key, value));
            }
            else
            {
                attribute.Value = value;
            }
        }
    }
}
