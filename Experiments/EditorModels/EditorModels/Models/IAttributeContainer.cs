using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal interface IAttributeContainer
    {
        IEnumerable<Attribute> Attributes { get; }

        void AddAttribute(Attribute attribute);
        void RemoveAttribute(Attribute attribute);
    }
}
