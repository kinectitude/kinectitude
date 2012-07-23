using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Kinect.Toolbox;
using Microsoft.Kinect;

namespace GesturesViewer
{
    partial class MainWindow
    {
        void LoadGestureDetectors()
        {
            using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
            {
                circleGestureRecognizer = new TemplatedGestureDetector("Circle", recordStream);
                circleGestureRecognizer.TraceTo(gesturesCanvas, Colors.Chocolate);
                circleGestureRecognizer.OnGestureDetected += OnGestureDetected;
            }

            using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
            {
                circleGestureRecognizerL = new TemplatedGestureDetector("Circle L", recordStream);
                circleGestureRecognizerL.TraceTo(gesturesCanvas, Colors.Chocolate);
                circleGestureRecognizerL.OnGestureDetected += OnGestureDetected;
            }

            using (Stream recordStream = File.Open(stepPath, FileMode.OpenOrCreate))
            {
                stepGestureRecognizer = new TemplatedGestureDetector("RIGHT FOOT", recordStream);
                stepGestureRecognizer.TraceTo(gesturesCanvas, Colors.Red);
                stepGestureRecognizer.OnGestureDetected += OnGestureDetected;
            }

            using (Stream recordStream = File.Open(stepPathL, FileMode.OpenOrCreate))
            {
                stepGestureRecognizerL = new TemplatedGestureDetector("LEFT FOOT", recordStream);
                stepGestureRecognizerL.TraceTo(gesturesCanvas, Colors.Yellow);
                stepGestureRecognizerL.OnGestureDetected += OnGestureDetected;
            }
            

            using (Stream recordStream = File.Open(stepPath, FileMode.OpenOrCreate))
            {
                stepGestureRecognizer = new TemplatedGestureDetector("RIGHT FOOT", recordStream);
                stepGestureRecognizer.TraceTo(gesturesCanvas, Colors.Red);
                stepGestureRecognizer.OnGestureDetected += OnGestureDetected;
            }

            using (Stream recordStream = File.Open(stepPathL, FileMode.OpenOrCreate))
            {
                stepGestureRecognizerL = new TemplatedGestureDetector("LEFT FOOT", recordStream);
                stepGestureRecognizerL.TraceTo(gesturesCanvas, Colors.Yellow);
                stepGestureRecognizerL.OnGestureDetected += OnGestureDetected;
            }

            using (Stream recordStream = File.Open(blockPath, FileMode.OpenOrCreate))
            {
                stepGestureRecognizer = new TemplatedGestureDetector("RIGHT FOOT", recordStream);
                stepGestureRecognizer.TraceTo(gesturesCanvas, Colors.Red);
                stepGestureRecognizer.OnGestureDetected += OnGestureDetected;
            }

            using (Stream recordStream = File.Open(blockPathL, FileMode.OpenOrCreate))
            {
                stepGestureRecognizerL = new TemplatedGestureDetector("LEFT FOOT", recordStream);
                stepGestureRecognizerL.TraceTo(gesturesCanvas, Colors.Yellow);
                stepGestureRecognizerL.OnGestureDetected += OnGestureDetected;
            }

            templates.ItemsSource = circleGestureRecognizer.LearningMachine.Paths;
        }

        private void recordCircle_Click(object sender, RoutedEventArgs e)
        {
            if (circleGestureRecognizer.IsRecordingPath)
            {
                circleGestureRecognizer.EndRecordTemplate();
                recordCircle.Content = "Record new Circle";
                return;
            }

            circleGestureRecognizer.StartRecordTemplate();
            recordCircle.Content = "Stop Recording";
        }

        private void recordStep_Click(object sender, RoutedEventArgs e)
        {
            StepRecord();
        }

        public void StepRecord()
        {
            /*if (stepGestureRecognizer.IsRecordingPath)
            {
                stepGestureRecognizer.EndRecordTemplate();
                recordStep.Content = "Record new Step";
                return;
            }
            stepGestureRecognizer.StartRecordTemplate();*/

            if (stepGestureRecognizerL.IsRecordingPath)
            {
                stepGestureRecognizerL.EndRecordTemplate();
                recordStep.Content = "Record new Step";
                return;
            }
            stepGestureRecognizerL.StartRecordTemplate();

            recordStep.Content = "";
        }

        void OnGestureDetected(string gesture)
        {
            //if (!gesture.ToLower().Contains("foot")) return;
            int pos = detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));

            detectedGestures.SelectedIndex = pos;
        }

        void CloseGestureDetector()
        {
            if (circleGestureRecognizer != null)
            {
                using (Stream recordStream = File.Create(circleKBPath))
                {
                    circleGestureRecognizer.SaveState(recordStream);
                }
                circleGestureRecognizer.OnGestureDetected -= OnGestureDetected;
            }

            if (stepGestureRecognizer != null)
            {
                using (Stream recordStream = File.Create(stepPath))
                {
                    stepGestureRecognizer.SaveState(recordStream);
                }
                stepGestureRecognizer.OnGestureDetected -= OnGestureDetected;
            }

            if (stepGestureRecognizerL != null)
            {
                using (Stream recordStream = File.Create(stepPathL))
                {
                    stepGestureRecognizerL.SaveState(recordStream);
                }
                stepGestureRecognizerL.OnGestureDetected -= OnGestureDetected;
            }

            if (highBlockGestureRecognizer != null)
            {
                using (Stream recordStream = File.Create(highBlockPath))
                {
                    highBlockGestureRecognizer.SaveState(recordStream);
                }
                highBlockGestureRecognizer.OnGestureDetected -= OnGestureDetected;
            }

        }
    }
}
