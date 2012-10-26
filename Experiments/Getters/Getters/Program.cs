using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Getters
{
    class Program
    {
        static void Main(string[] args)
        {
            GetMethods gm = new GetMethods();
            gm.A = 100;
            gm.Next = gm;
            gm.Type = GetMethods.X.Test;

            PropertyInfo pi = typeof(GetMethods).GetProperty("A");
            
            var call = new Func<GetMethods, int>(input => (int)pi.GetGetMethod().Invoke(input, null));

            var instance = Expression.Parameter(typeof(object), "instance");

            UnaryExpression instanceCast = Expression.Convert(instance, pi.DeclaringType);

            var parameter = Expression.Parameter(typeof(object), "i");

            var cast = Expression.Convert(parameter, pi.DeclaringType);

            var getterBody = Expression.Property(cast, pi);

            var exp = Expression.Lambda<Func<object, int>>(getterBody, parameter);

            var call2 = exp.Compile();


            /*pi = typeof(GetMethods).GetProperty("Next");
            var instance = Expression.Parameter(typeof(object), "instance");

            UnaryExpression instanceCast = Expression.Convert(instance, pi.DeclaringType);

            var parameter = Expression.Parameter(typeof(object), "i");

            var cast = Expression.TypeAs(parameter, pi.DeclaringType);

            var getterBody = Expression.Property(cast, pi);

            var exp = Expression.Lambda<Func<object, object>>(getterBody, parameter);

            var call2 = exp.Compile();*/


            /*pi = typeof(GetMethods).GetProperty("Type");
            var instance = Expression.Parameter(typeof(object), "instance");

            UnaryExpression instanceCast = Expression.Convert(instance, pi.DeclaringType);

            var parameter = Expression.Parameter(typeof(object), "i");

            var cast = Expression.TypeAs(parameter, pi.DeclaringType);

            var getterBody = Expression.Property(cast, pi);

            var exp = Expression.Lambda<Func<object, Enum>>(getterBody, parameter);

            var call2 = exp.Compile();*/

            DateTime start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                int tmp = gm.A;
            }

            DateTime end = DateTime.Now;

            Console.WriteLine(end - start);

            start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                int tmp = call(gm);
            }

            end = DateTime.Now;

            Console.WriteLine(end - start);

            start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                int tmp = call2(gm);
            }

            end = DateTime.Now;

            start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                GetMethods.X tmp = (GetMethods.X)call2(gm);
            }

            end = DateTime.Now;

            Console.WriteLine(end - start);

            Console.ReadKey();
        }
    }
}
