
namespace Kinectitude.Editor.Models.Properties
{
    internal abstract class Property<T> : Property
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

    internal abstract class Property
    {
        public static Property CreateProperty(PropertyDescriptor descriptor)
        {
            Property property = null;

            if (null != descriptor)
            {
                if (descriptor.Type == PropertyDescriptor.PropertyType.String)
                {
                    property = new TextProperty(descriptor);
                }
                else if (descriptor.Type == PropertyDescriptor.PropertyType.Integer)
                {
                    property = new IntegerProperty(descriptor);
                }
                else if (descriptor.Type == PropertyDescriptor.PropertyType.Double)
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

        protected Property(PropertyDescriptor descriptor)
        {
            this.descriptor = descriptor;
            inherited = true;
        }

        public abstract bool TryParse(string input);

        public abstract override string ToString();
    }
}
