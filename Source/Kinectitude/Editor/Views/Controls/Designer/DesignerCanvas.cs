using Kinectitude.Editor.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal static class ExtensionMethods
    {
        public static T FindAncestorOfType<T>(this FrameworkElement element) where T : FrameworkElement
        {
            var current = element as T;

            while (null == current && VisualTreeHelper.GetParent(element) != null)
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                current = element as T;
            }

            return current;
        }
    }

    internal sealed class DesignerCanvas : ItemsControl
    {
        public static DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(DesignerCanvas));

        public static DependencyProperty IsSnapEnabledProperty =
            DependencyProperty.Register("IsSnapEnabled", typeof(bool), typeof(DesignerCanvas));

        public static DependencyProperty SnapDistanceProperty =
            DependencyProperty.Register("SnapDistance", typeof(double), typeof(DesignerCanvas));

        public static DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CutCommandProperty =
            DependencyProperty.Register("CutCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty PasteCommandProperty =
            DependencyProperty.Register("PasteCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty PointCommandProperty =
            DependencyProperty.Register("PointCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CameraLeftProperty =
            DependencyProperty.Register("CameraLeft", typeof(double), typeof(DesignerCanvas));

        public static DependencyProperty CameraTopProperty =
            DependencyProperty.Register("CameraTop", typeof(double), typeof(DesignerCanvas));

        public static DependencyProperty CameraWidthProperty =
            DependencyProperty.Register("CameraWidth", typeof(double), typeof(DesignerCanvas));

        public static DependencyProperty CameraHeightProperty =
            DependencyProperty.Register("CameraHeight", typeof(double), typeof(DesignerCanvas));

        private enum Mode
        {
            Selecting,
            Transforming,
            ScalingRotating
        }

        public const double DragThreshold = 0.5;

        private readonly List<DesignerItem> selected;

        private Mode mode;

        private bool mouseDown;
        private bool dragging;
        private Point startPoint;
        private Point previousPoint;

        private ElasticBandAdorner elasticBand;
        private TranslateAdorner translator;

        public IEnumerable<DesignerItem> SelectedItems
        {
            get { return selected; }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public bool IsSnapEnabled
        {
            get { return (bool)GetValue(IsSnapEnabledProperty); }
            set { SetValue(IsSnapEnabledProperty, value); }
        }

        public double SnapDistance
        {
            get { return (double)GetValue(SnapDistanceProperty); }
            set { SetValue(SnapDistanceProperty, value); }
        }

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public ICommand CopyCommand
        {
            get { return (ICommand)GetValue(CopyCommandProperty); }
            set { SetValue(CopyCommandProperty, value); }
        }

        public ICommand CutCommand
        {
            get { return (ICommand)GetValue(CutCommandProperty); }
            set { SetValue(CutCommandProperty, value); }
        }

        public ICommand PasteCommand
        {
            get { return (ICommand)GetValue(PasteCommandProperty); }
            set { SetValue(PasteCommandProperty, value); }
        }

        public ICommand PointCommand
        {
            get { return (ICommand)GetValue(PointCommandProperty); }
            set { SetValue(PointCommandProperty, value); }
        }

        public double CameraLeft
        {
            get { return (double)GetValue(CameraLeftProperty); }
            set { SetValue(CameraLeftProperty, value); }
        }

        public double CameraTop
        {
            get { return (double)GetValue(CameraTopProperty); }
            set { SetValue(CameraTopProperty, value); }
        }

        public double CameraWidth
        {
            get { return (double)GetValue(CameraWidthProperty); }
            set { SetValue(CameraWidthProperty, value); }
        }

        public double CameraHeight
        {
            get { return (double)GetValue(CameraHeightProperty); }
            set { SetValue(CameraHeightProperty, value); }
        }

        public DesignerCanvas()
        {
            mode = Mode.Selecting;
            selected = new List<DesignerItem>();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
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
                    if (null != CutCommand)
                    {
                        CutCommand.Execute(GetSelectedData());
                    }
                }
                else if (e.Key == Key.C)
                {
                    if (null != CopyCommand)
                    {
                        CopyCommand.Execute(GetSelectedData());
                    }
                }
                else if (e.Key == Key.V)
                {
                    if (null != PasteCommand)
                    {
                        PasteCommand.Execute(null);
                    }
                }
                else if (e.Key == Key.D)
                {
                    DeselectAll();
                }
                else if (e.Key == Key.A)
                {
                    SelectAll();
                }
            }
            else if (e.Key == Key.Delete)
            {
                if (null != DeleteCommand)
                {
                    DeleteCommand.Execute(GetSelectedData());
                }
            }

            if (delta.X != 0 || delta.Y != 0)
            {
                if (mode == Mode.Selecting && selected.Count > 0)
                {
                    mode = Mode.Transforming;
                }

                if (mode == Mode.Transforming)
                {
                    TranslateSelection(delta);
                }
            }

            e.Handled = true;
        }

        private IEnumerable GetSelectedData()
        {
            return SelectedItems.Select(x => ItemContainerGenerator.ItemFromContainer(x));
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(this);

            if (mouseDown)
            {
                if (!dragging)
                {
                    var delta = currentPoint - startPoint;

                    if (Math.Abs(delta.X) > DragThreshold || Math.Abs(delta.Y) > DragThreshold)
                    {
                        dragging = true;
                        OnDragStart(e);
                    }
                }

                if (dragging)
                {
                    OnDrag(e);
                }
            }

            previousPoint = currentPoint;
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            mouseDown = false;

            if (dragging)
            {
                OnDragStop(e);
            }
            else
            {
                OnClicked(e);
            }

            dragging = false;
            ReleaseMouseCapture();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Focus();

            mouseDown = true;
            dragging = false;

            startPoint = e.GetPosition(this);
            previousPoint = startPoint;

            CaptureMouse();
        }

        private void OnDragStart(MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(this);

            if (mode == Mode.Selecting)
            {
                var item = FindDesignerItemAt(currentPoint);
                if (null != item)
                {
                    Select(item);
                    mode = Mode.Transforming;
                }
                else
                {
                    ShowElasticBand();
                }
            }
            else if (mode == Mode.Transforming)
            {
                var item = FindDesignerItemAt(currentPoint);
                if (null != item && item.IsSelected)
                {
                    var delta = currentPoint - previousPoint;
                    TranslateSelection(delta);
                }
                else
                {
                    mode = Mode.Selecting;
                    ShowElasticBand();
                }
            }
        }

        private void TranslateSelection(Vector delta)
        {
            foreach (var item in SelectedItems)
            {
                item.DesignLeft += delta.X;
                item.DesignTop += delta.Y;
            }

            translator.Update();

            var layer = AdornerLayer.GetAdornerLayer(this);
            if (null != layer)
            {
                layer.Update();
            }
        }

        private void ShowElasticBand()
        {
            elasticBand = new ElasticBandAdorner(this, startPoint);

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            if (null != layer)
            {
                layer.Add(elasticBand);
            }
        }

        private void HideElasticBand()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            if (null != layer)
            {
                layer.Remove(elasticBand);
                elasticBand = null;
            }
        }

        private void OnDrag(MouseEventArgs e)
        {
            if (mode == Mode.Selecting)
            {
                elasticBand.Update(e.GetPosition(this));
            }
            else if (mode == Mode.Transforming)
            {
                var currentPoint = e.GetPosition(this);
                var delta = currentPoint - previousPoint;

                TranslateSelection(delta);
            }
        }

        private void OnDragStop(MouseEventArgs e)
        {
            if (mode == Mode.Selecting)
            {
                HideElasticBand();
            }
            else if (mode == Mode.Transforming)
            {

            }
        }

        private void OnClicked(MouseButtonEventArgs e)
        {
            var currentPoint = e.GetPosition(this);

            if (mode == Mode.Selecting)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                {
                    DeselectAll();
                }

                DesignerItem item = FindDesignerItemAt(currentPoint);
                if (null != item)
                {
                    if (item.IsSelected)
                    {
                        Deselect(item);
                    }
                    else
                    {
                        Select(item);
                    }
                }
                else if (null != PointCommand)
                {
                    PointCommand.Execute(currentPoint);
                }
            }
            else if (mode == Mode.Transforming)
            {
                DesignerItem item = FindDesignerItemAt(currentPoint);
                if (null != item)
                {
                    if (!item.IsSelected)
                    {
                        mode = Mode.Selecting;
                        DeselectAll();
                        Select(item);
                    }
                }
                else
                {
                    mode = Mode.Selecting;
                    DeselectAll();
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DesignerItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DesignerItem(this);
        }

        private DesignerItem FindDesignerItemAt(Point point)
        {
            DesignerItem item = null;

            var result = VisualTreeHelper.HitTest(this, point);
            if (null != result)
            {
                var element = result.VisualHit as FrameworkElement;
                item = element.FindAncestorOfType<DesignerItem>();
            }

            return item;
        }

        private void ShowSelection()
        {
            translator = new TranslateAdorner(this);

            var layer = AdornerLayer.GetAdornerLayer(this);
            if (null != layer)
            {
                layer.Add(translator);
            }
        }

        private void HideSelection()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            if (null != layer)
            {
                layer.Remove(translator);
                translator = null;
            }
        }

        public void Select(DesignerItem item)
        {
            if (!item.IsSelected)
            {
                item.IsSelected = true;
                selected.Add(item);

                if (null == translator)
                {
                    ShowSelection();
                }

                translator.Update();

                CheckSelectedItem();
            }
        }

        public void Deselect(DesignerItem item)
        {
            if (item.IsSelected)
            {
                item.IsSelected = false;
                selected.Remove(item);

                if (selected.Count == 0)
                {
                    HideSelection();
                }
                else
                {
                    translator.Update();
                }

                CheckSelectedItem();
            }
        }

        private void CheckSelectedItem()
        {
            if (selected.Count == 1)
            {
                var item = selected.Single();
                var obj = ItemContainerGenerator.ItemFromContainer(item);
                SelectedItem = obj;
            }
            else
            {
                SelectedItem = null;
            }
        }

        public void SelectAll()
        {
            foreach (var item in Items)
            {
                var designerItem = (DesignerItem)ItemContainerGenerator.ContainerFromItem(item);
                Select(designerItem);
            }

            CheckSelectedItem();
        }

        public void DeselectAll()
        {
            var previouslySelected = selected.ToArray();

            foreach (var item in previouslySelected)
            {
                Deselect(item);
            }
        }

        public void Update()
        {
            if (null != translator)
            {
                translator.Update();
            }
        }

        public void StartPlacement(EntityFactory factory)
        {

        }
    }
}
