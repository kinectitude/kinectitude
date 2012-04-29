using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Editor.Base;

namespace Editor
{
    public class AttributeContainer
    {
        private readonly List<BaseAttribute> _attributes;
        private readonly ReadOnlyCollection<BaseAttribute> attributes;

        public ReadOnlyCollection<BaseAttribute> Attributes
        {
            get { return attributes; }
        }

        protected AttributeContainer()
        {
            _attributes = new List<BaseAttribute>();
            attributes = new ReadOnlyCollection<BaseAttribute>(_attributes);
        }

        public void AddAttribute(BaseAttribute attribute)
        {
            if (!_attributes.Contains(attribute))
            {
                BaseAttribute found = _attributes.FirstOrDefault(x => x.Key == attribute.Key);
                if (null != found)
                {
                    RemoveAttribute(found);
                }
                attribute.Parent = this;
                _attributes.Add(attribute);
            }
        }

        public void RemoveAttribute(BaseAttribute attribute)
        {
            if (!attribute.IsLocked)
            {
                attribute.Parent = null;
                _attributes.Remove(attribute);
            }
        }

        public virtual T GetAttribute<T>(string key) where T : BaseAttribute
        {
            BaseAttribute attribute = _attributes.FirstOrDefault(x => x.Key == key);
            return attribute as T;
        }
    }
}
