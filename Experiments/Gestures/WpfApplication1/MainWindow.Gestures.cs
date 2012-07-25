using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Kinect.Toolbox;
using Microsoft.Kinect;

namespace WpfApplication1
{
    partial class MainWindow
    {
        void LoadCircleGestureDetector()
        {
            using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
            {
                circleGestureRecognizer = new TemplatedGestureDetector("Circle", recordStream);
                circleGestureRecognizer.TraceTo(gesturesCanvas, Colors.Red);
                circleGestureRecognizer.OnGestureDetected += OnGestureDetected;
            }
            templates.ItemsSource = circleGestureRecognizer.LearningMachine.Paths;
        }

<<<<<<< HEAD
        void LoadHandUpGestureDetector()
        {
            using (Stream recordStream = File.Open(handUpPath, FileMode.OpenOrCreate))
            {
                handUpGestureRecognizer = new TemplatedGestureDetector("MoveHandUp", recordStream);
                handUpGestureRecognizer.TraceTo(gesturesCanvas, Colors.Red);
                handUpGestureRecognizer.OnGestureDetected += OnGestureDetected;
            }
            templates.ItemsSource = handUpGestureRecognizer.LearningMachine.Paths;
        }

=======
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd
        void OnGestureDetected(string gesture)
        {
            int pos = detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));

            detectedGestures.SelectedIndex = pos;
        }

        void CloseGestureDetector()
        {
<<<<<<< HEAD
            if (circleGestureRecognizer != null)
            {
                using (Stream recordStream = File.Create(circleKBPath))
                {
                    circleGestureRecognizer.SaveState(recordStream);
                }
                circleGestureRecognizer.OnGestureDetected -= OnGestureDetected;
            }
            if (handUpGestureRecognizer != null)
            {
                using (Stream recordStream = File.Create(handUpPath))
                {
                    handUpGestureRecognizer.SaveState(recordStream);
                }
                handUpGestureRecognizer.OnGestureDetected -= OnGestureDetected;
            }
=======
            if (circleGestureRecognizer == null)
                return;

            using (Stream recordStream = File.Create(circleKBPath))
            {
                circleGestureRecognizer.SaveState(recordStream);
            }
            circleGestureRecognizer.OnGestureDetected -= OnGestureDetected;
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd
        }
    }
}
