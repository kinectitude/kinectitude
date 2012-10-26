using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Getters
{
    class GetMethods
    {
        public int A { get; set; }
        public GetMethods Next { get; set; }
        public enum X : int { Test = 10 };
        public X Type { get; set; }
    }
}
