using appLauncher.Core.CustomEvent;
using appLauncher.Core.Extensions;
using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Pages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            MainPage.PageChanged += PageChanged;
            MainPage.AppNum += SizedChanged;
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
            List<FinalTiles> apptiles;
            List<AppFolder> appfolders;
            switch (selected)
            {
                case "AppAZ":
                    orderList = originalCollection.OrderBy(y => y.Name).ToList();
                    for (int i = 0; i < orderList.Count(); i++)
                    {
                        orderList[i].ListPos = i;
                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "AppZA":
                    orderList = originalCollection.OrderByDescending(y => y.Name).ToList();
                    for (int i = 0; i < orderList.Count(); i++)
                    {
                        orderList[i].ListPos = i;
                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "DevAZ":
                    apptiles = originalCollection.OfType<FinalTiles>().ToList();
                    appfolders = originalCollection.OfType<AppFolder>().ToList();
                    List<FinalTiles> a = apptiles.OrderBy(x => x.Developer).ToList();
                    orderList = new List<IApporFolder>();
                    orderList.AddRange(a);
                    orderList.AddRange(appfolders);
                    for (int i = 0; i < orderList.Count; i++)
                    {
                        if (orderList[i].GetType() == typeof(FinalTiles))
                        {
                            orderList[i].ListPos = i;
                        }
                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "DevZA":
                    apptiles = originalCollection.OfType<FinalTiles>().ToList();
                    appfolders = originalCollection.OfType<AppFolder>().ToList();
                    List<FinalTiles> TilesbyDeveloperName = apptiles.OrderByDescending(x => x.Developer).ToList();
                    orderList = new List<IApporFolder>();
                    orderList.AddRange(TilesbyDeveloperName);
                    orderList.AddRange(appfolders);
                    for (int i = 0; i < orderList.Count; i++)
                    {
                        if (orderList[i].GetType() == typeof(FinalTiles))
                        {
                            orderList[i].ListPos = i;
                        }

                    }

                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "InstalledNewest":
                    apptiles = originalCollection.OfType<FinalTiles>().ToList();
                    appfolders = originalCollection.OfType<AppFolder>().ToList();
                    List<FinalTiles> TilesbyInstalledDate = apptiles.OrderBy(x => x.InstalledDate).ToList();
                    orderList = new List<IApporFolder>();
                    orderList.AddRange(TilesbyInstalledDate);
                    orderList.AddRange(appfolders);
                    for (int i = 0; i < orderList.Count; i++)
                    {
                        if (orderList[i].GetType() == typeof(FinalTiles))
                        {
                            orderList[i].ListPos = i;
                        }

                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "InstalledOldest":
                    apptiles = originalCollection.OfType<FinalTiles>().ToList();
                    appfolders = originalCollection.OfType<AppFolder>().ToList();
                    List<FinalTiles> TilesbyInstalledDates = apptiles.OrderByDescending(x => x.InstalledDate).ToList();
                    orderList = new List<IApporFolder>();
                    orderList.AddRange(TilesbyInstalledDates);
                    orderList.AddRange(appfolders);
                    for (int i = 0; i < orderList.Count; i++)
                    {
                        if (orderList[i].GetType() == typeof(FinalTiles))
                        {
                            orderList[i].ListPos = i;
                        }

                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                default:
                    return;
            }
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
            {
                originalCollection.Add(item);
            }
            else
            {
                originalCollection.Insert(index, item);
            }
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
            List<IApporFolder> finallist = new List<IApporFolder>();
            ObservableCollection<FinalTiles> folderapps = new ObservableCollection<FinalTiles>();
            List<FinalTiles> removeapp = originalCollection.OfType<FinalTiles>().ToList();
            List<AppFolder> removeappfromfolder = originalCollection.OfType<AppFolder>().ToList();
            foreach (AppFolder item in removeappfromfolder)
            {
                removeapp.AddRange(item.FolderApps);
            }
            if (removeapp.Any(x => x.FullName == fullname))
            {
                removeapp.Remove(y => y.FullName == fullname);
            }
            foreach (AppFolder item in removeappfromfolder)
            {
                folderapps = new ObservableCollection<FinalTiles>(removeapp.Where(x => x.FolderName == item.Name).ToList());
                item.FolderApps = folderapps;
                finallist.Add(item);
            }
            finallist.AddRange(removeapp);

            originalCollection.Clear();
            originalCollection = new ObservableCollection<IApporFolder>(finallist.OrderBy(x => x.ListPos).ToList());
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public void Removefolder(AppFolder folder)
        {
            List<IApporFolder> finallist = new List<IApporFolder>();
            ObservableCollection<FinalTiles> folderapps = new ObservableCollection<FinalTiles>();
            List<FinalTiles> removeapp = originalCollection.OfType<FinalTiles>().ToList();
            List<AppFolder> removeappfromfolder = originalCollection.OfType<AppFolder>().ToList();

            if (removeappfromfolder.Any(x => x.Name == folder.Name))
            {
                var a = removeappfromfolder.First(x => x.Name == folder.Name);
                folderapps = a.FolderApps;
                removeapp.AddRange(folderapps);
                removeappfromfolder.Remove(x => x.Name == folder.Name);
            }
            finallist.AddRange(removeapp);
            finallist.AddRange(removeappfromfolder);
            originalCollection.Clear();
            originalCollection = new ObservableCollection<IApporFolder>(finallist.OrderBy(x => x.ListPos).ToList());
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
        public void SizedChanged(PageSizeEventArgs e)
        {
            _countPerPage = e.AppPageSize;
            _startIndex = _page * _countPerPage;
            _endIndex = _startIndex + _countPerPage;
            RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
}
