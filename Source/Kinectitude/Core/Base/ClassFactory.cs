using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kinectitude.Attributes;
using Kinectitude.Core.Actions;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;
using Kinectitude.Core.Events;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Base;
using Kinectitude.Core.Managers;

namespace Kinectitude.Core.Base
{
    internal class ClassFactory
    {
        public static readonly Dictionary<string, Type> TypesDict = new Dictionary<string, Type>();

        private static readonly MethodInfo IntParse = typeof(int).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo FloatParse = typeof(float).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo LongParse = typeof(long).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo DoubleParse = typeof(double).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo BoolParse = typeof(bool).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo EnumParse =
            typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string) });

        //used for creating ValueWriter
        private static readonly MethodInfo CreateValueWriter = typeof(ValueWriter).GetMethod
            ("CreateValueWriter", BindingFlags.Static | BindingFlags.NonPublic);

        //used for creating ExpressionReader
        private static readonly MethodInfo CreateExpressionReader = typeof(ExpressionReader).GetMethod
            ("CreateExpressionReader", BindingFlags.Static | BindingFlags.NonPublic);

        //used for creating TypeMatcher
        private static readonly MethodInfo CreateTypeMatcher = typeof(TypeMatcher).GetMethod
            ("CreateTypeMatcher", BindingFlags.Static | BindingFlags.NonPublic);

        //empty constructors
        private static readonly Dictionary<string, Func<object>> Constructors =
            new Dictionary<string, Func<object>>();

        //empty constructors
        private static readonly Dictionary<Type, Func<object>> ConstructorTypes =
            new Dictionary<Type, Func<object>>();

        //normal setters
        private static readonly Dictionary<Type, Dictionary<string, object>> SettersByType =
            new Dictionary<Type, Dictionary<string, object>>();

        //stores the names set in define in case they are different
        private static readonly Dictionary<Type, string> ReferedDictionary = new Dictionary<Type, string>();

        //used to get the type that the setter is for casting the object in the setter's dictionary.
        private static Dictionary<Type, Dictionary<string, Type>> ParamType =
            new Dictionary<Type, Dictionary<string, Type>>();

        static ClassFactory()
        {
            RegisterType("IncrementAction", typeof(IncrementAction));
            RegisterType("AttributeChangesEvent", typeof(AttributeChangesEvent));
            RegisterType("AttributeEqualsEvent", typeof(AttributeEqualsEvent));
            RegisterType("SetAttributeAction", typeof(SetAttributeAction));
            RegisterType("PushSceneAction", typeof(PushSceneAction));
            RegisterType("PopSceneAction", typeof(PopSceneAction));
            RegisterType("ChangeSceneAction", typeof(ChangeSceneAction));
            RegisterType("SceneStartsEvent", typeof(SceneStartsEvent));
            RegisterType("FireTriggerAction", typeof(FireTriggerAction));
            RegisterType("TriggerOccursEvent", typeof(TriggerOccursEvent));
            RegisterType("TransformComponent", typeof(TransformComponent));
            RegisterType("SetPositionAction", typeof(SetPositionAction));
            RegisterType("TimeManager", typeof(TimeManager));
        }

        public static void RegisterType(string registeredName, Type type)
        {
            Constructors[registeredName] = ConstructorTypes[type] = createConstructorDelegate(type);
            SettersByType[type] = new Dictionary<string, object>();
            ReferedDictionary[type] = registeredName;
            ParamType[type] = new Dictionary<string, Type>();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                MethodInfo setMethod = pi.GetSetMethod();
                if (null == setMethod)
                {
                    continue;
                }
                ParamType[type][pi.Name] = pi.PropertyType;
                if (!Attribute.IsDefined(pi, typeof(PluginAttribute)))
                {
                    continue;
                }
                object setAction;
                if (typeof(IValueWriter) == pi.PropertyType)
                {
                    setAction = createIValueWriter(type, setMethod);
                }
                else if (typeof(ITypeMatcher) == pi.PropertyType || typeof(IExpressionReader) == pi.PropertyType)
                {
                    setAction = createReaderSetterDelegate(type, setMethod);
                }
                else
                {
                    setAction = createSetterDelegate(type, setMethod);
                }

                if (null != setAction)
                {
                    SettersByType[type][pi.Name] = setAction;
                }
                else
                {
                    //TODO throw exception
                }
            }
        }

        private static T CreateHelper<T, U>(U lookup, Dictionary<U, Func<object>> maker) where T : class
        {
            Func<object> constructor = maker[lookup];
            T returnVal = constructor() as T;
            return returnVal;
        }

        internal static T Create<T>(string name) where T : class
        {
            return CreateHelper<T, string>(name, Constructors);
        }

        internal static T Create<T>(Type type) where T : class
        {
            return CreateHelper<T, Type>(type, ConstructorTypes);
        }

        internal static void SetParam(object obj, string param, string val, Event evt, Entity entity)
        {
            Type setType = null;
            ParamType[obj.GetType()].TryGetValue(param, out setType);

            if (null == setType)
            {
                throw new InvalidAttributeException(param, ReferedDictionary[obj.GetType()]);
            }

            Dictionary<string, object> setters = SettersByType[obj.GetType()];
            if (typeof(IValueWriter) == setType)
            {
                Action<object, string, Entity> action =
                    setters[param] as Action<object, string, Entity>;
                action(obj, val, entity);
            }
            else if (typeof(IExpressionReader) == setType || typeof(ITypeMatcher) == setType)
            {
                Action<object, string, Event, Entity> action =
                    setters[param] as Action<object, string, Event, Entity>;
                action(obj, val, evt, entity);
            }
            else
            {
                Action<object, string> action =
                    setters[param] as Action<object, string>;
                action(obj, val);
            }
        }

        private static Func<object> createConstructorDelegate(Type type)
        {
            return Expression.Lambda<Func<object>>(
                Expression.New(
                    type.GetConstructor(Type.EmptyTypes)
                )
            ).Compile();
        }

        private static Action<object, string, Entity> createIValueWriter
            (Type type, MethodInfo method)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression entity = Expression.Parameter(typeof(Entity));

            return Expression.Lambda<Action<object, string, Entity>>(
                Expression.Call(
                    Expression.Convert(target, type),
                    method,
                    Expression.Call(
                        CreateValueWriter,
                        value,
                        entity
                    )
                ),
                target,
                value,
                entity
            ).Compile();
        }

        private static Action<object, string, Event, Entity> createReaderSetterDelegate
            (Type type, MethodInfo method)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression evt = Expression.Parameter(typeof(Event));
            ParameterExpression entity = Expression.Parameter(typeof(Entity));
            Type propertyType = method.GetParameters().Single().ParameterType;

            MethodInfo mi = typeof(IExpressionReader) == propertyType ? CreateExpressionReader : CreateTypeMatcher;

            return Expression.Lambda<Action<object, string, Event, Entity>>(
                Expression.Call(
                    Expression.Convert(target, type),
                    method,
                    Expression.Call(
                        mi,
                        value,
                        evt,
                        entity
                    )
                ),
                target,
                value,
                evt,
                entity
            ).Compile();
        }

        private static Action<object, string> createSetterDelegate(Type type, MethodInfo method)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression stringParameter = Expression.Parameter(typeof(string));
            var propertyType = method.GetParameters().Single().ParameterType;
            Expression convertedParameter = null;

            if (typeof(string) == propertyType)
            {
                convertedParameter = stringParameter;
            }
            else if (typeof(int) == propertyType)
            {
                convertedParameter = Expression.Call(IntParse, stringParameter);
            }
            else if (typeof(double) == propertyType)
            {
                convertedParameter = Expression.Call(DoubleParse, stringParameter);
            }
            else if (typeof(bool) == propertyType)
            {
                convertedParameter = Expression.Call(BoolParse, stringParameter);
            }
            else if (typeof(float) == propertyType)
            {
                convertedParameter = Expression.Call(FloatParse, stringParameter);
            }
            else if (typeof(long) == propertyType)
            {
                convertedParameter = Expression.Call(LongParse, stringParameter);
            }
            else if (propertyType.IsEnum)
            {
                convertedParameter = Expression.Convert(
                    Expression.Call(
                        EnumParse,
                        Expression.Constant(propertyType, typeof(Type)),
                        stringParameter
                    ),
                    propertyType
                );
            }
            return Expression.Lambda<Action<object, string>>(
                Expression.Call(
                    Expression.Convert(target, type),
                    method,
                    convertedParameter
                ),
                target,
                stringParameter
            ).Compile();
        }
    }
}
