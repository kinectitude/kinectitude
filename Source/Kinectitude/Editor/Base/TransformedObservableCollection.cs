using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Base
{
    internal sealed class TransformedObservableCollection<TInput, TOutput> : ObservableCollection<TOutput>
    {
        private readonly ObservableCollection<TInput> input;
        private readonly Func<TInput, TOutput> func;

        public TransformedObservableCollection(ObservableCollection<TInput> input, Func<TInput, TOutput> func)
        {
            this.input = input;
            this.func = func;

            input.CollectionChanged += OnCollectionChanged;
            foreach (var item in input)
            {
                this.Add(func(item));
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.Insert(e.NewStartingIndex, func((TInput)e.NewItems[0]));
                    return;
                case NotifyCollectionChangedAction.Move:
                    this.Move(e.OldStartingIndex, e.NewStartingIndex);
                    return;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveAt(e.OldStartingIndex);
                    return;
                case NotifyCollectionChangedAction.Replace:
                    this[e.OldStartingIndex] = func((TInput)e.NewItems[0]);
                    return;
            }

            this.Clear();
            foreach (var item in input)
            {
                this.Add(func(item));
            }
        }
    }
}
