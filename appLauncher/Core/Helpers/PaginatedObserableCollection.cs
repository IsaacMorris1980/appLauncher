using appLauncher.Core.CustomEvent;
using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;

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
        private int _Pagenum;
        [NonSerialized]
        private int _CountPerPage;
        [NonSerialized]
        private int _startIndex;
        private int startindex
        {
            get
            {
                return _startIndex;
            }
            set
            {
                if (value < 0)
                {
                    _startIndex = 0;
                }
                else
                {
                    _startIndex = value;
                }
            }
        }
        private int _endIndex;
        private int endindex
        {
            get
            {
                return _endIndex;
            }
            set
            {
                if (value >= originalCollection.Count())
                {
                    _endIndex = originalCollection.Count() - 1;
                }
                else
                {
                    _endIndex = value;
                }
            }
        }


        public PaginationObservableCollection(IEnumerable<AppTile> collection) : base(collection)
        {
            GlobalVariables.NumofApps += new AppPageSizeChangedDelegate(NumofApps);
            GlobalVariables.PageNumChanged += new PageChangedDelegate(PagedChanged);
            _Pagenum = 0;
            _CountPerPage = 1;
            originalCollection = new ObservableCollection<AppTile>(collection);
            Analytics.TrackEvent("collection loaded");

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
            startindex = _Pagenum * _CountPerPage;
            endindex = startindex + _CountPerPage;
            for (int i = startindex; i < endindex; i++)
            {
                if (originalCollection.Count > i)
                    base.InsertItem(i - startindex, originalCollection[i]);
            }
        }

        protected override void InsertItem(int index, AppTile item)
        {
            startindex = CurrentPage * PageSize;
            endindex = startindex + PageSize;

            //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
            if ((index >= startindex) && (index < endindex))
            {
                base.InsertItem(index - startindex, item);

                if (Count > _CountPerPage)
                    base.RemoveItem(endindex);
            }
            if (index >= originalCollection.Count)
            {
                originalCollection.Insert(originalCollection.Count - 1, item);
            }
            else
            {
                originalCollection.Insert(index, item);
            }

            //if (index >= Count)
            //{
            //    originalCollection.Add(item);
            //}
            RecalculateThePageItems();
            //else
            //{
            //    originalCollection.Insert(index, item);
            //        }


        }
        protected new void RemoveItem(int index)
        {
            startindex = CurrentPage * PageSize;
            endindex = startindex + PageSize;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startindex) && (index < endindex))
            {
                AppTile ap = this[index - startindex];
                base.Remove(ap);

                if (Count <= PageSize)
                {
                    if (endindex >= originalCollection.Count)
                    {
                        base.InsertItem(originalCollection.Count() - 1, originalCollection[originalCollection.Count() - 1]);
                    }
                    else
                    {
                        base.InsertItem(endindex - 1, originalCollection[index + 1]);
                    }

                }

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
        public void Move(DraggedItem e)
        {
            var a = ((CurrentPage * PageSize) + e.indexonnewpage);
            if (a >= Counts())
            {
                originalCollection.Move(e.initialindex, Counts() - 1);
                return;
            }
            originalCollection.Move(e.initialindex, a);


        }
        public int GetIndexof(AppTile item)
        {
            var a = originalCollection.IndexOf(item);
            return a;
        }
        public int GetAppIndex(AppTile app)
        {
            return base.IndexOf(app);
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
                    _CountPerPage = value;
                    startindex = CurrentPage * PageSize;
                    endindex = startindex + PageSize;
                    RecalculateThePageItems();

                }
            }
        }

        public int CurrentPage
        {
            get { return _Pagenum; }
            set
            {
                if (value >= 0)
                {
                    _Pagenum = value;
                    startindex = CurrentPage * PageSize;
                    endindex = startindex + PageSize;
                    RecalculateThePageItems();

                }
            }
        }
    }
}
