using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class MultiValueReader : ExpressionReader
    {
        private readonly List<ExpressionReader> readers;

        internal MultiValueReader(List<ExpressionReader> expressionReaders)
        {
            readers = expressionReaders;
        }


        public override string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ExpressionReader reader in readers)
            {
                sb.Append(reader.GetValue());
            }
            return sb.ToString();
        }

        public override void notifyOfChange(Action<string> callback)
        {
            foreach (ExpressionReader reader in readers)
            {
                reader.notifyOfChange(callback);
            }
        }
    }
}
