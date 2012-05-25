using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace PlaingDynamic
{
    class Entity : DynamicObject
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        static void Main(string[] args)
        {
            /*dynamic d = new Program();
            d.test = "hello";
            Console.Out.WriteLine(d.test);            */

            System.IO.StreamReader str = new System.IO.StreamReader("C:\\Users\\Ryan\\Desktop\\Kinectitude\\person.xml");
            //System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(BClass));

            dynamic entity = new Entity();
            Entity e2 = new Entity();
            try
            {
                Console.Out.WriteLine(res.MI);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                res = null;
            }
            
            Console.In.ReadLine();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}
