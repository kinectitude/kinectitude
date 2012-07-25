using System;
using Kinectitude.Core.Base;
using NCalc;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    internal class ExpressionEval : IChangeNotify
    {
        private Expression expression;

        private readonly List<Action<string>> callbacks = new List<Action<string>>();
        private readonly Dictionary<string, ExpressionReader> expressions = new Dictionary<string, ExpressionReader>();
        private bool isUpdateReady = false;

        internal ExpressionEval(string expressionStr, Event evt, Entity entity)
        {

            expressionStr = expressionStr.Replace('.', '_').Replace("s:", "_").Replace("!","__").Replace("$","___");
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
                    case "random":
                    case "rnd":
                        Random r = new Random();
                        args.Result = (double)r.Next(1000) / 1000;
                        return;
                }

                if(name.StartsWith("__"))
                {
                    name = "!" + name.Substring(2);
                }
                else if (name.StartsWith("___"))
                {
                    name = "$" + name.Substring(3);
                }

                bool stringLookup = false;
                if (name[0] == '_')
                {
                    name = expressionStr.Substring(1);
                    stringLookup = true;
                }
                name = name.Replace('_', '.');
                string value;
                if (expressions.ContainsKey(name))
                {
                    value = expressions[name].GetValue();
                }
                else
                {
                    ExpressionReader reader = ExpressionReader.CreateExpressionReader("{" + name + "}", evt, entity);
                    expressions.Add(name, reader);
                    value = reader.GetValue();
                }
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
            isUpdateReady = true;
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
            isUpdateReady = true;
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
            isUpdateReady = true;
            return (string)Convert.ChangeType(result, typeof(string));
        }

        private void changeOccured(string change)
        {
            foreach (Action<string> callback in callbacks)
            {
                callback(change);
            }
        }

        public void notifyOfChange(Action<string> callback)
        {
            callbacks.Add(callback);
            if (!isUpdateReady)
            {
                expression.Evaluate();
                isUpdateReady = true;
            }
            foreach(KeyValuePair<string, ExpressionReader> expressionPair in expressions)
            {
                expressionPair.Value.notifyOfChange(changeOccured);
            }
        }
    }
}
