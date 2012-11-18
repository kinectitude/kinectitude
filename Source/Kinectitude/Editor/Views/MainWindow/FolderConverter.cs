using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Kinectitude.Editor.Views
{
    internal sealed class Folder
    {
        private readonly string name;
        private readonly IEnumerable children;

        public string Name
        {
            get { return name; }
        }

        public IEnumerable Children
        {
            get { return children; }
        }

        public Folder(string name, IEnumerable children)
        {
            this.name = name;
            this.children = children;
        }
    }

    internal sealed class FolderConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<object> result = new List<object>();

            List<string> folderNames = (parameter as string ?? string.Empty).Split(',').Select(x => x.Trim()).ToList();
            while (values.Length > folderNames.Count)
            {
                folderNames.Add(string.Empty);
            }

            for (int i = 0; i < values.Length; i++)
            {
                IEnumerable children = values[i] as IEnumerable ?? new object[] { values[i] };

                if (!string.IsNullOrEmpty(folderNames[i]))
                {
                    Folder folder = new Folder(folderNames[i], children);
                    result.Add(folder);
                }
                else
                {
                    foreach (object obj in children)
                    {
                        result.Add(obj);
                    }
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
