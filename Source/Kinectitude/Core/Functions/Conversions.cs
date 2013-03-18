using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Functions
{
    public static class Conversions
    {
        [Plugin("Bool", "Returns the boolean value of {value}")]
        public static bool Bool(ValueReader value) { return value.GetBoolValue(); }

        [Plugin("Number", "Returns the decimal value of {value}")]
        public static double Number(ValueReader value) { return value.GetDoubleValue(); }

        [Plugin("String", "Returns the string value of {value}")]
        public static string String(ValueReader value) { return value.GetStrValue(); }

        [Plugin("Format", "Returns the numeric value {value} formatted as a string.")]
        public static string Format(ValueReader value, ValueReader format) { return value.GetDoubleValue().ToString(format.GetStrValue()); }
    }
}
