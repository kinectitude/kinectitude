using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TestSetInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            HasItemToSet has = new HasItemToSet();
            //base timing 
            DateTime startTime = DateTime.Now;
            for (int i = 0; i < 100000000; i++)
            {
                has.Item = ItemType.getItemType("lol");
            }
            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            Console.WriteLine("Consturctor Info invoke " + duration);

            //setter timing 
            //this happens already with our current method it is not overhead
            PropertyInfo pi = typeof(HasItemToSet).GetProperty("Item");
            startTime = DateTime.Now;
            for (int i = 0; i < 100000000; i++)
            {
                pi.SetValue(has, ItemType.getItemType("lol"), null);
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            Console.WriteLine("Consturctor Info invoke " + duration);
        }
    }
}
