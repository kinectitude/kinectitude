﻿using System;
using System.IO;

namespace GesturesViewer
{
    partial class MainWindow
    {
        void StartVoiceCommander()
        {
            voiceCommander.Start(kinectSensor);
        }

        void voiceCommander_OrderDetected(string order)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (audioControl.IsChecked == false)
                    return;

                StepRecord(order);

                /*switch (order)
                {
                    case "record":
<<<<<<< HEAD
                        DirectRecord(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "kinectRecord" + Guid.NewGuid() + ".replay"));
                        break;
                    case "stop":
                        StopRecord();
=======
                        StepRecord();
                        break;
                    case "stop":
                        StepRecord();
>>>>>>> 11c5ca39c6e057b16bc2e5c5721c1ab574d74abd
                        break;
                }*/
            }));
        }

        void vc_OrderDetected(string order)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (audioControl.IsChecked == false)
                    return;

                StepRecord(order.ToUpper());
            }));
        }
    }
}
