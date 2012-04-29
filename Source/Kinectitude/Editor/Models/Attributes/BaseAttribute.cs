using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public abstract class Attribute<T> : BaseAttribute
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

        protected Attribute(string key) : base(key) { }
    }

    public abstract class BaseAttribute
    {
        public static BaseAttribute CreateAttribute(string key, string value)
        {
            int parsedInteger = 0;
            bool successfullyParsed = int.TryParse(value, out parsedInteger);
            if (successfullyParsed)
            {
                IntegerAttribute integerAttribute = new IntegerAttribute(key);
                integerAttribute.Value = parsedInteger;
                return integerAttribute;
            }

            double parsedDouble = 0;
            successfullyParsed = double.TryParse(value, out parsedDouble);
            if (successfullyParsed)
            {
                RealAttribute realAttribute = new RealAttribute(key);
                realAttribute.Value = parsedDouble;
                return realAttribute;
            }

            bool parsedBoolean = false;
            successfullyParsed = bool.TryParse(value, out parsedBoolean);
            if (successfullyParsed)
            {
                BooleanAttribute booleanAttribute = new BooleanAttribute(key);
                booleanAttribute.Value = parsedBoolean;
                return booleanAttribute;
            }

            TextAttribute textAttribute = new TextAttribute(key);
            textAttribute.Value = value;
            return textAttribute;
        }

        private AttributeContainer parent;
        private string key;
        private bool locked;
        private bool inherited;

        public AttributeContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string Key
        {
            get { return key; }
            set
            {
                if (!IsLocked)
                {
                    key = value;    // TODO: Renaming a key should rename all references to it and display a notification that that's what happens
                }
            }
        }

        public abstract string StringValue { get; }

        public bool IsLocked
        {
            get { return locked; }
            set { locked = value; }
        }

        public bool IsInherited
        {
            get { return inherited; }
            set { inherited = value; }
        }
        
        protected BaseAttribute(string key)
        {
            this.key = key;
            IsLocked = false;
            IsInherited = true;
        }
    }
}
