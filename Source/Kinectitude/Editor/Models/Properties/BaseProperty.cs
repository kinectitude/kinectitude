using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Properties
{
    public abstract class Property<T> : BaseProperty
    {
        private T value;

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                IsInherited = false;
            }
        }

        protected Property(PropertyDescriptor descriptor) : base(descriptor) { }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public abstract class BaseProperty
    {
        public static BaseProperty CreateProperty(PropertyDescriptor descriptor)
        {
            BaseProperty property = null;

            if (null != descriptor)
            {
                if (descriptor.Type == PropertyDescriptor.PropertyType.Text)
                {
                    property = new TextProperty(descriptor);
                }
                else if (descriptor.Type == PropertyDescriptor.PropertyType.Integer)
                {
                    property = new IntegerProperty(descriptor);
                }
                else if (descriptor.Type == PropertyDescriptor.PropertyType.Real)
                {
                    property = new RealProperty(descriptor);
                }
                else if (descriptor.Type == PropertyDescriptor.PropertyType.Boolean)
                {
                    property = new BooleanProperty(descriptor);
                }
                else if (descriptor.Type == PropertyDescriptor.PropertyType.Enumeration)
                {
                    property = new EnumerationProperty(descriptor);
                }
            }
            return property;
        }

        private PropertyDescriptor descriptor;
        private bool inherited;

        public PropertyDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public bool IsInherited
        {
            get { return inherited; }
            set { inherited = value; }
        }

        protected BaseProperty(PropertyDescriptor descriptor)
        {
            this.descriptor = descriptor;
            inherited = true;
        }

        public abstract bool TryParse(string input);

        public abstract override string ToString();
    }
}
