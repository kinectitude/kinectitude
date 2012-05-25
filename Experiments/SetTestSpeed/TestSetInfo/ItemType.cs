using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSetInfo
{
    public class ItemType
    {
        public string Value { get; private set; }
        public readonly int Id;
        private static int on = 0;
        
        
        public static ItemType getItemType(string s)
        {
            return new ItemType(s, on++);
        }

        private ItemType(string s, int id)
        {
            Value = s;
            Id = id;
        }

    }
}
