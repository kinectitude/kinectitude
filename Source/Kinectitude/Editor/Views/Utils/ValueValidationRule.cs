using Kinectitude.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Kinectitude.Editor.Views.Utils
{
    internal sealed class ValueValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var str = value.ToString();

            if (Workspace.ValueMaker.HasErrors(str))
            {
                return new ValidationResult(false, string.Format("'{0}' is not a valid expression.", str));
            }

            return new ValidationResult(true, null);
        }
    }
}
