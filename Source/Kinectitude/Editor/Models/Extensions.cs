using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kinectitude.Editor.Models
{
    internal static class Extensions
    {
        public static bool ContainsWhitespace(this string name)
        {
            return null != name && Regex.IsMatch(name, @"\s");
        }
    }
}
