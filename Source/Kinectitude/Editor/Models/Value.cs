using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class Value
    {
        private readonly string initializer;
        private readonly ValueReader reader;

        public string Initializer
        {
            get { return initializer; }
        }

        public ValueReader Reader
        {
            get { return reader; }
        }

        public Value(string initializer, ValueReader reader)
        {
            this.initializer = initializer;
            this.reader = reader;
        }

        //public override string ToString()
        //{
        //    return Initializer;
        //}
    }
}
