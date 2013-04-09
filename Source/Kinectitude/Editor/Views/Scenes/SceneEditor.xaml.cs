//-----------------------------------------------------------------------
// <copyright file="SceneEditor.xaml.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;
using Kinectitude.Editor.Views.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kinectitude.Editor.Views.Scenes
{
    /// <summary>
    /// Interaction logic for SceneEditor.xaml
    /// </summary>
    public partial class SceneEditor : UserControl
    {
        public SceneEditor()
        {
            InitializeComponent();
        }

        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Vector delta = new Vector();

            if (e.Key == Key.Left)
            {
                delta.X = -1.0d;
            }
            else if (e.Key == Key.Right)
            {
                delta.X = 1.0d;
            }
            else if (e.Key == Key.Up)
            {
                delta.Y = -1.0d;
            }
            else if (e.Key == Key.Down)
            {
                delta.Y = 1.0d;
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.X)
                {
                    canvas.Cut();
                }
                else if (e.Key == Key.C)
                {
                    canvas.Copy();
                }
                else if (e.Key == Key.V)
                {
                    canvas.Paste();
                }
                else if (e.Key == Key.D)
                {
                    canvas.DeselectAll();
                }
                else if (e.Key == Key.A)
                {
                    canvas.SelectAll();
                }
            }
            else if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                canvas.Delete();
            }

            if (delta.X != 0 || delta.Y != 0)
            {
                canvas.PrecisionTranslate(delta);
            }

            e.Handled = true;
        }
    }
}
