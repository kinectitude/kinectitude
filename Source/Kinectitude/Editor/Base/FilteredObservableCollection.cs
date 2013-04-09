//-----------------------------------------------------------------------
// <copyright file="FilteredObservableCollection.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Kinectitude.Editor.Base
{
    internal sealed class FilteredObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Predicate<T> filter;

        public FilteredObservableCollection(ObservableCollection<T> items, Predicate<T> filter)
        {
            items.CollectionChanged += Items_CollectionChanged;
            this.filter = filter;

            foreach (T item in items)
            {
                if (filter(item))
                {
                    this.Add(item);
                }
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (T item in e.NewItems)
                {
                    if (filter(item))
                    {
                        this.Add(item);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (T item in e.OldItems)
                {
                    this.Remove(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Clear();
                foreach (T item in e.NewItems)
                {
                    if (filter(item))
                    {
                        this.Add(item);
                    }
                }
            }
        }
    }
}
