using Kinectitude.Editor.Models.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kinectitude.Editor.Models.Statements.Base
{
    internal sealed class HeaderToken
    {
        public string Text { get; private set; }

        public HeaderToken(string text)
        {
            Text = text;
        }
    }

    internal sealed class Header
    {
        public bool IsEditable { get; private set; }

        public IEnumerable<object> Tokens { get; private set; }

        public Header(string header, IEnumerable<AbstractProperty> properties, bool editable)
        {
            IsEditable = editable;

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
                    tokens.Add(new HeaderToken(token));
                }
            }

            Tokens = tokens.ToArray();
        }
    }
}
