using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Properties;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class PropertyViewModel : BaseModel
    {
        private readonly Entity entity;
        private readonly PropertyDescriptor descriptor;
        private PropertyViewModel inheritedViewModel;
        private Property property;

        public Property Property
        {
            get { return property; }
        }

        public string Name
        {
            get { return descriptor.DisplayName; }
        }

        public bool IsInherited
        {
            get { return null != inheritedViewModel && property == inheritedViewModel.Property; }
            set
            {

            }
        }

        public object Value
        {
            get { return property; }
            set
            {
                /*if (property.
                bool success = property.TryParse(value);
                if (success)
                {
                    RaisePropertyChanged("Value");
                }*/
            }
        }

        public PropertyViewModel(Entity entity, PropertyDescriptor descriptor)
        {
            this.entity = entity;
            this.descriptor = descriptor;
        }
    }
}
