using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Functions
{
    public static class Math
    {
        private static readonly Random random = new Random();

        [Plugin("Min", "Returns a number representing the minimum of the values")]
        public static double Min(ValueReader arg1, params ValueReader [] args)
        {
            double min = arg1;
            foreach (ValueReader arg in args)
            {
                if (arg < min) min = arg;
            }
            return min;
        }

        [Plugin("Max", "Returns a number representing the maximum of the values")]
        public static double Max(ValueReader arg1, params ValueReader[] args)
        {
            double max = arg1;
            foreach (ValueReader arg in args)
            {
                if (arg > max) max = arg;
            }
            return max;
        }

        [Plugin("Absolute", "Returns the aboslute value of {value}")]
        public static double Abs(ValueReader value)
        {
            double val = value.GetDoubleValue();
            if (val >= 0) return val;
            return -val;
        }

        [Plugin("ACos", "Returns the angle with the cos of the {value}")]
        public static double ACos(ValueReader value) { return System.Math.Acos(value.GetDoubleValue()); }

        [Plugin("ASin", "Returns the angle with the sin of the {value}")]
        public static double ASin(ValueReader value) { return System.Math.Asin(value.GetDoubleValue()); }

        [Plugin("ATan", "Returns the angle with the tan of the {value}")]
        public static double ATan(ValueReader value) { return System.Math.Atan(value.GetDoubleValue()); }

        [Plugin("ATan", "Returns the angle whose tangent is the quotient of {x} an {y}")]
        public static double ATan2(ValueReader x, ValueReader y) { return System.Math.Atan2(x.GetDoubleValue(), y.GetDoubleValue()); }

        [Plugin("Ceiling", "Returns the smallest integer that is bigger or equal to {value}")]
        public static int Ceiling(ValueReader value)
        {
            //will round down the -1 number and add one to it :).
            return 1 + (int)(value.GetIntValue() - 1);
        }

        [Plugin("Cos", "Returns cos of {value}")]
        public static double Cos(ValueReader value) { return System.Math.Cos(value.GetDoubleValue()); }

        [Plugin("Cosh", "Returns hyperbolic cosh of the angle {value}")]
        public static double Cosh(ValueReader value) { return System.Math.Cosh(value.GetDoubleValue()); }

        [Plugin("Floor", "Returns the largest integer that is smaller or equal to {value}")]
        public static int Floor(ValueReader value) { return value.GetIntValue(); }

        [Plugin("Ln", "Returns the Log base e of {value}")]
        public static double Ln(ValueReader value) { return System.Math.Log(value.GetDoubleValue()); }

        [Plugin("Log", "Returns the Log {value} base 10")]
        public static double Log(ValueReader value) { return System.Math.Log10(value.GetDoubleValue()); }

        [Plugin("Log", "Returns the Log of {log}  in base {b}")]
        public static double Log(ValueReader log, ValueReader b) { return System.Math.Log(log.GetDoubleValue(), b.GetDoubleValue()); }

        [Plugin("Round", "Rounds the {value} to the nearest integer")]
        public static int Round(ValueReader value) { return (int)System.Math.Round(value.GetDoubleValue()); }

        [Plugin("Round", "Rounds the {value} to {digits} digits")]
        public static double Round(ValueReader value, ValueReader digits) { return (int)System.Math.Round(value.GetDoubleValue(), digits.GetIntValue()); }

        [Plugin("Sin", "Returns the Sin of {value}")]
        public static double Sin(ValueReader value) { return (int)System.Math.Sin(value.GetDoubleValue()); }

        [Plugin("Sqrt", "Returns the square root of {value}")]
        public static double Sqrt(ValueReader value) { return System.Math.Sqrt(value.GetDoubleValue()); }

        [Plugin("Tan", "Returns the tan of {value}")]
        public static double Tan(ValueReader value) { return System.Math.Tan(value.GetDoubleValue()); }

        [Plugin("Tanh", "Returns the tan of {value}")]
        public static double Tanh(ValueReader value) { return System.Math.Tanh(value.GetDoubleValue()); }

        [Plugin("Truncate", "Returns the tan of {value}")]
        public static int Truncate(ValueReader value) { return (int)System.Math.Truncate(value.GetDoubleValue()); }

        [Plugin("Random", "Returns a random number between 0 and 1")]
        public static double Random() { return random.NextDouble(); }

        [Plugin("Random", "Returns a random number between 0 and {max}")]
        public static double Random(ValueReader max) { return random.NextDouble() * max; }

        [Plugin("Random", "Returns a random number between {min} and {max}")]
        public static double Random(ValueReader min, ValueReader max) { return random.NextDouble() * (max - min) + min; }
    }
}
