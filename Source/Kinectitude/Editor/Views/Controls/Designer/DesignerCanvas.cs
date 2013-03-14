using Kinectitude.Editor.Base;
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

        public static DependencyProperty BeginTranslateCommandProperty =
            DependencyProperty.Register("BeginTranslateCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitTranslateCommandProperty =
            DependencyProperty.Register("CommitTranslateCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitSendForwardCommandProperty =
            DependencyProperty.Register("CommitSendForwardCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitSendBackwardCommandProperty =
            DependencyProperty.Register("CommitSendBackwardCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitSendToFrontCommandProperty =
            DependencyProperty.Register("CommitSendToFrontCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitSendToBackCommandProperty =
            DependencyProperty.Register("CommitSendToBackCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitAlignLeftCommandProperty =
            DependencyProperty.Register("CommitAlignLeftCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitAlignCenterCommandProperty =
            DependencyProperty.Register("CommitAlignCenterCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitAlignRightCommandProperty =
            DependencyProperty.Register("CommitAlignRightCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitAlignTopCommandProperty =
            DependencyProperty.Register("CommitAlignTopCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitAlignMiddleCommandProperty =
            DependencyProperty.Register("CommitAlignMiddleCommand", typeof(ICommand), typeof(DesignerCanvas));

        public static DependencyProperty CommitAlignBottomCommandProperty =
            DependencyProperty.Register("CommitAlignBottomCommand", typeof(ICommand), typeof(DesignerCanvas));

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
            Translating,
            ScalingRotating
        }

        public const double DragThreshold = 0.5;

        private Mode mode;

        private bool mouseDown;
        private bool dragging;
        private Point startPoint;
        private Point previousPoint;

        private ElasticBandAdorner elasticBand;
        private TranslateAdorner translator;

        public IEnumerable<DesignerItem> SelectedItems
        {
            get
            {
                var items = Items;
                var itemscast = items.Cast<object>();
                var selected = itemscast.Select(x => (DesignerItem)ItemContainerGenerator.ContainerFromItem(x));
                var wheres = selected.Where(x => x.IsSelected);

                return wheres;

                //return Items.Cast<object>().Select(x => (DesignerItem)ItemContainerGenerator.ContainerFromItem(x)).Where(x => x.IsSelected);
            }
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

        public ICommand BeginTranslateCommand
        {
            get { return (ICommand)GetValue(BeginTranslateCommandProperty); }
            set { SetValue(BeginTranslateCommandProperty, value); }
        }

        public ICommand CommitTranslateCommand
        {
            get { return (ICommand)GetValue(CommitTranslateCommandProperty); }
            set { SetValue(CommitTranslateCommandProperty, value); }
        }

        public ICommand SendForwardCommand { get; private set; }
        public ICommand SendBackwardCommand { get; private set; }
        public ICommand SendToFrontCommand { get; private set; }
        public ICommand SendToBackCommand { get; private set; }
        public ICommand AlignLeftCommand { get; private set; }
        public ICommand AlignCenterCommand { get; private set; }
        public ICommand AlignRightCommand { get; private set; }
        public ICommand AlignTopCommand { get; private set; }
        public ICommand AlignMiddleCommand { get; private set; }
        public ICommand AlignBottomCommand { get; private set; }

        public ICommand CommitSendForwardCommand
        {
            get { return (ICommand)GetValue(CommitSendForwardCommandProperty); }
            set { SetValue(CommitSendForwardCommandProperty, value); }
        }

        public ICommand CommitSendBackwardCommand
        {
            get { return (ICommand)GetValue(CommitSendBackwardCommandProperty); }
            set { SetValue(CommitSendBackwardCommandProperty, value); }
        }

        public ICommand CommitSendToFrontCommand
        {
            get { return (ICommand)GetValue(CommitSendToFrontCommandProperty); }
            set { SetValue(CommitSendToFrontCommandProperty, value); }
        }

        public ICommand CommitSendToBackCommand
        {
            get { return (ICommand)GetValue(CommitSendToBackCommandProperty); }
            set { SetValue(CommitSendToBackCommandProperty, value); }
        }

        public ICommand CommitAlignLeftCommand
        {
            get { return (ICommand)GetValue(CommitAlignLeftCommandProperty); }
            set { SetValue(CommitAlignLeftCommandProperty, value); }
        }

        public ICommand CommitAlignCenterCommand
        {
            get { return (ICommand)GetValue(CommitAlignCenterCommandProperty); }
            set { SetValue(CommitAlignCenterCommandProperty, value); }
        }

        public ICommand CommitAlignRightCommand
        {
            get { return (ICommand)GetValue(CommitAlignRightCommandProperty); }
            set { SetValue(CommitAlignRightCommandProperty, value); }
        }

        public ICommand CommitAlignTopCommand
        {
            get { return (ICommand)GetValue(CommitAlignTopCommandProperty); }
            set { SetValue(CommitAlignTopCommandProperty, value); }
        }

        public ICommand CommitAlignMiddleCommand
        {
            get { return (ICommand)GetValue(CommitAlignMiddleCommandProperty); }
            set { SetValue(CommitAlignMiddleCommandProperty, value); }
        }

        public ICommand CommitAlignBottomCommand
        {
            get { return (ICommand)GetValue(CommitAlignBottomCommandProperty); }
            set { SetValue(CommitAlignBottomCommandProperty, value); }
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

            SendForwardCommand = new DelegateCommand(p => SelectedItems.Count() > 0, p => SendForward());
            SendBackwardCommand = new DelegateCommand(p => SelectedItems.Count() > 0, p => SendBackward());
            SendToFrontCommand = new DelegateCommand(p => SelectedItems.Count() > 0, p => SendToFront());
            SendToBackCommand = new DelegateCommand(p => SelectedItems.Count() > 0, p => SendToBack());
            AlignLeftCommand = new DelegateCommand(p => SelectedItems.Count() > 1, p => AlignLeft());
            AlignCenterCommand = new DelegateCommand(p => SelectedItems.Count() > 1, p => AlignCenter());
            AlignRightCommand = new DelegateCommand(p => SelectedItems.Count() > 1, p => AlignRight());
            AlignTopCommand = new DelegateCommand(p => SelectedItems.Count() > 1, p => AlignTop());
            AlignMiddleCommand = new DelegateCommand(p => SelectedItems.Count() > 1, p => AlignMiddle());
            AlignBottomCommand = new DelegateCommand(p => SelectedItems.Count() > 1, p => AlignBottom());
        }

        private void SendForward()
        {
            if (null != CommitSendForwardCommand)
            {
                CommitSendForwardCommand.Execute(GetSelectedData());
            }
        }

        private void SendBackward()
        {
            if (null != CommitSendBackwardCommand)
            {
                CommitSendBackwardCommand.Execute(GetSelectedData());
            }
        }

        private void SendToFront()
        {
            if (null != CommitSendToFrontCommand)
            {
                CommitSendToFrontCommand.Execute(GetSelectedData());
            }
        }

        private void SendToBack()
        {
            if (null != CommitSendToBackCommand)
            {
                CommitSendToBackCommand.Execute(GetSelectedData());
            }
        }

        private void AlignLeft()
        {
            if (null != CommitAlignLeftCommand)
            {
                CommitAlignLeftCommand.Execute(GetSelectedData());
            }
        }

        private void AlignCenter()
        {
            if (null != CommitAlignCenterCommand)
            {
                CommitAlignCenterCommand.Execute(GetSelectedData());
            }
        }

        private void AlignRight()
        {
            if (null != CommitAlignRightCommand)
            {
                CommitAlignRightCommand.Execute(GetSelectedData());
            }
        }

        private void AlignTop()
        {
            if (null != CommitAlignTopCommand)
            {
                CommitAlignTopCommand.Execute(GetSelectedData());
            }
        }

        private void AlignMiddle()
        {
            if (null != CommitAlignMiddleCommand)
            {
                CommitAlignMiddleCommand.Execute(GetSelectedData());
            }
        }

        private void AlignBottom()
        {
            if (null != CommitAlignBottomCommand)
            {
                CommitAlignBottomCommand.Execute(GetSelectedData());
            }
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
                    mode = Mode.Translating;

                    if (null != BeginTranslateCommand)
                    {
                        BeginTranslateCommand.Execute(GetSelectedData());
                    }
                }
                else
                {
                    ShowElasticBand();
                }
            }
            else if (mode == Mode.Translating)
            {
                var item = FindDesignerItemAt(currentPoint);
                if (null != item && item.IsSelected)
                {
                    if (null != BeginTranslateCommand)
                    {
                        BeginTranslateCommand.Execute(GetSelectedData());
                    }
                    
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
            else if (mode == Mode.Translating)
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
            else if (mode == Mode.Translating)
            {
                if (null != CommitTranslateCommand)
                {
                    CommitTranslateCommand.Execute(GetSelectedData());
                }
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
            else if (mode == Mode.Translating)
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
                if (null != translator)
                {
                    layer.Remove(translator);
                    translator = null;
                }
            }
        }

        public void Select(DesignerItem item)
        {
            if (!item.IsSelected)
            {
                item.IsSelected = true;

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

                if (SelectedItems.Count() == 0)
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
            if (SelectedItems.Count() == 1)
            {
                var item = SelectedItems.Single();
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
            var previouslySelected = SelectedItems.ToArray();

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

        public void Cut()
        {
            if (null != CutCommand)
            {
                CutCommand.Execute(GetSelectedData());
            }
        }

        public void Copy()
        {
            if (null != CopyCommand)
            {
                CopyCommand.Execute(GetSelectedData());
            }
        }

        public void Paste()
        {
            if (null != PasteCommand)
            {
                PasteCommand.Execute(null);
            }
        }

        public void Delete()
        {
            if (null != DeleteCommand)
            {
                DeleteCommand.Execute(GetSelectedData());
            }
        }

        public void Translate(Vector delta)
        {
            if (mode == Mode.Selecting && SelectedItems.Count() > 0)
            {
                mode = Mode.Translating;
            }

            if (mode == Mode.Translating)
            {
                if (null != BeginTranslateCommand)
                {
                    BeginTranslateCommand.Execute(GetSelectedData());
                }

                TranslateSelection(delta);

                if (null != CommitTranslateCommand)
                {
                    CommitTranslateCommand.Execute(GetSelectedData());
                }
            }
        }
    }
}
