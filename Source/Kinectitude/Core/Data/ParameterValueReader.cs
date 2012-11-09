﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ParameterValueReader : ValueReader
    {
        internal readonly object Obj;
        internal readonly string Param;
        internal readonly DataContainer Owner;

        internal static ParameterValueReader getParameterValueReader(object obj, string param, Scene scene)
        {
            Func<ParameterValueReader> create = new Func<ParameterValueReader>(() => new ParameterValueReader(obj, param, scene));
            return DoubleDictionary<object, string, ParameterValueReader>.getItem(obj, param, create);
        }

        private ParameterValueReader(object obj, string param, Scene scene)
        {
            Obj = obj;
            Param = param;
            Type objType = obj.GetType();
            if (typeof(Component).IsAssignableFrom(objType)) Owner = ((Component)obj).Entity;
            else if (typeof(IManager).IsAssignableFrom(objType)) Owner = scene;
            else if (typeof(Service).IsAssignableFrom(objType)) Owner = scene.Game;
            else Owner = null;
            if (null != Owner)
                Owner.NotifyOfComponentChange(ClassFactory.GetReferedName(Obj.GetType()) + '.' + Param, Change);
        }

        internal override double GetDoubleValue() { return ToNumber<double>(ClassFactory.GetParam(Obj, Param)); }
        internal override float GetFloatValue() { return ToNumber<float>(ClassFactory.GetParam(Obj, Param)); }
        internal override int GetIntValue() { return ToNumber<int>(ClassFactory.GetParam(Obj, Param)); }
        internal override long GetLongValue() { return ToNumber<long>(ClassFactory.GetParam(Obj, Param)); }
        internal override bool GetBoolValue() { return ToBool(ClassFactory.GetParam(Obj, Param)); }
        internal override string GetStrValue() { return ClassFactory.GetParam(Obj, Param).ToString(); }
        internal override PreferedType PreferedRetType() { return NativeReturnType(ClassFactory.GetParam(Obj, Param)); }

        internal override ValueWriter ConvertToWriter() { return new ParameterValueWriter(this); }
    }
}
