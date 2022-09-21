using appLauncher.Core.CustomEvent;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Helpers
{
    [Serializable]
    public class PaginationObservableCollection : ObservableCollection<AppTile>, IList<AppTile>
    {
        private ObservableCollection<AppTile> originalCollection;
        [NonSerialized]
        private int Pagenum;
        [NonSerialized]
        private int CountPerPage;
        [NonSerialized]
        private int startindex;
        [NonSerialized]
        private int endindex;


        public PaginationObservableCollection(IEnumerable<AppTile> collection) : base(collection)
        {
            GlobalVariables.NumofApps += new AppPageSizeChangedDelegate(NumofApps);
            GlobalVariables.PageNumChanged += new PageChangedDelegate(PagedChanged);
            Pagenum = 0;
            CountPerPage = 1;
            startindex = Pagenum * CountPerPage;
            endindex = startindex + CountPerPage;
            originalCollection = new ObservableCollection<AppTile>(collection);
            RecalculateThePageItems();



        }

        private void PagedChanged(PageChangedEventArgs e)
        {
            CurrentPage = e.PageIndex;
        }

        private void NumofApps(AppPageSizeChangedEventArgs e)
        {
            PageSize = e.AppPageSize;
        }

        private void RecalculateThePageItems()
        {
            ClearItems();
            int startIndex = Pagenum * CountPerPage;
            int endIndex = startIndex + CountPerPage;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (originalCollection.Count > i)
                    base.InsertItem(i - startIndex, originalCollection[i]);
            }
        }

        protected override void InsertItem(int index, AppTile item)
        {
            //int startIndex = Page * CountPerPage;
            //int endIndex = startIndex + CountPerPage;

            //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
            if ((index >= startindex) && (index < endindex))
            {
                base.InsertItem(index - startindex, item);

                if (Count > CountPerPage)
                    base.RemoveItem(endindex);
            }

            if (index >= Count)
            {
                originalCollection.Add(item);
            }
            RecalculateThePageItems();
            //else
            //{
            //    originalCollection.Insert(index, item);
            //        }


        }
        protected override void RemoveItem(int index)
        {
            //int startIndex = Page * CountPerPage;
            //int endIndex = startIndex + CountPerPage;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startindex) && (index < endindex))
            {
                this.RemoveAt(index - startindex);

                if (Count <= CountPerPage)
                    base.InsertItem(endindex - 1, originalCollection[index + 1]);
            }

            originalCollection.RemoveAt(index);
        }

        public List<AppTile> GetInternalList()
        {
            return originalCollection.ToList();
        }
        public int Counts()
        {
            return originalCollection.Count();
        }
        public void Sort(string typetosort)
        {
            IOrderedEnumerable<AppTile> orderlist;
            switch (typetosort)
            {
                case "AtoZ":
                    orderlist = this.originalCollection.OrderBy(y => y.appname);
                    break;
                case "Developer":
                    orderlist = this.originalCollection.OrderBy(x => x.appdeveloper);
                    break;

                case "Installed":
                    orderlist = this.originalCollection.OrderBy(x => x.appinstalleddate);
                    break;

                default:
                    orderlist = this.OrderBy(y => y);
                    return;

            }
            originalCollection = new ObservableCollection<AppTile>(orderlist);
            RecalculateThePageItems();
        }

        public int GetIndexof(AppTile item)
        {
            var a = originalCollection.IndexOf(item);
            return a;
        }
        public void Moved(int oldindex, int newindex, AppTile itemtomove)
        {
            RemoveItem(oldindex);
            InsertItem(newindex, itemtomove);
        }

        public int PageSize
        {
            get { return Count; }
            set
            {
                if (value >= 0)
                {
                    CountPerPage = value;
                    startindex = Pagenum * CountPerPage;
                    endindex = startindex + CountPerPage;
                    RecalculateThePageItems();

                }
            }
        }

        public int CurrentPage
        {
            get { return Pagenum; }
            set
            {
                if (value >= 0)
                {
                    Pagenum = value;
                    startindex = Pagenum * CountPerPage;
                    endindex = startindex + CountPerPage;
                    RecalculateThePageItems();

                }
            }
        }
    }
}
