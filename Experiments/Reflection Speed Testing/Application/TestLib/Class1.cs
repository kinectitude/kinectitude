using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestLib
{
    public class Class1:Application.Interface1
    {
        public int Value { get; set; }
        public Class1() { }

        public void stop()
        {
            return;
        }
    }
}
