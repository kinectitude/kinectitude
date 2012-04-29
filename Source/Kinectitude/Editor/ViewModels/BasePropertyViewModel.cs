using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.ViewModels
{
    public class BasePropertyViewModel
    {
        private static readonly Dictionary<BaseProperty, BasePropertyViewModel> viewModels;

        static BasePropertyViewModel()
        {
            viewModels = new Dictionary<BaseProperty, BasePropertyViewModel>();
        }

        public static BasePropertyViewModel Create(BaseProperty property)
        {
            if (!viewModels.ContainsKey(property))
            {
                BasePropertyViewModel viewModel = new BasePropertyViewModel(property);
                viewModels[property] = viewModel;
            }
            return viewModels[property];
        }

        private readonly BaseProperty property;

        public BaseProperty Property
        {
            get { return property; }
        }

        public string Key
        {
            get { return property.Descriptor.Key; }
        }

        private BasePropertyViewModel(BaseProperty property)
        {
            this.property = property;
        }
    }
}
