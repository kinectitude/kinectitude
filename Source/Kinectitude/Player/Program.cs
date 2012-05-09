using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SlimDX.Windows;

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
            //RenderForm form = new RenderForm("Kinectitude");
            //MessagePump.Run(form, () => { });

            /*using (KinectitudeSample sample = new KinectitudeSample())
            {
                sample.Run();
            }*/

            using (Application app = new Application())
            {
                app.Run();
            }
        }
    }
}
