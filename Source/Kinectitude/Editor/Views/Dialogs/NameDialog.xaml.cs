//-----------------------------------------------------------------------
// <copyright file="NameDialog.xaml.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace Kinectitude.Editor.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        public NameDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
