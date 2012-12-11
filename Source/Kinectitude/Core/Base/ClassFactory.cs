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
using Kinectitude.Core.Functions;

using Math = Kinectitude.Core.Functions.Math;

namespace Kinectitude.Core.Base
{
    internal sealed class ClassFactory
    {
        internal static readonly Dictionary<string, Type> TypesDict = new Dictionary<string, Type>();

        private static readonly MethodInfo EnumParse = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string) });
        
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
        private static readonly Dictionary<string, Func<object>> Constructors = new Dictionary<string, Func<object>>();

        //empty constructors
        private static readonly Dictionary<Type, Func<object>> ConstructorTypes = new Dictionary<Type, Func<object>>();

        //used to get the required types of a component
        private static readonly Dictionary<Type, List<Type>> componentNeeds = new Dictionary<Type, List<Type>>();

        //used to get the provided types of a component
        private static readonly Dictionary<Type, List<Type>> componentProvides = new Dictionary<Type, List<Type>>();

        //used for each funciton name
        private static readonly Dictionary<string, FunctionHolder> functions = new Dictionary<string, FunctionHolder>();

        static ClassFactory()
        {
            RegisterType("AttributeChanges", typeof(AttributeChangesEvent));
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
            foreach(MethodInfo mi in typeof(Math).GetMethods().Where(mi => Attribute.IsDefined(mi, typeof(PluginAttribute))))
                FunctionHolder.AddFunction(mi.Name, mi);
            foreach (MethodInfo mi in typeof(Conversions).GetMethods().Where(mi => Attribute.IsDefined(mi, typeof(PluginAttribute))))
                FunctionHolder.AddFunction(mi.Name, mi);
        }

        internal static void LoadServicesAndManagers(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes().Where(item => typeof(Service).IsAssignableFrom(item)))
            {
                Service service = Activator.CreateInstance(type) as Service;
                Game.CurrentGame.SetService(service);
            }

            foreach (Type type in assembly.GetTypes().Where(item => typeof(IManager).IsAssignableFrom(item)))
            {
                ConstructorTypes[type] = createConstructorDelegate(type);
            }
        }

        internal static void RegisterFunction(string registeredName, MethodInfo methodInfo)
        {
            //if (FunctionHolder.HasFunction(registeredName)) Game.CurrentGame.Die("The function " + registeredName + " is declared twice");
            FunctionHolder.AddFunction(registeredName, methodInfo);
        }

        internal static void RegisterType(string registeredName, Type type)
        {
            //if (Constructors.ContainsKey(registeredName)) Game.CurrentGame.Die("The class " + registeredName + "  is declared twice");
            if (typeof(Component).IsAssignableFrom(type))
            {
                List<Type> provides = new List<Type>();
                foreach (ProvidesAttribute provided in type.GetCustomAttributes(true).
                    Where(input => input.GetType() == typeof(ProvidesAttribute)))
                {
                    if (!provided.Type.IsAssignableFrom(type)) Game.CurrentGame.Die(registeredName + " can't provide " + provided.Type.FullName);
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
            if (ConstructorTypes.TryGetValue(type, out creator)) Constructors[registeredName] = creator;
            else Constructors[registeredName] = ConstructorTypes[type] = createConstructorDelegate(type);

            settersByType[type] = new Dictionary<string, Action<object, object>>();
            referedDictionary[type] = registeredName;
            paramType[type] = new Dictionary<string, Type>();
            gettersByType[type] = new Dictionary<string, Func<object, object>>();
            TypesDict[registeredName] = type;

            foreach (PropertyInfo pi in type.GetProperties().Where(item => Attribute.IsDefined(item, typeof(PluginPropertyAttribute))))
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
                    Game.CurrentGame.Die("Can't get the setter for " + registeredName);
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

        internal static void SetParam(object obj, string param, object val)
        {
            Type setType = null;
            
            if(!paramType[obj.GetType()].TryGetValue(param, out setType))
                Game.CurrentGame.Die(referedDictionary[obj.GetType()] + " does not have parameter " + param);

            Dictionary<string, Action<object, object>> setters = settersByType[obj.GetType()];

            Action<object, object> action = setters[param] as Action<object, object>;

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
            ParameterExpression setAs = Expression.Parameter(typeof(object));
            ParameterExpression var = Expression.Variable(type);
            List<Expression> rest = new List<Expression>();
            rest.Add(Expression.Assign(var, Expression.New(type.GetConstructor(Type.EmptyTypes))));
            List<ParameterExpression> varExprs = new List<ParameterExpression>();
            varExprs.Add(var);
            LabelTarget target = Expression.Label(typeof(object));

            foreach (PropertyInfo pi in type.GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(PluginPropertyAttribute))))
            {
                PluginPropertyAttribute pluginAttribute = pi.GetCustomAttributes(typeof(PluginPropertyAttribute), true)[0] as PluginPropertyAttribute;

                Type propertyType = pi.PropertyType;

                
                Expression setBody;
                if (propertyType == typeof(ValueReader))
                {
                    ValueReader value = ConstantReader.CacheOrCreate(pluginAttribute.DefaultValue);
                    setBody = pluginAttribute.DefaultValue == null ? (Expression)Expression.Constant(ConstantReader.NullValue) : Expression.Constant(value);
                }
                else
                {
                    Expression set;
                    set = pluginAttribute.DefaultValue == null ? (Expression)Expression.Default(propertyType) : Expression.Constant(pluginAttribute.DefaultValue);
                    setBody = Expression.Convert(set, propertyType);
                }
                
                MethodCallExpression setExpr = Expression.Call(var, pi.GetSetMethod(), setBody);
                rest.Add(setExpr);
            }

            rest.Add(Expression.Return(target, var, typeof(object)));
            rest.Add(Expression.Label(target, var));
            Expression<Func<object>> createAndSet = Expression.Lambda<Func<object>>(Expression.Block(varExprs, rest));
            while (createAndSet.CanReduce)createAndSet = (Expression<Func<object>>) createAndSet.Reduce();
            return createAndSet.Compile();
        }

        private static void createDelegate(PropertyInfo pi, out Action<object,object> setter, out Func<object, object> getter)
        {
            Expression setBody;
            ParameterExpression setAs = Expression.Parameter(typeof(object));
            if (pi.PropertyType.IsEnum)
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
