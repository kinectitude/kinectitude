using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Using
    {
        private readonly List<Define> defines;

        public string File
        {
            get;
            set;
        }

        public IEnumerable<Define> Defines
        {
            get { return defines; }
        }

        public Using()
        {
            defines = new List<Define>();
        }

        public void AddDefine(Define define)
        {
            defines.Add(define);
        }

        public void RemoveDefine(Define define)
        {
            defines.Remove(define);
        }
    }
}
