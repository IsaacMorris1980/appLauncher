using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Model
{
    [Serializable]
    public class PaginationObservableCollection : ObservableCollection<Apps>
    {
        private ObservableCollection<Apps> originalCollection;
        [NonSerialized]
        private int Page;
        [NonSerialized]
        private int CountPerPage;
        private int startIndex;
        private int endIndex;

        public PaginationObservableCollection(IEnumerable<Apps> collection) : base(collection)
        {

            Page = 0;
            CountPerPage = 1;
            startIndex = 0;
            endIndex = 1;
            originalCollection = new ObservableCollection<Apps>(collection);
            RecalculateThePageItems();
            GlobalVariables.PageNumChanged += PageChanged;
            GlobalVariables.NumofApps += SizedChanged;
        }

        private void RecalculateThePageItems()
        {
            ClearItems();


            for (int i = startIndex; i < endIndex; i++)
            {
                if (originalCollection.Count > i)
                    base.InsertItem(i - startIndex, originalCollection[i]);
            }
        }









        protected override void InsertItem(int index, Apps item)
        {


            //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                base.InsertItem(index - startIndex, item);

                if (Count > CountPerPage)
                    base.RemoveItem(endIndex);
            }

            if (index >= Count)
                originalCollection.Add(item);
            else
                originalCollection.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            int startIndex = Page * CountPerPage;
            int endIndex = startIndex + CountPerPage;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                this.RemoveAt(index - startIndex);

                if (Count <= CountPerPage)
                    base.InsertItem(endIndex - 1, originalCollection[index + 1]);
            }

            originalCollection.RemoveAt(index);
        }
        public ObservableCollection<Apps> GetOriginalCollection()
        {
            return originalCollection;
        }
        public void PageChanged(PageChangedEventArgs e)
        {
            Page = e.PageIndex;
            startIndex = Page * CountPerPage;
            endIndex = startIndex + CountPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public void SizedChanged(AppPageSizeChangedEventArgs e)
        {
            CountPerPage = e.AppPageSize;
            startIndex = Page * CountPerPage;
            endIndex = startIndex + CountPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

    }

    public static class ExtensionMethods
    {
        public static int Remove<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }
}
