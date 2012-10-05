using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kinectitude.Core.Actions;
using Kinectitude.Core.Attributes;
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

        //used for creating BoolExpressionReader
        private static readonly ConstructorInfo CreateBoolExpressionReader = 
            typeof(BoolExpressionReader).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();

        //used for creating IntExpressionReader
        private static readonly ConstructorInfo CreateIntExpressionReader =
            typeof(IntExpressionReader).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();

        //used for creating DoubleExpressionReader
        private static readonly ConstructorInfo CreateDoubleExpressionReader =
            typeof(DoubleExpressionReader).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();

        //used for creating TypeMatcher
        private static readonly MethodInfo CreateTypeMatcher = typeof(TypeMatcher).GetMethod
            ("CreateTypeMatcher", BindingFlags.Static | BindingFlags.NonPublic);

        //empty constructors
        private static readonly Dictionary<string, Func<object>> constructors =
            new Dictionary<string, Func<object>>();

        //empty constructors
        private static readonly Dictionary<Type, Func<object>> ConstructorTypes =
            new Dictionary<Type, Func<object>>();

        //stores setters that are used by the user
        private static readonly Dictionary<Type, Dictionary<string, object>> settersByType =
            new Dictionary<Type, Dictionary<string, object>>();

        //stores getters for components, and actions of primatives
        private static readonly Dictionary<Type, Dictionary<string, object>> gettersByType =
            new Dictionary<Type, Dictionary<string, object>>();

        //stores getters for components, and actions of primatives
        private static readonly Dictionary<Type, Dictionary<string, Func<object, string>>> gettersByName =
            new Dictionary<Type, Dictionary<string, Func<object, string>>>();

        //stores the names set in define in case they are different
        private static readonly Dictionary<Type, string> referedDictionary = new Dictionary<Type, string>();

        //used to get the type that the setter is for casting the object in the setter's dictionary.
        private static readonly Dictionary<Type, Dictionary<string, Type>> paramType =
            new Dictionary<Type, Dictionary<string, Type>>();

        //used to get the required types of a component
        private static readonly Dictionary<Type, List<Type>> componentNeeds = new Dictionary<Type, List<Type>>();

        //used to get the provided types of a component
        private static readonly Dictionary<Type, List<Type>> componentProvides = new Dictionary<Type, List<Type>>();

        private static readonly HashSet<Type> stringEventEntityConstruct = new HashSet<Type>()
        {
            typeof(ITypeMatcher),
            typeof(IExpressionReader),
            typeof(IBoolExpressionReader),
            typeof(IDoubleExpressionReader),
            typeof(IIntExpressionReader)
        };

        static ClassFactory()
        {
            RegisterType("IncrementAction", typeof(IncrementAction));
            RegisterType("AttributeChangesEvent", typeof(AttributeChangesEvent));
            RegisterType("AttributeEqualsEvent", typeof(AttributeEqualsEvent));
            RegisterType("SetAction", typeof(SetAction));
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
            RegisterType("CreateTimerAction", typeof(CreateTimerAction));
            RegisterType("PauseTimersAction", typeof(PauseTimersAction));
            RegisterType("ResumeTimersAction", typeof(ResumeTimersAction));
            RegisterType("OnCreateEvent", typeof(OnCreateEvent));
        }

        internal static void LoadServices(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes().Where(item => typeof(Service).IsAssignableFrom(item)))
            {
                Service service = Activator.CreateInstance(type) as Service;
                Game.CurrentGame.SetService(service);
                if (service.AutoStart())
                {
                    service.Start();
                }
            }
        }

        internal static void RegisterType(string registeredName, Type type)
        {

            if (typeof(Component).IsAssignableFrom(type))
            {
                List<Type> provides = new List<Type>();
                foreach (ProvidesAttribute provided in type.GetCustomAttributes(true).
                    Where(input => input.GetType() == typeof(ProvidesAttribute)))
                {
                    if (!provided.Type.IsAssignableFrom(type))
                    {
                        throw new ArgumentException(type.FullName + " can't provide " + provided.Type.FullName);
                    }
                    provides.Add(provided.Type);
                }
                componentProvides.Add(type, provides);

                List<Type> needs = new List<Type>();
                foreach (RequiresAttribute requires in type.GetCustomAttributes(true).
                    Where(input => input.GetType() == typeof(RequiresAttribute)))
                {
                    needs.Add(requires.Type);
                }
                componentNeeds[type] = needs;
            }

            constructors[registeredName] = ConstructorTypes[type] = createConstructorDelegate(type);
            settersByType[type] = new Dictionary<string, object>();
            referedDictionary[type] = registeredName;
            paramType[type] = new Dictionary<string, Type>();
            gettersByType[type] = new Dictionary<string, object>();
            gettersByName[type] = new Dictionary<string, Func<object, string>>();
            TypesDict[registeredName] = type;

            foreach (PropertyInfo pi in type.GetProperties().Where(item => Attribute.IsDefined(item, typeof(PluginAttribute))))
            {

                paramType[type][pi.Name] = pi.PropertyType;

                object setter;
                object getter;
                Func<object, string> stringGetter;

                if (typeof(IValueWriter) == pi.PropertyType)
                {
                    createIValueWriter(type, pi, out setter, out getter, out stringGetter);
                }
                else if (stringEventEntityConstruct.Contains(pi.PropertyType))
                {
                    createReaderDelegate(type, pi, out setter, out getter, out stringGetter);
                }
                else
                {
                    createBasicDelegate(type, pi, out setter, out getter, out stringGetter);
                }

                if (null != setter)
                {
                    settersByType[type][pi.Name] = setter;
                }
                else
                {
                    //TODO throw exception
                }

                if(null != getter)
                {
                    gettersByType[type][pi.Name] = getter;
                    //if it has a getter, it should have a string representation
                    gettersByName[type][pi.Name] = stringGetter;
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

        internal static bool PreEvaluate(object obj, string param)
        {
            Type setType = null;
            if (!paramType[obj.GetType()].TryGetValue(param, out setType))
            {
                throw new InvalidAttributeException(param, referedDictionary[obj.GetType()]);
            }
            return !stringEventEntityConstruct.Contains(setType);
        }

        internal static void SetParam(object obj, string param, string val, Event evt, Entity entity)
        {
            Type setType = null;
            if(!paramType[obj.GetType()].TryGetValue(param, out setType))
            {
                throw new InvalidAttributeException(param, referedDictionary[obj.GetType()]);
            }

            Dictionary<string, object> setters = settersByType[obj.GetType()];

            if (typeof(IValueWriter) == setType)
            {
                Action<object, string, Entity> action =
                    setters[param] as Action<object, string, Entity>;
                action(obj, val, entity);
            }
            else if (stringEventEntityConstruct.Contains(setType))
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

        internal static T GetParam<T>(object obj, string param) where T : class
        {
            Func<object, T> getter = gettersByType[obj.GetType()][param] as Func<object, T>;
            return getter(obj);
        }

        internal static string GetStringParam(object obj, string param)
        {
            return gettersByName[obj.GetType()][param](obj);
        }

        private static Func<object> createConstructorDelegate(Type type)
        {
            return Expression.Lambda<Func<object>>(
                Expression.New(
                    type.GetConstructor(Type.EmptyTypes)
                )
            ).Compile();
        }

        private static Func<object, T> createGetter<T>(PropertyInfo pi)
        {
            if (null != pi.GetGetMethod())
            {
                ParameterExpression parameter = Expression.Parameter(typeof(object));
                UnaryExpression cast = Expression.TypeAs(parameter, pi.DeclaringType);
                Expression getterBody = Expression.Property(cast, pi);
                return Expression.Lambda<Func<object, T>>(getterBody, parameter).Compile();
            }
            return null;
        }

        private static void createIValueWriter
            (Type type, PropertyInfo pi, out object setter, out object getter, out Func<object, string> stringGetter)
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

            Func<object, IValueWriter> returnedGetter = createGetter<IValueWriter>(pi);
            getter = returnedGetter;
            stringGetter = new Func<object, string>(input => returnedGetter(input).Value);
        }

        private static void createReaderDelegate
            (Type type, PropertyInfo pi, out object setter, out object getter, out Func<object, string> stringGetter)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(string));
            ParameterExpression evt = Expression.Parameter(typeof(Event));
            ParameterExpression entity = Expression.Parameter(typeof(Entity));
            MethodInfo setMethod = pi.GetSetMethod();
            Type propertyType = setMethod.GetParameters().Single().ParameterType;

            Expression expr;

            if (typeof(IExpressionReader) == propertyType)
            {
                expr = Expression.Call(
                            CreateExpressionReader,
                            value,
                            evt,
                            entity
                        );
                Func<object, IExpressionReader> expressionGetter = createGetter<IExpressionReader>(pi);
                getter = expressionGetter;
                stringGetter = new Func<object, string>(input => expressionGetter(input).GetValue());
            }
            else if (typeof(IBoolExpressionReader) == propertyType)
            {
                expr =  Expression.New(CreateBoolExpressionReader,
                                value,
                                evt,
                                entity);
                Func<object, IBoolExpressionReader> expressionGetter = createGetter<IBoolExpressionReader>(pi);
                getter = expressionGetter;
                stringGetter = new Func<object, string>(input => expressionGetter(input).GetValue().ToString());
            }
            else if (typeof(IIntExpressionReader) == propertyType)
            {
                expr =  Expression.New(CreateIntExpressionReader,
                                value,
                                evt,
                                entity);
                Func<object, IIntExpressionReader> expressionGetter = createGetter<IIntExpressionReader>(pi);
                getter = expressionGetter;
                stringGetter = new Func<object, string>(input => expressionGetter(input).GetValue().ToString());
            }
            else if (typeof(IDoubleExpressionReader) == propertyType)
            {
                expr = Expression.New(CreateDoubleExpressionReader,
                                value,
                                evt,
                                entity);
                Func<object, IDoubleExpressionReader> expressionGetter = createGetter<IDoubleExpressionReader>(pi);
                getter = expressionGetter;
                stringGetter = new Func<object, string>(input => expressionGetter(input).GetValue().ToString());
            }
            else
            {
                expr = Expression.Call(
                            CreateTypeMatcher,
                            value,
                            evt,
                            entity
                        );

                Func<object, ITypeMatcher> typeGetter = createGetter<ITypeMatcher>(pi);
                getter = typeGetter;
                stringGetter = new Func<object, string>(input => typeGetter(input).NameOfLastMatch);
            }
            
            setter = Expression.Lambda<Action<object, string, Event, Entity>>(
                        Expression.Call(
                            Expression.Convert(target, type),
                            setMethod,
                            expr
                        ),
                        target,
                        value,
                        evt,
                        entity
                    ).Compile();
        }

        private static void createBasicDelegate
            (Type type, PropertyInfo pi, out object setter, out object getter, out Func<object, string> stringGetter)
        {
            MethodInfo setMethod = pi.GetSetMethod();
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression stringParameter = Expression.Parameter(typeof(string));
            var propertyType = setMethod.GetParameters().Single().ParameterType;
            Expression convertedParameter = null;

            if (typeof(string) == propertyType)
            {
                convertedParameter = stringParameter;
                getter = stringGetter = createGetter<string>(pi);
            }
            else if (typeof(int) == propertyType)
            {
                convertedParameter = Expression.Call(IntParse, stringParameter);
                Func<object, int> intGetter = createGetter<int>(pi);
                getter = intGetter;
                stringGetter = new Func<object, string>(input => intGetter(input).ToString());
            }
            else if (typeof(double) == propertyType)
            {
                convertedParameter = Expression.Call(DoubleParse, stringParameter);
                Func<object, double> doubleGetter = createGetter<double>(pi);
                getter = doubleGetter;
                stringGetter = new Func<object, string>(input => doubleGetter(input).ToString());
            }
            else if (typeof(bool) == propertyType)
            {
                convertedParameter = Expression.Call(BoolParse, stringParameter);
                Func<object, bool> boolGetter = createGetter<bool>(pi);
                getter = boolGetter;
                stringGetter = new Func<object, string>(input => boolGetter(input).ToString());
            }
            else if (typeof(float) == propertyType)
            {
                convertedParameter = Expression.Call(FloatParse, stringParameter);
                Func<object, float> floatGetter = createGetter<float>(pi);
                getter = floatGetter;
                stringGetter = new Func<object, string>(input => floatGetter(input).ToString());
            }
            else if (typeof(long) == propertyType)
            {
                convertedParameter = Expression.Call(LongParse, stringParameter);
                Func<object, long> longGetter = createGetter<long>(pi);
                getter = longGetter;
                stringGetter = new Func<object, string>(input => longGetter(input).ToString());
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

                ParameterExpression parameter = Expression.Parameter(typeof(object));
                UnaryExpression cast = Expression.Convert(parameter, pi.DeclaringType);
                Expression getterBody = Expression.Convert(Expression.Property(cast, pi), typeof(Enum));

                Func<object, Enum> objGetter =  Expression.Lambda<Func<object, Enum>>(getterBody, parameter).Compile();

                getter = objGetter;
                stringGetter = new Func<object, string>(input => objGetter(input).ToString());
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

        internal static List<Type> GetRequirements(Type component)
        {
            return componentNeeds[component];
        }

        internal static List<Type> GetProvided(Type component)
        {
            return componentProvides[component];
        }

        internal static string GetReferedName(Type t)
        {
            string name;
            if(referedDictionary.TryGetValue(t, out name)) return name;
            return null;
        }

    }
}
