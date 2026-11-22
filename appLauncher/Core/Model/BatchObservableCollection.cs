using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
    /// <summary>
    /// Thin ObservableCollection wrapper that replaces items in a single
    /// NotifyCollectionChangedAction.Reset pass to prevent UI layout thrashing.
    /// </summary>
    public class BatchObservableCollection<T> : ObservableCollection<T>
    {
        public BatchObservableCollection() : base()
        {
        }

        public BatchObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public void ReplaceRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            CheckReentrancy();
            Items.Clear();

            foreach (var item in items)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
