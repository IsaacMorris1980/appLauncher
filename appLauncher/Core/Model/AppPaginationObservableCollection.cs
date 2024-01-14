using appLauncher.Core.CustomEvent;
using appLauncher.Core.Extensions;
using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Pages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
    [Serializable]
    public class AppPaginationObservableCollection : ObservableCollection<IApporFolder>
    {
        private ObservableCollection<IApporFolder> originalCollection;
        [NonSerialized]
        private int _countPerPage = 1;
        private int _numOfPages = 1;
        private int _selectedPage = 0;
        private int _previousSelectedPage = 0;
        private int _previousNumOfPages = 1;
        private List<List<IApporFolder>> _itemLists = new List<List<IApporFolder>>();
        private bool _firstRun;

        public AppPaginationObservableCollection(IEnumerable<IApporFolder> collection) : base(collection)
        {
            _selectedPage = SettingsHelper.totalAppSettings.LastPageNumber;
            _countPerPage = SettingsHelper.totalAppSettings.AppsPerPage;
            originalCollection = new ObservableCollection<IApporFolder>(collection);
            _firstRun = true;
            MainPage.pageChanged += PageChanged;
            MainPage.pageSizeChanged += SizedChanged;
            MainPage.numofPagesChanged += AllPagesChanged;
        }
        public async Task RecalculateThePageItems()
        {
            _itemLists.Clear();
            if (_firstRun)
            {
                for (int i = 0; i < _numOfPages; i++)
                {
                    int startIndex = i * _countPerPage;
                    int endIndex = startIndex + _countPerPage;
                    if (endIndex >= originalCollection.Count)
                    {
                        endIndex = originalCollection.Count - 1;
                    }
                    List<IApporFolder> iapporfolderslist = new List<IApporFolder>();
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        iapporfolderslist.Add(originalCollection[j]);
                    }
                    _itemLists.Add(iapporfolderslist);
                }
                foreach (var item in _itemLists[_selectedPage])
                {
                    if (item.GetType() == typeof(FinalTiles))
                    {
                        await ((FinalTiles)item).SetLogo();
                    }
                    base.Add(item);
                }
            }
            else
            {
                if (_selectedPage == _previousSelectedPage)
                {
                    if (_numOfPages == _previousNumOfPages)
                    {
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < _numOfPages; i++)
                        {
                            _itemLists.Add(originalCollection.Skip(_selectedPage * _countPerPage).Take(_countPerPage).ToList());
                        }
                    }
                }
                else
                {
                    if (_numOfPages == _previousNumOfPages)
                    {

                    }
                    else
                    {
                        for (int i = 0; i < _numOfPages; i++)
                        {
                            List<IApporFolder> apporFolders = new List<IApporFolder>();
                            for (int j = (_selectedPage * _countPerPage); j < ((_selectedPage * _countPerPage) + _countPerPage); j++)
                            {
                                if (j >= originalCollection.Count)
                                {
                                    j = (originalCollection.Count - 1);
                                    apporFolders.Add(originalCollection[j]);
                                    break;
                                }
                                apporFolders.Add(originalCollection[j]);
                            }
                            _itemLists.Add(originalCollection.Skip(_selectedPage * _countPerPage).Take(_countPerPage).ToList());
                        }
                    }
                }

                ClearItems();
                foreach (var item in _itemLists[_previousSelectedPage])
                {
                    if (item.GetType() == typeof(FinalTiles))
                    {
                        ((FinalTiles)item).Logo = Convert.FromBase64String(string.Empty);
                    }
                }
                foreach (var item in _itemLists[_selectedPage])
                {
                    if (item.GetType() == typeof(FinalTiles))
                    {
                        await ((FinalTiles)item).SetLogo();
                    }
                    base.Add(item);
                }
            }
            _firstRun = false;
        }
        public void AddFolder(AppFolder folder)
        {
            originalCollection.Add(folder);
        }
        public void AddApp(FinalTiles tiles)
        {
            originalCollection.Add(tiles);
        }
        public async Task Search(string searchText)
        {
            ClearItems();
            var searchlist = originalCollection.Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToList();
            foreach (var item in _itemLists[_previousSelectedPage])
            {
                if (item.GetType() == typeof(FinalTiles))
                {
                    ((FinalTiles)item).Logo = Convert.FromBase64String(string.Empty);
                }
            }
            foreach (var item in searchlist)
            {
                if (item.GetType() == typeof(FinalTiles))
                {
                    await ((FinalTiles)item).SetLogo();
                }
                base.Add(item);
            }
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));


        }
        public int GetIndexApp(IApporFolder app)
        {
            return originalCollection.IndexOf(app);
        }
        public async void MoveApp(int initailindex, int newindex)
        {
            originalCollection.Move(initailindex, newindex);
            await RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));

        }
        public async void GetFilteredApps(string selected)
        {

            List<IApporFolder> orderList;
            List<FinalTiles> apptiles;
            List<AppFolder> appfolders;
            switch (selected)
            {
                case "alphaAZ":
                    orderList = originalCollection.OrderBy(y => y.Name).ToList();
                    for (int i = 0; i < orderList.Count(); i++)
                    {
                        orderList[i].ListPos = i;
                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "alphaZA":
                    orderList = originalCollection.OrderByDescending(y => y.Name).ToList();
                    for (int i = 0; i < orderList.Count(); i++)
                    {
                        orderList[i].ListPos = i;
                    }
                    originalCollection = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "devAZ":
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
                case "devZA":
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
                case "installNewest":
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
                case "installOldest":
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
            await RecalculateThePageItems();
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        //protected override void InsertItem(int index, IApporFolder item)
        //{
        //    Check if the Index is with in the current Page then add to the collection as bellow.And add to the originalCollection also
        //    if ((index >= _startIndex) && (index < _endIndex))
        //    {
        //        base.InsertItem(index - _startIndex, item);
        //        if (Count > _countPerPage)
        //            base.RemoveItem(_endIndex);
        //    }
        //    if (index >= Count)
        //    {
        //        originalCollection.Add(item);
        //    }
        //    else
        //    {
        //        originalCollection.Insert(index, item);
        //    }
        //}
        //protected override void RemoveItem(int index)
        //{
        //    //int startIndex = _page * _countPerPage;
        //    //int endIndex = startIndex + _countPerPage;
        //    ////Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
        //    //if ((index >= startIndex) && (index < endIndex))
        //    //{
        //    //    this.RemoveAt(index - startIndex);
        //    //    if (Count <= _countPerPage)
        //    //        base.InsertItem(endIndex - 1, originalCollection[index + 1]);
        //    //}
        //    //originalCollection.RemoveAt(index);
        //}
        public ObservableCollection<IApporFolder> GetOriginalCollection()
        {
            return originalCollection;
        }
        public async void RemoveApps(String fullname)
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
            await RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public async void Removefolder(AppFolder folder)
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
            await RecalculateThePageItems();
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public async void PageChanged(PageChangedEventArgs e)
        {
            if (e.PageIndex != _selectedPage)
            {
                _selectedPage = e.PageIndex;
                await RecalculateThePageItems();
                OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            }
        }
        public async void SizedChanged(PageSizeEventArgs e)
        {
            if (e.AppPageSize != _countPerPage)
            {
                _countPerPage = e.AppPageSize;
                await RecalculateThePageItems();
                OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            }
        }
        public async void AllPagesChanged(PageNumChangedArgs e)
        {
            if (e.numofpages != _numOfPages)
            {
                _numOfPages = e.numofpages;
                await RecalculateThePageItems();
                OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
