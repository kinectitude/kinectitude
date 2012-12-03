using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kinectitude.Editor.Models
{
    internal sealed class Header
    {
        public bool IsInherited { get; private set; }

        public IEnumerable<object> Tokens { get; private set; }

        public Header(string header, IEnumerable<AbstractProperty> properties, bool inherited)
        {
            IsInherited = inherited;

            string[] splitHeader = Regex.Split(header, "({.*?})");
            List<object> tokens = new List<object>();

            foreach (string token in splitHeader)
            {
                if (token.StartsWith("{", StringComparison.Ordinal))
                {
                    string property = token.TrimStart('{').TrimEnd('}');
                    tokens.Add(properties.FirstOrDefault(x => x.Name == property));
                }
                else if (!string.IsNullOrEmpty(token))
                {
                    tokens.Add(token);
                }
            }

            Tokens = tokens;
        }
    }
}
