
namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Attribute
    {
        public static dynamic TryParse(string value)
        {
            int parsedInteger = 0;
            bool successfullyParsed = int.TryParse(value, out parsedInteger);
            if (successfullyParsed)
            {
                return parsedInteger;
            }
            
            double parsedDouble = 0;
            successfullyParsed = double.TryParse(value, out parsedDouble);
            if (successfullyParsed)
            {
                return parsedDouble;
            }
            
            bool parsedBoolean = false;
            successfullyParsed = bool.TryParse(value, out parsedBoolean);
            if (successfullyParsed)
            {
                return parsedBoolean;
            }
            
            return value;
        }

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
            this.value = Attribute.TryParse(value);
        }
    }
}
