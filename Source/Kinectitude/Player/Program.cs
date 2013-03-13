using System;
using System.Windows.Forms;

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
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            using (Application app = new Application())
            {
                app.Run();
            }
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                if (null != ex)
                {
                    MessageBox.Show(
                        ex.Message + "\n\n" + ex.StackTrace,
                        "An Error Occurred",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop
                    );
                }
            }
            finally
            {
                System.Windows.Forms.Application.Exit();
            }
        }
    }
}
