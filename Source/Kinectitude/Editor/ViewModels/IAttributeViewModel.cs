using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.ViewModels
{
    public interface IAttributeViewModel
    {
        string Key { get; set; }
        string Value { get; set; }
        bool CanInherit { get; }
        bool IsInherited { get; set; }
        bool IsLocal { get; }

        void AddAttribute();
        void RemoveAttribute();
    }
}
