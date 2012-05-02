﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Models
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
            attributes.Add(attribute.Key, attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attributes.Remove(attribute.Key);
        }

        public virtual Attribute GetAttribute(string key)
        {
            Attribute ret = null;
            if (attributes.ContainsKey(key))
            {
                ret = attributes[key];
            }
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
