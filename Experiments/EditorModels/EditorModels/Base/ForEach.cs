using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Base
{
    internal static class ExtensionMethods
    {
        public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
            foreach (T element in instance)
            {
                action(element);
            }
        }
    }
}
