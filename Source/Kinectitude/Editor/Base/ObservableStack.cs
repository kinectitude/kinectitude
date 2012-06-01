using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Kinectitude.Editor.Base
{
    internal class ObservableStack<T> : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> collection;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { ((INotifyPropertyChanged)collection).PropertyChanged += value; }
            remove { ((INotifyPropertyChanged)collection).PropertyChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { collection.CollectionChanged += value; }
            remove { collection.CollectionChanged -= value; }
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public ObservableStack()
        {
            collection = new ObservableCollection<T>();
        }

        public void Push(T item)
        {
            collection.Add(item);
        }

        public T Pop()
        {
            T ret = default(T);
            if (collection.Count > 0)
            {
                ret = collection.ElementAtOrDefault(collection.Count - 1);
                collection.RemoveAt(collection.Count - 1);
            }
            return ret;
        }

        public T Peek()
        {
            return collection.ElementAtOrDefault(collection.Count - 1);
        }

        public void Clear()
        {
            collection.Clear();
        }
    }
}
