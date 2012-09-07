using System;

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
            using (Application app = new Application())
            {
                app.Run();
            }
        }
    }
}
