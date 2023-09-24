using appLauncher.Core.CustomEvent;
using appLauncher.Core.Extensions;
using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace appLauncher.Core.Model
{
    [Serializable]
    public class AppPaginationObservableCollection : ObservableCollection<IApporFolder>
    {
        private ObservableCollection<IApporFolder> originalCollection;
        [NonSerialized]
        private int _page;
        [NonSerialized]
        private int _countPerPage;
        private int _startIndex;
        private int _endIndex;

        public AppPaginationObservableCollection(IEnumerable<IApporFolder> collection) : base(collection)
        {

            _page = SettingsHelper.totalAppSettings.LastPageNumber;
            _countPerPage = SettingsHelper.totalAppSettings.AppsPerPage;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            originalCollection = new ObservableCollection<IApporFolder>(collection);
            RecalculateThePageItems();
            GlobalVariables.PageNumChanged += PageChanged;
            GlobalVariables.NumofApps += SizedChanged;
        }

        private void RecalculateThePageItems()
        {
            ClearItems();


            for (int i = _startIndex; i < _endIndex; i++)
            {
                if (originalCollection.Count > i)
                    base.InsertItem(i - _startIndex, originalCollection[i]);
            }
        }

        public int GetIndexApp(IApporFolder app)
        {
            return originalCollection.IndexOf(app);
        }
        public void MoveApp(int initailindex, int newindex)
        {
            originalCollection.Move(initailindex, newindex);
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));

        }
        public void GetFilteredApps(string selected)
        {

            List<IApporFolder> orderList;
            //switch (selected)
            //{
            //    case "AppAZ":
            //        orderList = originalCollection.OrderBy(y => y.Name).ToList();
            //        originalCollection = new ObservableCollection<AppTiles>(orderList);
            //        break;
            //    case "AppZA":
            //        orderList = originalCollection.OrderByDescending(y => y.Name).ToList();
            //        originalCollection = new ObservableCollection<AppTiles>(orderList);
            //        break;
            //    case "DevAZ":
            //        orderList = originalCollection.OrderBy(x => x.Developer).ToList();
            //        originalCollection = new ObservableCollection<AppTiles>(orderList);
            //        break;
            //    case "DevZA":
            //        orderList = originalCollection.OrderByDescending(x => x.Developer).ToList();
            //        originalCollection = new ObservableCollection<AppTiles>(orderList);
            //        break;
            //    case "InstalledNewest":
            //        orderList = originalCollection.OrderByDescending(x => x.InstalledDate).ToList();
            //        originalCollection = new ObservableCollection<AppTiles>(orderList);
            //        break;
            //    case "InstalledOldest":
            //        orderList = originalCollection.OrderBy(x => x.InstalledDate).ToList();
            //        originalCollection = new ObservableCollection<AppTiles>(orderList);
            //        break;
            //    default:
            //        return;
            //}
            RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        protected override void InsertItem(int index, IApporFolder item)
        {
            //Check if the Index is with in the current Page then add to the collection as bellow. And add to the originalCollection also
            if ((index >= _startIndex) && (index < _endIndex))
            {
                base.InsertItem(index - _startIndex, item);
                if (Count > _countPerPage)
                    base.RemoveItem(_endIndex);
            }
            if (index >= Count)
                originalCollection.Add(item);
            else
                originalCollection.Insert(index, item);
        }
        protected override void RemoveItem(int index)
        {
            int startIndex = _page * _countPerPage;
            int endIndex = startIndex + _countPerPage;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                this.RemoveAt(index - startIndex);
                if (Count <= _countPerPage)
                    base.InsertItem(endIndex - 1, originalCollection[index + 1]);
            }
            originalCollection.RemoveAt(index);
        }
        public ObservableCollection<IApporFolder> GetOriginalCollection()
        {
            return originalCollection;
        }
        public void RemoveApps(String fullname)
        {
            originalCollection.Remove(x => x.Name == fullname);
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public void PageChanged(PageChangedEventArgs e)
        {
            _page = e.PageIndex;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public void SizedChanged(AppPageSizeChangedEventArgs e)
        {
            _countPerPage = e.AppPageSize;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
}
