using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Base;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.ViewModels
{
    public class AttributeViewModel : BaseModel
    {
        private readonly Attribute attribute;

        public string Key
        {
            get { return attribute.Key; }
        }

        public dynamic Value
        {
            get { return attribute.Value; }
        }

        public AttributeViewModel(Attribute attribute)
        {
            this.attribute = attribute;
        }
    }
}
