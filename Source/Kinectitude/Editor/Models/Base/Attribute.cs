using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Base
{
    public class Attribute
    {
        private AttributeContainer parent;
        private string key;
        private dynamic value;

        public AttributeContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public dynamic Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Attribute(string key, dynamic value)
        {
            this.key = key;
            this.value = value;
        }

        public Attribute(string key, string value)
        {
            this.key = key;

            int parsedInteger = 0;
            bool successfullyParsed = int.TryParse(value, out parsedInteger);
            if (successfullyParsed)
            {
                this.value = parsedInteger;
            }
            else
            {
                double parsedDouble = 0;
                successfullyParsed = double.TryParse(value, out parsedDouble);
                if (successfullyParsed)
                {
                    this.value = parsedDouble;
                }
                else
                {
                    bool parsedBoolean = false;
                    successfullyParsed = bool.TryParse(value, out parsedBoolean);
                    if (successfullyParsed)
                    {
                        this.value = parsedBoolean;
                    }
                    else
                    {
                        this.value = value;
                    }
                }
            }
        }
    }
}
