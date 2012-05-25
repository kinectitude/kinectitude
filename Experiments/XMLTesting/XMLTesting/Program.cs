using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace XMLTesting
{
    class Program
    {
        static void Main(string[] args)
        {

            Object c = new CClass1();

            Type componentType = System.Type.GetType("XMLTesting.TestLoad");
            Type[] constructors = { typeof(CClass1)};
            ConstructorInfo ci = componentType.GetConstructor(constructors);
            Object [] objs = {c};
            TestLoad ti = (TestLoad)ci.Invoke(objs);
            try
            {
                Type t = typeof(TestLoad);
                MethodInfo mi = t.GetMethod("test");
                mi.Invoke(ti, objs);
            }
            catch
            {
                Console.WriteLine("What?");
            }



            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Users\\Ryan\\Desktop\\School\\Assignments\\Kinectitude\\Kinectitude Repo\\pong.xml");
            XmlNode root = doc.DocumentElement;
            XmlAttributeCollection attrs = root.Attributes;
            foreach (XmlNode node in root)
            {
                Console.WriteLine(node.Name);
                Console.ReadLine();
            }
        }

    }
}
