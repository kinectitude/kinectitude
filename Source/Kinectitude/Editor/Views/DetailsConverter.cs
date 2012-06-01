using System;
using System.Collections.Generic;
using System.Windows.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Views
{
    internal sealed class Detail
    {
        private readonly string name;
        private readonly object value;

        public string Name
        {
            get { return name; }
        }

        public object Value
        {
            get { return value; }
        }

        public Detail(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }

    internal sealed class Details : BaseModel
    {
        private readonly string header;
        private readonly IEnumerable<Detail> items;

        public string Header
        {
            get { return header; }
        }

        public IEnumerable<Detail> Items
        {
            get { return items; }
        }

        public Details(string header, IEnumerable<Detail> items)
        {
            this.header = header;
            this.items = items;
        }
    }

    internal sealed class DetailsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<object> result = new List<object>();

            foreach (object obj in values)
            {
                ModelCollection<AttributeViewModel> attributes = obj as ModelCollection<AttributeViewModel>;
                if (null != attributes)
                {
                    List<Detail> details = new List<Detail>();

                    foreach (AttributeViewModel attribute in attributes)
                    {
                        details.Add(new Detail(attribute.Key, attribute.Value));
                    }

                    result.Add(new Details("Attributes", details));
                }
                else
                {
                    ModelCollection<ComponentViewModel> components = obj as ModelCollection<ComponentViewModel>;
                    if (null != components)
                    {
                        foreach (ComponentViewModel component in components)
                        {
                            List<Detail> properties = new List<Detail>();

                            result.Add(new Details(component.Name, properties));
                        }
                    }
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
