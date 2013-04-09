//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;
using Kinectitude.Editor.Views.Utils;
using System.ComponentModel;
using System.Windows;

namespace Kinectitude.Editor.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Workspace.Instance.Initialize();
            DataContext = Workspace.Instance;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Workspace.Instance.WarnOnClose(r =>
            {
                if (r == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            });
        }
    }
}
