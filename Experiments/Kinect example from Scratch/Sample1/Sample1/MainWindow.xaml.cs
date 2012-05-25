using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Research.Kinect.Nui;
//using Microsoft.Samples.Kinect.WpfViewers;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Coding4Fun.Kinect.Wpf.Controls;
using Coding4Fun.Kinect.Wpf;


namespace Sample1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Runtime kinectDriver = Runtime.Kinects[0];
        private int minKinectCount = 1;       //0 - app is "Kinect Enabled". 1 - app "Requires Kinect".
        const int maxKinectCount = 2;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
           // kinectDriver.VideoFrameReady += runtime_VideoFrameReady;
            kinectDriver.SkeletonFrameReady += SkeletonFrameReady;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            kinectDriver.Initialize(RuntimeOptions.UseSkeletalTracking);
        }


        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            kinectDriver.Uninitialize();
        }

        void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            while (e.SkeletonFrame.Skeletons.Count() == 0) {
                MessageBox.Show("Please plug in a kinect!");
            };
            SkeletonFrame skeletonSet = e.SkeletonFrame;

            SkeletonData firstPerson = (from s in skeletonSet.Skeletons
                                        where s.TrackingState == SkeletonTrackingState.Tracked
                                        orderby s.UserIndex descending
                                        select s).FirstOrDefault();
            if (firstPerson == null) return;

            JointsCollection joints = firstPerson.Joints;

            Joint rightHand = joints[JointID.HandRight];
            Joint leftHand = joints[JointID.HandLeft];

            /*float posX = rightHand.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight).Position.X;
            float posY = rightHand.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight).Position.Y;*/

            Canvas.SetTop(rectangle1, -rightHand.Position.Y * 1000);

        }

        
        
    }

}
