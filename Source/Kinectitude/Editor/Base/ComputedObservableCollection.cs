using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;

namespace Kinectitude.Editor.Base
{
    internal sealed class ComputedObservableCollection<TInput, TOutput> : IList<TOutput>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly ObservableCollection<TInput> inputItems;
        private readonly ObservableCollection<TOutput> outputItems;
        private readonly Func<TInput, TOutput> function;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { outputItems.CollectionChanged += value; }
            remove { outputItems.CollectionChanged -= value; }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { ((INotifyPropertyChanged)outputItems).PropertyChanged += value; }
            remove { ((INotifyPropertyChanged)outputItems).PropertyChanged -= value; }
        }

        public TOutput this[int index]
        {
            get { return outputItems[index]; }
            set { throw new NotSupportedException(); }
        }

        public int Count
        {
            get { return outputItems.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ComputedObservableCollection(ObservableCollection<TInput> items, Func<TInput, TOutput> function)
        {
            this.function = function;

            inputItems = items;
            outputItems = new ObservableCollection<TOutput>();

            foreach (TInput item in items)
            {
                PrivateAdd(item);
            }

            items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int index = e.NewStartingIndex;
                foreach (TInput input in e.NewItems)
                {
                    PrivateInsert(index, input);
                    index++;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                PrivateMove(e.OldStartingIndex, e.NewStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                int index = e.OldStartingIndex;
                foreach (TInput input in e.OldItems)
                {
                    PrivateRemoveAt(index);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                int index = e.OldStartingIndex;
                PrivateReplace(index, (TInput)e.NewItems[0]);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                outputItems.Clear();
                foreach (TInput input in e.NewItems)
                {
                    PrivateAdd(input);
                }
            }
        }

        private void PrivateAdd(TInput input)
        {
            TOutput output = function(input);
            outputItems.Add(output);
        }

        private void PrivateInsert(int index, TInput input)
        {
            TOutput output = function(input);
            outputItems.Insert(index, output);
        }

        private void PrivateReplace(int index, TInput input)
        {
            TOutput output = function(input);
            outputItems[index] = output;
        }

        private void PrivateRemove(TInput input)
        {
            int index = inputItems.IndexOf(input);
            if (index != -1)
            {
                PrivateRemoveAt(index);
            }
        }

        private void PrivateRemoveAt(int index)
        {
            outputItems.RemoveAt(index);
        }

        private void PrivateMove(int oldIndex, int newIndex)
        {
            outputItems.Move(oldIndex, newIndex);
        }

        public int IndexOf(TOutput item)
        {
            return outputItems.IndexOf(item);
        }

        public void Insert(int index, TOutput item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(TOutput item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TOutput item)
        {
            return outputItems.Contains(item);
        }

        public void CopyTo(TOutput[] array, int arrayIndex)
        {
            outputItems.CopyTo(array, arrayIndex);
        }

        public bool Remove(TOutput item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<TOutput> GetEnumerator()
        {
            return outputItems.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)outputItems).GetEnumerator();
        }
    }
}
