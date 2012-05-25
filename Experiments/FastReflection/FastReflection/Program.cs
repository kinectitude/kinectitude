using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using Core;
using System.Diagnostics;
using System.Linq.Expressions;

namespace FastReflection
{
    class Program
    {
        static void Main(string[] args)
        {
            List<object> result = new List<object>();

            // Load the module

            Assembly module = Assembly.LoadFrom("Module.dll");

            foreach (Type type in module.GetExportedTypes())
            {
                ClassFactory.RegisterType(type);
            }
            
            // Parse the XML

            XElement root = XElement.Load("game.xml");

            foreach (XElement element in root.Elements("Entity"))
            {
                Entity entity = new Entity();

                foreach (XElement componentElement in element.Elements("Component"))
                {
                    string name = (string)componentElement.Attribute("Type");
                    Component component = ClassFactory.Deserialize<Component>(componentElement);
                    entity.AddComponent(component);
                }
                result.Add(entity);
            }
        }
    }
}
