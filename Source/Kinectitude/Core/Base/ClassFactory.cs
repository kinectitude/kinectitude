using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Data;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Actions;
using Kinectitude.Core.Events;
using Kinectitude.Core.Components;
using Kinectitude.Attributes;

namespace Kinectitude.Core.Base
{
    internal class ClassFactory{
        public static readonly Dictionary<string, Type> TypesDict = new Dictionary<string, Type>();

        private static readonly MethodInfo IntParse = typeof(int).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo FloatParse = typeof(float).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo LongParse = typeof(long).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo DoubleParse = typeof(double).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo BoolParse = typeof(bool).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo EnumParse = 
            typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string) });

        //used for creating SpecificWriter
        private static readonly MethodInfo CreateSpecificWriter = typeof(SpecificWriter).GetMethod
            ("CreateSpecificWriteableData", BindingFlags.Static | BindingFlags.NonPublic);

        //used for creating WriteableData
        private static readonly MethodInfo CreateWriteableData = typeof(WriteableData).GetMethod
            ("CreateWriteableData", BindingFlags.Static | BindingFlags.NonPublic);

        //used for creating SpecificReadable
        private static readonly MethodInfo CreateSpecificReadable = typeof(SpecificReadable).GetMethod
            ("CreateSpecificReadable", BindingFlags.Static | BindingFlags.NonPublic);

        //used for creating ReadableData
        private static readonly MethodInfo CreateReadableData = typeof(ReadableData).GetMethod
            ("CreateReadableData", BindingFlags.Static | BindingFlags.NonPublic);

        //empty constructors
        private static readonly Dictionary<string, Func<object>> Constructors = 
            new Dictionary<string, Func<object>>();

        //manager constructors, takes Game argument
        private static readonly Dictionary<string, Func<Game, object>> ManagerConstructors =
            new Dictionary<string, Func<Game,object>>();

        //normal setters
        private static readonly Dictionary<Type, Dictionary<string, object>> SettersByType = 
            new Dictionary<Type,Dictionary<string, object>>();

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
        }

        public static void RegisterType(string registeredName, Type type)
        {
            if (typeof(IManager).IsAssignableFrom(type))
            {
                ManagerConstructors[registeredName] = createManagerConstructorDelegate(type);
            }
            else
            {
                Constructors[registeredName] = createConstructorDelegate(type);
            }
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
                if (typeof(SpecificWriter) == pi.PropertyType || typeof(WriteableData) == pi.PropertyType)
                {
                    setAction = createWriterSetterDelegate(type, setMethod);
                }
                else if (typeof(ReadableData) == pi.PropertyType || typeof(SpecificReadable) == pi.PropertyType)
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

        public static T Create<T>(string name) where T : class
        {
            Func<object> constructor = Constructors[name];
            return constructor() as T;
        }

        public static T Create<T>(Type type, Game game) where T : class
        {
            if (!Constructors.ContainsKey(type.FullName))
            {
                RegisterType(type.FullName, type);
            }
            return ManagerConstructors[type.FullName](game) as T;
        }

        public static void SetParam(object obj, string param, string val, Scene s, Event evt, Entity entity)
        {

            Type setType = null;
            ParamType[obj.GetType()].TryGetValue(param, out setType);

            if (null == setType)
            {
                throw new InvalidAttributeException(param, ReferedDictionary[obj.GetType()]);
            }

            Dictionary<string, object> setters = SettersByType[obj.GetType()];
            if (typeof(SpecificWriter) == setType || typeof(WriteableData) == setType)
            {
                Action<object, string, Entity, Scene> action =
                    setters[param] as Action<object, string, Entity, Scene>;
                action(obj, val, entity, s);
            }
            else if (typeof(SpecificReadable) == setType || typeof(ReadableData) == setType)
            {
                Action<object, string, Event, Scene> action =
                    setters[param] as Action<object, string, Event, Scene>;
                action(obj, val, evt, s);
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

        public static Func<Game, object> createManagerConstructorDelegate(Type type)
        {
            ParameterExpression paramExpression = Expression.Parameter(typeof(Game));
            return Expression.Lambda<Func<Game, object>>(
                Expression.New(
                    type.GetConstructor(new Type[] { typeof(Game) }), paramExpression
                ), paramExpression
            ).Compile();
        }

        private static Action<object, string, Entity, Scene> createWriterSetterDelegate
            (Type type, MethodInfo method)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression entity = Expression.Parameter(typeof(Entity));
            ParameterExpression scene = Expression.Parameter(typeof(Scene));
            Type propertyType = method.GetParameters().Single().ParameterType;

            MethodInfo mi = typeof(SpecificWriter) == propertyType ? CreateSpecificWriter : CreateWriteableData;

            return Expression.Lambda<Action<object, string, Entity, Scene>>(
                Expression.Call(
                    Expression.Convert(target, type),
                    method,
                    Expression.Call(
                        mi,
                        value,
                        entity,
                        scene
                    )
                ),
                target,
                value,
                entity,
                scene
            ).Compile();
        }

        private static Action<object, string, Event, Scene> createReaderSetterDelegate
            (Type type, MethodInfo method)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression evt = Expression.Parameter(typeof(Event));
            ParameterExpression scene = Expression.Parameter(typeof(Scene));
            Type propertyType = method.GetParameters().Single().ParameterType;

            MethodInfo mi = typeof(SpecificReadable) == propertyType ? CreateSpecificReadable : CreateReadableData;

            return Expression.Lambda<Action<object, string, Event, Scene>>(
                Expression.Call(
                    Expression.Convert(target, type),
                    method,
                    Expression.Call(
                        mi,
                        value,
                        evt,
                        scene
                    )
                ),
                target,
                value,
                evt,
                scene
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
