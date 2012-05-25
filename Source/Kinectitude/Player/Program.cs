using System;
using System.Reflection;

namespace Kinectitude.Player
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            using (Application app = new Application())
            {
                app.Run();
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender,
                                              ResolveEventArgs args)
        {
            var assemblyname = args.Name.Split(',')[0];
            var assemblyFileName = ".\\Plugins\\Kinectitude.Render.dll";
            var assembly = Assembly.LoadFrom(assemblyFileName);
            return assembly;
        }


    }
}
