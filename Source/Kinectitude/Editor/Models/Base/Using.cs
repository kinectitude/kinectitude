using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Using
    {
        private string file;
        private readonly SortedDictionary<string, Define> defines;

        public string File
        {
            get { return file; }
            set { file = value; }
        }

        public IEnumerable<Define> Defines
        {
            get { return defines.Values; }
        }

        public Using()
        {
            defines = new SortedDictionary<string, Define>();
        }

        public void AddDefine(Define define)
        {
            defines.Add(define.Name, define);
        }

        public void RemoveDefine(Define define)
        {
            defines.Remove(define.Name);
        }

        public Define GetDefine(string name)
        {
            Define ret = null;
            defines.TryGetValue(name, out ret);
            return ret;
        }
    }
}
