using System;
using Kinectitude.Core.Base;
using NCalc;

namespace Kinectitude.Core.Data
{
    internal class ExpressionEval : IChangeNotify
    {
        private Expression expression;

        internal ExpressionEval(string expressionStr, Event evt, Entity entity)
        {
            expressionStr = expressionStr.Replace('.', '_').Replace("b:", "_");
            Expression expression = new Expression(expressionStr);

            //TODO check if sin and stuff is already in expression, if not put it there.
            //TODO random (decide if funciton or word)

            expression.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                switch (name)
                {
                    case "pi":
                        args.Result = Math.PI;
                        return;
                    case "e":
                        args.Result = Math.E;
                        return;
                    case "True":
                        args.Result = 1;
                        return;
                    case "False":
                        args.Result = 0;
                        return;
                }

                bool stringLookup = false;
                if (name[0] == '_')
                {
                    name = expressionStr.Substring(1);
                    stringLookup = true;
                }
                name = name.Replace('_', '.');
                string value = ExpressionReader.CreateExpressionReader("{" + name + "}", evt, entity).GetValue();
                if(stringLookup)
                {
                    args.Result = value;
                    return;
                }
                if (null == value)
                {
                    args.Result = 0;
                    return;
                }
                double val = 0;
                if (!double.TryParse(value, out val))
                {
                    args.Result = value.ToLower() == "true" ? 1 : 0;
                    return;
                }
                args.Result = val;

            };
            this.expression = expression;

        }

        internal T ToNumber<T>() where T : struct
        {
            object result = expression.Evaluate();
            if (typeof(string) == result.GetType())
            {
                int i = 0;
                int.TryParse(result as string, out i);
                return (T)Convert.ChangeType(i, typeof(T));
            }
            if (typeof(bool) == result.GetType())
            {
                int i = (bool)result ? 1 : 0;
                return (T)Convert.ChangeType(i, typeof(T));
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }

        internal bool ToBool()
        {
            object result = expression.Evaluate();
            Type type = result.GetType();
            if (typeof(bool) == type)
            {
                return (bool)result;
            }
            if (typeof(string) == type)
            {
                string strResult = result as string;
                return strResult.ToLower() == "true";
            }
            return 0 != (double)Convert.ChangeType(result, typeof(double));
        }

        internal string ToStr()
        {
            object result = expression.Evaluate();
            return (string)Convert.ChangeType(result, typeof(string));
        }

        public void notifyOfChange(Action<string> callback)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
