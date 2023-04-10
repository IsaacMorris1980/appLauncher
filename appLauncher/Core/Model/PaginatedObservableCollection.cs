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

            startIndex = Page = 0;
            endIndex = CountPerPage = 1;
            originalCollection = new ObservableCollection<Apps>(collection);
            GlobalVariables.PageNumChanged += PageChanged;
            GlobalVariables.NumofApps += SizedChanged;
            RecalculateThePageItems();
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
        public int GetIndexApp(Apps app)
        {
            return originalCollection.IndexOf(app);
        }
        public void MoveApp(DraggedItem item)
        {
            originalCollection.Move(item.initialindex, item.indexonnewpage);
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Replace));

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
            {
                originalCollection.Add(item);
            }
            else
            {
                originalCollection.Insert(index, item);
            }

            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add));
        }


        protected override void RemoveItem(int index)
        {

            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                this.RemoveAt(index - startIndex);

                if (Count <= CountPerPage)
                    base.InsertItem(endIndex - 1, originalCollection[index + 1]);
            }

            originalCollection.RemoveAt(index);
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove));
        }
        public ObservableCollection<Apps> GetOriginalCollection()
        {
            return originalCollection;
        }
        public void GetFilteredApps(string selected)
        {

            ObservableCollection<Apps> orderlist;
            switch (selected)
            {
                case "AtoZ":
                    orderlist = originalCollection.OrderBy(y => y.Name).ToObservableCollection();
                    originalCollection = orderlist;
                    break;
                case "Developer":
                    orderlist = originalCollection.OrderBy(x => x.Developer).ToObservableCollection();
                    originalCollection = orderlist;
                    break;
                case "Installed":
                    orderlist = originalCollection.OrderBy(x => x.InstalledDate).ToObservableCollection();
                    originalCollection = orderlist;// new ObservableCollection<Apps>(orderlist);
                    break;

                default:
                    return;


            }
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
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
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }
}
