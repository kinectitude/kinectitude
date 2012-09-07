﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views
{
    /// <summary>
    /// Interaction logic for WorkspaceWindow.xaml
    /// </summary>
    public partial class WorkspaceWindow : Window
    {
        public WorkspaceWindow()
        {
            DataContext = Workspace.Instance;
            Workspace.Instance.NewGame();

            InitializeComponent();
        }
    }
}
