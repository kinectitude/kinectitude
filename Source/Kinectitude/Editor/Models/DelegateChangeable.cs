using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class DelegateChangeable : IChangeable
    {
        private readonly Func<string, object> func;

        public DelegateChangeable(Func<string, object> func)
        {
            this.func = func;
        }

        object IChangeable.this[string parameter]
        {
            get { return func(parameter); }
        }

        bool IChangeable.ShouldCheck { get; set; }
    }
}
