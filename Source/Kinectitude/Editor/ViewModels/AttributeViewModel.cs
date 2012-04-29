using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Base;

namespace Editor.ViewModels
{
    public class AttributeViewModel : BaseModel
    {
        private readonly BaseAttribute attribute;

        public string Key
        {
            get { return attribute.Key; }
        }

        public string Value
        {
            get { return attribute.StringValue; }
        }

        public AttributeViewModel(BaseAttribute attribute)
        {
            this.attribute = attribute;
        }
    }
}
