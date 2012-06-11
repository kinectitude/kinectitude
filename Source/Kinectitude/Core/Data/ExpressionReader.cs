using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Data
{
    internal abstract class ExpressionReader : IExpressionReader
    {

        private static readonly Regex innerBrackets = 
            new Regex(@"(\{[^\}]*\{)|(\}[^\{]*\}) | (\[[^\]]*\[)|(\][^\[]*\])");
        private static readonly Regex bracketMatch = new Regex(@"\{[^\}]*\}|\[[^\]]*\]");

        internal static ExpressionReader CreateExpressionReader(string value, Event evt, Entity entity)
        {
            /*It is hard to think of a rule to match if I don't replace \{ and \} first.
             * Also, this may be more effecient because longer regexs are bad to use
             * A second variable is used, so taht I don't need to replace it again*/
            string str = value.Replace(@"\\", "\f").Replace(@"\{", "\a").Replace(@"\}", "\v")
                .Replace(@"\[", "\x000E").Replace(@"\]", "\x000F");

            MatchCollection matches = innerBrackets.Matches(str);
            if (matches.Count != 0)
            {
                //TODO make this more useful exception
                throw new ArgumentException("{} and [] cannot be nested.");
            }
            matches = bracketMatch.Matches(str);

            string nonmatch;

            if (matches.Count != 0)
            {
                List<ExpressionReader> expressions = new List<ExpressionReader>();
                int last = 0;
                foreach (Match match in matches)
                {
                    nonmatch = str.Substring(last, match.Index - last).Replace("\a", "{")
                        .Replace("\v", "}").Replace("\f", @"\").Replace("\x000E", "[").Replace("\x000F", "]");
                    if("" != nonmatch)
                    {
                        expressions.Add(new ConstantExpressionReader(nonmatch));
                    }
                    last = match.Index + match.Length;
                    string matchStr = match.ToString().Substring(1, match.ToString().Length - 2);
                    if (match.ToString()[0] == '[')
                    {
                        expressions.Add(new EvalExpressionReader(new ExpressionEval(matchStr, evt, entity)));
                    }
                    else
                    {
                        string[] vals = matchStr.Split('.');
                        if ('!' == value[0])
                        {
                            if (evt == null)
                            {
                                throw new IllegalPlacementException("!", "events or actions");
                            }
                        }
                        switch (vals.Length)
                        {
                            case 1:
                                expressions.Add(new ParameterValueReader(evt, matchStr.Substring(1)));
                                break;
                            case 2:
                                expressions.Add(new EntityValueReader
                                    (vals[1], TypeMatcher.CreateTypeMatcher(vals[0], evt, entity), entity));
                                break;
                            case 3:
                                string[] part = new string[] { vals[1], vals[2] };
                                if ("scene" == vals[0])
                                {
                                    expressions.Add(new ManagerValueReader(part, entity.Scene));
                                }
                                else
                                {
                                    expressions.Add(new ComponentValueReader
                                        (part, TypeMatcher.CreateTypeMatcher(vals[0], evt, entity)));
                                }
                                break;
                            default:
                                //TODO make this a better exception
                                throw new ArgumentException("Invalid reader");
                        }
                    }
                }
                nonmatch = str.Substring(last);
                if ("" != nonmatch)
                {
                    expressions.Add(CreateExpressionReader(nonmatch, evt, entity));
                }
                //no need for extra ovehead
                if (expressions.Count == 1)
                {
                    return expressions[0];
                }
                return new MultiValueReader(expressions);
            }

            value = value.Replace("\a", "{").Replace("\v", "}").Replace("\f", @"\");
            return new ConstantExpressionReader(value);
        }

        public abstract string GetValue();

        public abstract void notifyOfChange(Action<string> callback);

    }
}