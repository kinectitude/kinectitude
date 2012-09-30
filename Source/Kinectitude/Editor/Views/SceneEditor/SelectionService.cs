using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Views
{
    internal class SelectionService
    {
        private SceneCanvas canvas;
        private List<ISelectable> selectedItems;

        public List<ISelectable> SelectedItems
        {
            get { return selectedItems; }
        }

        public SelectionService(SceneCanvas canvas)
        {
            this.canvas = canvas;

            selectedItems = new List<ISelectable>();
        }

        public void ClearSelection()
        {
            SelectedItems.ForEach(item => item.IsSelected = false);
            SelectedItems.Clear();
        }

        public void SelectItem(ISelectable item)
        {
            ClearSelection();
            AddToSelection(item);
        }

        public void AddToSelection(ISelectable item)
        {
            item.IsSelected = true;
            SelectedItems.Add(item);
        }

        public void RemoveFromSelection(ISelectable item)
        {
            item.IsSelected = false;
            SelectedItems.Remove(item);
        }

        public void SelectAll()
        {
            ClearSelection();
            SelectedItems.AddRange(canvas.Children.OfType<ISelectable>());
            SelectedItems.ForEach(item => item.IsSelected = true);
        }
    }
}
