using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kinectitude.Core.Actions;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Core.Events;
using Kinectitude.Core.Managers;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    internal class ClassFactory
    {
        internal static readonly Dictionary<string, Type> TypesDict = new Dictionary<string, Type>();

        private static readonly MethodInfo EnumParse = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string) });
        private static readonly MethodInfo WriterMaker = typeof(ValueReader).GetMethod("GetValueWriter",new[] {typeof(ValueReader)});
        
        //stores getters for components, and actions of primatives
        private static readonly Dictionary<Type, Dictionary<string, Func<object, object>>> gettersByType =
            new Dictionary<Type, Dictionary<string, Func<object, object>>>();

        //used to get the type that the setter is for casting the object in the setter's dictionary.
        private static readonly Dictionary<Type, Dictionary<string, Type>> paramType =
            new Dictionary<Type, Dictionary<string, Type>>();

        //stores the names set in define in case they are different
        private static readonly Dictionary<Type, string> referedDictionary = new Dictionary<Type, string>();

        //stores setters that are used by the user
        private static readonly Dictionary<Type, Dictionary<string, Action<object, object>>> settersByType =
            new Dictionary<Type, Dictionary<string, Action<object, object>>>();

        //empty constructors
        private static readonly Dictionary<string, Func<object>> constructors =
            new Dictionary<string, Func<object>>();

        //empty constructors
        private static readonly Dictionary<Type, Func<object>> ConstructorTypes =
            new Dictionary<Type, Func<object>>();

        //used to get the required types of a component
        private static readonly Dictionary<Type, List<Type>> componentNeeds = new Dictionary<Type, List<Type>>();

        //used to get the provided types of a component
        private static readonly Dictionary<Type, List<Type>> componentProvides = new Dictionary<Type, List<Type>>();

        static ClassFactory()
        {
            RegisterType("Increment", typeof(IncrementAction));
            RegisterType("AttributeChanges", typeof(AttributeChangesEvent));
            RegisterType("AttributeEquals", typeof(AttributeEqualsEvent));
            RegisterType("Set", typeof(SetAction));
            RegisterType("PushScene", typeof(PushSceneAction));
            RegisterType("PopScene", typeof(PopSceneAction));
            RegisterType("ChangeScene", typeof(ChangeSceneAction));
            RegisterType("SceneStarts", typeof(SceneStartsEvent));
            RegisterType("FireTrigger", typeof(FireTriggerAction));
            RegisterType("TriggerOccurs", typeof(TriggerOccursEvent));
            RegisterType("Transform", typeof(TransformComponent));
            RegisterType("TimeManager", typeof(TimeManager));
            RegisterType("CreateEntity", typeof(CreateEntityAction));
            RegisterType("Destroy", typeof(DestroyAction));
            RegisterType("CreateTimer", typeof(CreateTimerAction));
            RegisterType("PauseTimers", typeof(PauseTimersAction));
            RegisterType("ResumeTimers", typeof(ResumeTimersAction));
            RegisterType("OnCreate", typeof(OnCreateEvent));
            RegisterType("Quit", typeof(QuitAction));
        }

        internal static void LoadServicesAndManagers(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes().Where(item => typeof(Service).IsAssignableFrom(item)))
            {
                Service service = Activator.CreateInstance(type) as Service;
                Game.CurrentGame.SetService(service);
                if (service.AutoStart()) service.Start();
            }

            foreach (Type type in assembly.GetTypes().Where(item => typeof(IManager).IsAssignableFrom(item)))
            {
                ConstructorTypes[type] = createConstructorDelegate(type);
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
                    if (!provided.Type.IsAssignableFrom(type)) Game.CurrentGame.Die(type.FullName + " can't provide " + provided.Type.FullName);
                    provides.Add(provided.Type);
                }
                componentProvides[type] = provides;

                List<Type> needs = new List<Type>();
                foreach (RequiresAttribute requires in type.GetCustomAttributes(true).
                    Where(input => input.GetType() == typeof(RequiresAttribute)))
                {
                    needs.Add(requires.Type);
                }
                componentNeeds[type] = needs;
            }

            Func<object> creator;
            if (ConstructorTypes.TryGetValue(type, out creator)) constructors[registeredName] = creator;
            else constructors[registeredName] = ConstructorTypes[type] = createConstructorDelegate(type);

            settersByType[type] = new Dictionary<string, Action<object, object>>();
            referedDictionary[type] = registeredName;
            paramType[type] = new Dictionary<string, Type>();
            gettersByType[type] = new Dictionary<string, Func<object, object>>();
            TypesDict[registeredName] = type;

            foreach (PropertyInfo pi in type.GetProperties().Where(item => Attribute.IsDefined(item, typeof(PluginAttribute))))
            {
                paramType[type][pi.Name] = pi.PropertyType;

                Action<object, object> setter = null;
                Func<object, object> getter = null;

                createDelegate(pi, out setter, out getter);

                if (null != setter)
                {
                    gettersByType[type][pi.Name] = getter;
                    settersByType[type][pi.Name] = setter;   
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
            return CreateHelper<T, string>(name, constructors);
        }

        internal static T Create<T>(Type type) where T : class
        {
            return CreateHelper<T, Type>(type, ConstructorTypes);
        }

        internal static void SetParam(object obj, string param, object val)
        {
            Type setType = null;
            
            if(!paramType[obj.GetType()].TryGetValue(param, out setType))
                Game.CurrentGame.Die(referedDictionary[obj.GetType()] + " does not have parameter " + param);

            Dictionary<string, Action<object, object>> setters = settersByType[obj.GetType()];

            Action<object, object> action = setters[param] as Action<object, object>;

            var p = Expression.Parameter(typeof(ValueReader));
            var tmp = new ConstantReader(10);
            var convert = Expression.Convert(p, typeof(string));
            var call = Expression.Lambda<Func<ValueReader, string>>(convert, p);
            var compiled = call.Compile();

            //TODO make this more effecient, just make the set in here.
            action(obj, val);
        }

        internal static object GetParam(object obj, string param)
        {
            Func<object, object> getter = gettersByType[obj.GetType()][param];
            return getter(obj);
        }

        private static Func<object> createConstructorDelegate(Type type)
        {
            return Expression.Lambda<Func<object>>(
                Expression.New(
                    type.GetConstructor(Type.EmptyTypes)
                )
            ).Compile();
        }

        private static void createDelegate(PropertyInfo pi, out Action<object,object> setter, out Func<object, object> getter)
        {
            Expression setBody;
            ParameterExpression setAs = Expression.Parameter(typeof(object));

            if (typeof(ValueWriter) == pi.PropertyType)
            {
                setBody = Expression.Call(WriterMaker, Expression.TypeAs(setAs, typeof(ValueReader)));
            }
            else if (pi.PropertyType.IsEnum)
            {
                setBody = Expression.Convert(
                    Expression.Call(
                        EnumParse,
                        Expression.Constant(pi.PropertyType, typeof(Type)),
                        Expression.Convert(Expression.TypeAs(setAs, typeof(ValueReader)), typeof(string))
                    ),
                    pi.PropertyType
                );
            }
            else if (typeof(TypeMatcher) == pi.PropertyType)
            {
                setBody = Expression.Convert(setAs, typeof(TypeMatcher));
            }
            else
            {
                setBody = Expression.Convert(Expression.TypeAs(setAs, typeof(ValueReader)), pi.PropertyType);
            }

            ParameterExpression objParam = Expression.Parameter(typeof(object));
            UnaryExpression cast = Expression.TypeAs(objParam, pi.DeclaringType);
            Expression getterBody = Expression.Property(cast, pi);
            
            Expression<Func<object, object>> expr = 
                Expression.Lambda<Func<object, object>>(Expression.Convert(getterBody, typeof(object)), objParam);

            while (expr.CanReduce) expr = (Expression<Func<object, object>>)expr.Reduce();
            getter = expr.Compile();

            Expression<Action<object, object>> setExpr = Expression.Lambda<Action<object, object>>(
                        Expression.Call(
                            cast,
                            pi.GetSetMethod(),
                            setBody
                        ),
                        objParam,
                        setAs);

            while(setExpr.CanReduce) setExpr = (Expression<Action<object, object>>)setExpr.Reduce();

            setter = setExpr.Compile();
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
            string name = null;
            referedDictionary.TryGetValue(t, out name);
            return name;
        }

        internal Type getParamType(string refered, string param)
        {
            Type type = TypesDict[refered];
            return paramType[type][param];
        }
    }
}
