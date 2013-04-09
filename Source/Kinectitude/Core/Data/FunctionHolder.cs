//-----------------------------------------------------------------------
// <copyright file="FunctionHolder.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class FunctionHolder
    {
        private static readonly Dictionary<string, FunctionHolder> Holders = new Dictionary<string, FunctionHolder>();

        private readonly Dictionary<int, Tuple<Func<ValueReader[], object>, Type>> exact = new Dictionary<int, Tuple<Func<ValueReader[], object>, Type>>();
        private readonly List<Tuple<int, Func<ValueReader[], ValueReader[], object>, Type>> min = new List<Tuple<int, Func<ValueReader[], ValueReader[], object>, Type>>();
        private readonly string Name;

        internal static void AddFunction(string name, MethodInfo callInfo)
        {
            FunctionHolder fh;
            if (!Holders.TryGetValue(name, out fh))
            {
                fh = new FunctionHolder(name);
                Holders[name] = fh;
            }
            fh.addFunction(callInfo);
        }

        internal static bool HasFunction(string name) { return Holders.ContainsKey(name); }

        internal static FunctionHolder getFunctionHolder(string name)
        {
            FunctionHolder fh = null;
            if(!Holders.TryGetValue(name, out fh)) Game.CurrentGame.Die("Function " + name + " has not been defined");
            return fh;
        }

        internal Tuple<Func<ValueReader[], object>, Type> GetExactMatch(int numArgs)
        {
            Tuple<Func<ValueReader[], object>, Type> function = null;
            exact.TryGetValue(numArgs, out function);
            return function;
        }

        internal Tuple<int, Func<ValueReader[], ValueReader[], object>, Type> GetParamsMatch(int numArgs)
        {
            foreach (Tuple<int, Func<ValueReader[], ValueReader[], object>, Type> func in min)
            {
                if (func.Item1 <= numArgs) return func;
            }
            Game.CurrentGame.Die("Can't matche a call to " + Name + " with " + numArgs + " arguments");
            return null;
        }

        private FunctionHolder(string name) {
            Name = name;
        }

        private void addFunction(MethodInfo callInfo)
        {
            ParameterInfo[] paramInfos = callInfo.GetParameters();
            ParameterExpression arguments = Expression.Parameter(typeof(ValueReader[]));
            ParameterExpression paramArgs = Expression.Parameter(typeof(ValueReader[]));
            Expression[] argumentConversions = new Expression[paramInfos.Length];
            bool hasParams = false;
            for (int i = 0; i < paramInfos.Length; i++)
            {
                //since params can only be at the end, so it can't have been true then false.
                hasParams = Attribute.IsDefined(paramInfos[i], typeof(ParamArrayAttribute));
                if (paramInfos[i].ParameterType != typeof(ValueReader) && (paramInfos[i].ParameterType != typeof(ValueReader[]) && hasParams))
                    Game.CurrentGame.Die("Function " + Name + " has non ValueReader arguments");

                if (hasParams) argumentConversions[i] = paramArgs;
                else argumentConversions[i] = Expression.ArrayIndex(arguments, Expression.Constant(i));
            }

            if (hasParams)
            {
                Expression<Func<ValueReader[], ValueReader[], object>> lambda = Expression.Lambda<Func<ValueReader[], ValueReader[], object>>
                    (Expression.Convert(Expression.Call(callInfo, argumentConversions), typeof(object)), arguments, paramArgs);

                while (lambda.CanReduce) lambda = (Expression<Func<ValueReader[], ValueReader[], object>>)lambda.Reduce();
                
                min.Add(new Tuple<int, Func<ValueReader[], ValueReader[], object>, Type>
                    (paramInfos.Length, lambda.Compile(), callInfo.ReturnType));

                min.Sort((first, second) => second.Item1.CompareTo(first.Item1));
            }
            else
            {
                Expression<Func<ValueReader[], object>> lambda = Expression.Lambda<Func<ValueReader[], object>>
                    (Expression.Convert(Expression.Call(callInfo, argumentConversions), typeof(object)), arguments);
                while (lambda.CanReduce) lambda = (Expression<Func<ValueReader[], object>>)lambda.Reduce();
                exact[paramInfos.Length] = new Tuple<Func<ValueReader[], object>, Type>(lambda.Compile(), callInfo.ReturnType);
            }
        }
    }
}
