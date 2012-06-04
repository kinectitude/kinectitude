using System;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    internal abstract class ExpressionReader : IExpressionReader
    {

        private static readonly Regex innerBrackets = new Regex(@"(\{[^\}]*\{)|(\}[^\{]*\})");
        private static readonly Regex bracketMatch = new Regex(@"\{[^\}]*\}");

        internal static ExpressionReader CreateExpressionReader(string value, Event evt, Entity entity)
        {
            /*It is hard to think of a rule to match if I don't replace \{ and \} first.
             * Also, this may be more effecient because longer regexs are bad to use
             * A second variable is used, so taht I don't need to replace it again*/
            string str = value.Replace(@"\\", "\f").Replace(@"\{", "\a").Replace(@"\}", "\v");
            MatchCollection matches = innerBrackets.Matches(str);
            if (matches.Count != 0)
            {
                //TODO make this more useful exception
                throw new ArgumentException("{} cannot be nesteded in other {}");
            }
            matches = bracketMatch.Matches(str);
            string nonmatch;

            if (matches.Count != 0)
            {
                List<ExpressionReader> expressions = new List<ExpressionReader>();
                int last = 0;
                foreach (Match match in matches)
                {
                    string matchVal = match.ToString().Substring(1, match.Length - 2);
                    nonmatch = str.Substring(last, match.Index - last);
                    if("" != nonmatch)
                    {
                        expressions.Add(CreateExpressionReader(nonmatch, evt, entity));
                    }
                    last = match.Index + match.Length;
                    expressions.Add(CreateExpressionReader(matchVal, evt, entity));
                }
                nonmatch = str.Substring(last);
                if ("" != nonmatch)
                {
                    expressions.Add(CreateExpressionReader(nonmatch, evt, entity));
                }
                return new MultiValueReader(expressions);
            }
            double d;
            if (value.Contains('\\') || '!' != value[0] && !value.Contains('.') || double.TryParse(value, out d))
            {
                value = value.Replace(@"\!", "!").Replace(@"\.", ".").Replace("\a", "{")
                    .Replace("\v", "}").Replace("\f", @"\");
                return new ConstantExpressionReader(value);
            }
            string[] vals = value.Split('.');
            if ('!' == value[0])
            {
                if (evt == null)
                {
                    throw new IllegalPlacementException("!", "events or actions");
                }
            }
            switch(vals.Length)
            {
                case 1:
                    return new ParameterValueReader(evt, value.Substring(1));
                case 2:
                    return new EntityValueReader(vals[1], TypeMatcher.CreateTypeMatcher(vals[0], evt, entity), entity);
                case 3:
                    string[] part = new string [] { vals[1], vals[2] };
                    return new ComponentValueReader(part, TypeMatcher.CreateTypeMatcher(vals[0], evt, entity));
                default:
                    //TODO make this a better exception
                    throw new ArgumentException("Invalid reader");
            }

        }

        public abstract string GetValue();

        public abstract void notifyOfChange(Action<string> callback);

    }
}