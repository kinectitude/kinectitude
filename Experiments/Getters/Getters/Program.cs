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

            PropertyInfo pi = typeof(GetMethods).GetProperty("A");

            var instance = Expression.Parameter(pi.DeclaringType);

            var property = Expression.Property(instance, pi);

            var convert = Expression.Parameter(typeof(int));

            var call = (Func<GetMethods, int>)Expression.Lambda(instance, new ParameterExpression [] { convert } );
            Console.WriteLine(call(gm));

            Console.ReadKey();
        }
    }
}
