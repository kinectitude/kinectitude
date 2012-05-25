using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.LoadFrom("D:\\School\\Assignments\\Kinectitude\\Reflection Speed Testing\\Application\\TestLib\\bin\\Release\\TestLib.dll");
            Type componentType = assembly.GetType("TestLib.Class1");
            ConstructorInfo ci = componentType.GetConstructor(new Type[] { });
            DateTime startTime = DateTime.Now;
            for (int i = 0; i < 100000000; i++)
            {
                Interface1 created = (Interface1)ci.Invoke(new Object[] { });
            }
            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            Console.WriteLine("Consturctor Info invoke " + duration);
            
            startTime = DateTime.Now;
            for (int i = 0; i < 100000000; i++)
            {
                Interface1 created = (Interface1)Activator.CreateInstance(componentType);
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            Console.WriteLine("Activator Create Instance " + duration);

            startTime = DateTime.Now;
            for (int i = 0; i < 100000000; i++)
            {
                Interface1 created = (Interface1)Activator.CreateInstance(componentType, new Object[]{});
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            Console.WriteLine("Activator Create Instance with args " + duration);

            Console.ReadLine();
        }
    }
}
