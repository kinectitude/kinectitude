using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Xml.Linq;
using Core;

namespace FastReflection
{
    public static class ClassFactory
    {
        private static readonly MethodInfo IntParse = typeof(int).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo FloatParse = typeof(float).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo LongParse = typeof(long).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo DoubleParse = typeof(double).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo BoolParse = typeof(bool).GetMethod("Parse", new[] { typeof(string) });
        private static readonly MethodInfo EnumParse = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string) });

        private static readonly Dictionary<string, Func<object>> constructors;
        private static readonly Dictionary<string, Dictionary<string, Action<object, string>>> settersByType;

        static ClassFactory()
        {
            constructors = new Dictionary<string, Func<object>>();
            settersByType = new Dictionary<string, Dictionary<string, Action<object, string>>>();
        }

        public static void RegisterType(Type type)
        {
            if (typeof(Component).IsAssignableFrom(type))
            {
                constructors[type.Name] = createConstructorDelegate(type);
                settersByType[type.Name] = new Dictionary<string, Action<object, string>>();

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    MethodInfo setMethod = pi.GetSetMethod();
                    Action<object, string> setAction = createSetterDelegate(type, setMethod);
                    settersByType[type.Name][pi.Name] = setAction;
                }
            }
        }

        public static T Deserialize<T>(XElement element) where T : class
        {
            XAttribute type = element.Attribute("Type");

            if (null == type)
            {
                throw new ArgumentException("Element must have a Type attribute.");
            }
            
            string name = (string)type;
            Func<object> constructor = constructors[name];
            object ret = constructor();

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes("Type")))
            {
                Action<object, string> action = null;
                Dictionary<string, Action<object, string>> setters = null;
                settersByType.TryGetValue(name, out setters);
                if (null == setters)
                {
                    throw new ArgumentException(string.Format("Type '{0}' does not have a setter for property '{1}'.", name, attribute.Name.LocalName));
                }
                
                setters.TryGetValue(attribute.Name.LocalName, out action);

                if (null == action)
                {
                    throw new ArgumentException(string.Format("Type '{0}' is not registered.", name));
                }
                    
                action(ret, attribute.Value);
            }
            return ret as T;
        }

        private static Func<object> createConstructorDelegate(Type type)
        {
            return Expression.Lambda<Func<object>>(
                Expression.New(
                    type.GetConstructor(Type.EmptyTypes)
                )
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
