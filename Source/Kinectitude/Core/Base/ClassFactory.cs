using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Actions;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;
using Kinectitude.Core.Events;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Managers;

namespace Kinectitude.Core.Base
{
    internal class ClassFactory
    {
        internal static readonly Dictionary<string, Type> TypesDict = new Dictionary<string, Type>();

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
        private static readonly Dictionary<string, Func<object>> constructors =
            new Dictionary<string, Func<object>>();

        //empty constructors
        private static readonly Dictionary<Type, Func<object>> ConstructorTypes =
            new Dictionary<Type, Func<object>>();

        private static readonly Dictionary<Type, Dictionary<string, object>> SettersByType =
            new Dictionary<Type, Dictionary<string, object>>();

        private static readonly Dictionary<Type, Dictionary<string, object>> GettersByType =
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
            RegisterType("CreateEntityAction", typeof(CreateEntityAction));
            RegisterType("DestroyAction", typeof(DestroyAction));
        }

        internal static void LoadServices(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes().Where(item => typeof(Service).IsAssignableFrom(item)))
            {
                Service service = Activator.CreateInstance(type) as Service;
                Game.CurrentGame.SetService(service);
            }
        }

        internal static void RegisterType(string registeredName, Type type)
        {
            constructors[registeredName] = ConstructorTypes[type] = createConstructorDelegate(type);
            SettersByType[type] = new Dictionary<string, object>();
            ReferedDictionary[type] = registeredName;
            ParamType[type] = new Dictionary<string, Type>();
            GettersByType[type] = new Dictionary<string, object>();

            foreach (PropertyInfo pi in type.GetProperties().Where(item => Attribute.IsDefined(item, typeof(PluginAttribute))))
            {

                ParamType[type][pi.Name] = pi.PropertyType;

                object setter;
                object getter;

                if (typeof(IValueWriter) == pi.PropertyType)
                {
                    createIValueWriter(type, pi, out setter, out getter);
                }
                else if (typeof(ITypeMatcher) == pi.PropertyType || typeof(IExpressionReader) == pi.PropertyType)
                {
                    createReaderDelegate(type, pi, out setter, out getter);
                }
                else
                {
                    createBasicDelegate(type, pi, out setter, out getter);
                }

                if (null != setter)
                {
                    SettersByType[type][pi.Name] = setter;
                }
                else
                {
                    //TODO throw exception
                }

                if(null != getter)
                {
                    GettersByType[type][pi.Name] = getter;
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
            return CreateHelper<T, string>(name, constructors);
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

        internal static T GetClassParam<T>(object obj, string param) where T : class
        {
            Func<object, object> getter = GettersByType[obj.GetType()][param] as Func<object, object>;
            return getter(obj) as T;
        }

        internal static T GetPrimativeParam<T>(object obj, string param) where T : struct
        {
            Func<object, object> getter = GettersByType[obj.GetType()][param] as Func<object, object>;
            return (T)getter(obj);
        }

        private static Func<object> createConstructorDelegate(Type type)
        {
            return Expression.Lambda<Func<object>>(
                Expression.New(
                    type.GetConstructor(Type.EmptyTypes)
                )
            ).Compile();
        }


        private static object createGetter<T>(PropertyInfo pi)
        {
            if (null != pi.GetGetMethod())
            {
                ParameterExpression parameter = Expression.Parameter(typeof(object));
                UnaryExpression cast = Expression.TypeAs(parameter, pi.DeclaringType);
                Expression getterBody = Expression.Property(cast, pi);
                return Expression.Lambda<Func<object, T>>(getterBody, parameter).Compile();
            }
            else
            {
                return null;
            }
        }

        private static void createIValueWriter(Type type, PropertyInfo pi, out object setter, out object getter)
        {

            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression entity = Expression.Parameter(typeof(Entity));

            setter = Expression.Lambda<Action<object, string, Entity>>
                        (Expression.Call(
                            Expression.Convert(target, type),
                            pi.GetSetMethod(),
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

            getter = createGetter<IValueWriter>(pi);
        }

        private static void createReaderDelegate(Type type, PropertyInfo pi, out object setter, out object getter)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression evt = Expression.Parameter(typeof(Event));
            ParameterExpression entity = Expression.Parameter(typeof(Entity));
            MethodInfo setMethod = pi.GetSetMethod();
            Type propertyType = setMethod.GetParameters().Single().ParameterType;

            MethodInfo mi = typeof(IExpressionReader) == propertyType ? CreateExpressionReader : CreateTypeMatcher;

            setter = Expression.Lambda<Action<object, string, Event, Entity>>(
                        Expression.Call(
                            Expression.Convert(target, type),
                            setMethod,
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
            //object works, may as well use it here instead of using an if statement
            getter = createGetter<object>(pi);
        }

        private static void createBasicDelegate(Type type, PropertyInfo pi, out object setter, out object getter)
        {
            MethodInfo setMethod = pi.GetSetMethod();
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression stringParameter = Expression.Parameter(typeof(string));
            var propertyType = setMethod.GetParameters().Single().ParameterType;
            Expression convertedParameter = null;

            if (typeof(string) == propertyType)
            {
                convertedParameter = stringParameter;
                //use object, so that everything can be an object but primatives and enums.
                getter = createGetter<object>(pi);
            }
            else if (typeof(int) == propertyType)
            {
                convertedParameter = Expression.Call(IntParse, stringParameter);
                //primatives can't be object.
                getter = createGetter<int>(pi);
            }
            else if (typeof(double) == propertyType)
            {
                convertedParameter = Expression.Call(DoubleParse, stringParameter);
                //primatives can't be object.
                getter = createGetter<double>(pi);
            }
            else if (typeof(bool) == propertyType)
            {
                convertedParameter = Expression.Call(BoolParse, stringParameter);
                //primatives can't be object.
                getter = createGetter<bool>(pi);
            }
            else if (typeof(float) == propertyType)
            {
                convertedParameter = Expression.Call(FloatParse, stringParameter);
                //primatives can't be object.
                getter = createGetter<float>(pi);
            }
            else if (typeof(long) == propertyType)
            {
                convertedParameter = Expression.Call(LongParse, stringParameter);
                //primatives can't be object.
                getter = createGetter<long>(pi);
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
                //can't use object because enums are not nullable, not sure what to do.
                //TODO ENUMS getter
                //getter = createGetter<object>(pi);
                getter = null;
            }
            else
            {
                //TODO throw exception that means something
                throw new ArgumentException("Can't find the type of the argument");
            }

            setter = Expression.Lambda<Action<object, string>>(
                        Expression.Call(
                            Expression.Convert(target, type),
                            setMethod,
                            convertedParameter
                        ),
                        target,
                        stringParameter
                    ).Compile();
        }
    }
}
