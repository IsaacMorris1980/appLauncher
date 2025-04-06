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
        private int _countPerPage = 1;
        private int _selectedPage = 0;
        private bool _searched = false;
        private string _searchText = string.Empty;
        private ObservableCollection<IApporFolder> SearchList;
        public AppPaginationObservableCollection() 
        {
            originalCollection = new ObservableCollection<IApporFolder>();
            SearchList = new ObservableCollection<IApporFolder>();
        }
        public AppPaginationObservableCollection(IEnumerable<IApporFolder> collection) : base(collection)
        {
            _selectedPage = SettingsHelper.totalAppSettings.LastPageNumber;
            _countPerPage = SettingsHelper.totalAppSettings.AppsPerPage;
            originalCollection = new ObservableCollection<IApporFolder>(collection);
            SearchList = new ObservableCollection<IApporFolder>(collection);
            MainPage.pageChanged += PageChanged;
            MainPage.pageSizeChanged += SizedChanged;
            FirstPage.pageChanged += PageChanged;
            RecalculateThePageItems();
        }
        public AppPaginationObservableCollection SetAllApp
        {
            set
            {
                originalCollection = value;
            }
}
        public void RecalculateThePageItems()
        {


            if (!_searched)
            {



                ClearItems();
                var listofApps = originalCollection.Skip(_selectedPage * _countPerPage).Take(_countPerPage).ToList();

                foreach (var item in listofApps)
                {

                    base.Add(item);
                }
            }
            else
            {
                ClearItems();

                var searchlist = SearchList.Skip(_selectedPage * _countPerPage).Take(_countPerPage).ToList();
                foreach (var item in searchlist)
                {
                    base.Add(item);
                }

            }
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));

        }

        public void AddFolder(AppFolder folder)
        {
            if (originalCollection.Any(x => x.Name == folder.Name))
            {
                return;
            }
            originalCollection.Add(folder);
            RecalculateThePageItems();
        }
        public void UpdateApp(FinalTiles tiles)
        {
            List<FinalTiles> apps = originalCollection.OfType<FinalTiles>().ToList();
            List<AppFolder> folders = originalCollection.OfType<AppFolder>().ToList();
            if (apps.Any(x => x.FullName == tiles.FullName))
            {
                apps[apps.FindIndex(x => x.FullName == tiles.FullName)] = tiles;
            }
            foreach (var item in folders)
            {
                if (item.FolderApps.Any(x => x.FullName == tiles.FullName))
                {                   
                    item.FolderApps[item.FolderApps.FindIndex(x => x.FullName == tiles.FullName)] = tiles;
                }
            }
            List<IApporFolder> lists = new List<IApporFolder>();
            lists.AddRange(apps);
            lists.AddRange(folders);
            originalCollection = new ObservableCollection<IApporFolder>(lists.OrderBy(x => x.ListPos));
            RecalculateThePageItems();
        }
        public void UpdateFolder(AppFolder folder)
        {
            List<FinalTiles> apps = originalCollection.OfType<FinalTiles>().ToList();
            List<AppFolder> folders = originalCollection.OfType<AppFolder>().ToList();
            if (folders.Any(x => x.Name == folder.Name))
            {
                int index = folders.IndexOf(folders.First(x => x.Name == folder.Name));
                folders[index] = folder;

            }
            List<IApporFolder> lists = new List<IApporFolder>();
            lists.AddRange(apps);
            lists.AddRange(folders);
            originalCollection = new ObservableCollection<IApporFolder>(lists);
            RecalculateThePageItems();
        }
        public void Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                _searched = false;
                PageChanged(new PageChangedEventArgs(SettingsHelper.totalAppSettings.LastPageNumber));
            }
            else
            {
                _searched = true;
                SearchList = new ObservableCollection<IApporFolder>(originalCollection.Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToList());
                PageChanged(new PageChangedEventArgs(0));
            }
            RecalculateThePageItems();
        }
        public int GetIndexApp(IApporFolder app)
        {
            return originalCollection.IndexOf(app);
        }
        public void MoveApp(int initailindex, int newindex)
        {
            originalCollection.Move(initailindex, newindex);
            RecalculateThePageItems();
        }
        public void ChangePage(int page)
        {
            _selectedPage = page;
            RecalculateThePageItems();
        }
        public void GetFilteredApps(string selected)
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
            RecalculateThePageItems();
        }
        public ObservableCollection<IApporFolder> GetOriginalCollection()
        {
            return originalCollection;
        }
        public void RemoveApp(string fullname, bool inFolder = false)
        {
            List<IApporFolder> finallist = new List<IApporFolder>();
            if (!inFolder)
            {
                List<FinalTiles> removeapp = originalCollection.OfType<FinalTiles>().ToList();
                removeapp.Remove(x => x.FullName == fullname);
                finallist.AddRange(removeapp);
                finallist.AddRange(originalCollection.OfType<AppFolder>().ToList());
                originalCollection.Clear();
                originalCollection = new ObservableCollection<IApporFolder>(finallist.OrderBy(x => x.Name));
                FirstPage.showMessage.Show("App removed", 1000);
                return;

            }
            else
            {
                List<AppFolder> folders = originalCollection.OfType<AppFolder>().ToList();
                foreach (AppFolder item in folders)
                {
                    if (item.FolderApps.Any(x => x.FullName == fullname))
                    {
                        item.FolderApps.Remove(x => x.FullName == fullname);
                    }
                    finallist.Add(item);
                }
                finallist.AddRange(originalCollection.OfType<FinalTiles>().ToList());
                originalCollection.Clear();
                originalCollection = new ObservableCollection<IApporFolder>(finallist.OrderBy(x => x.Name));

            }

            RecalculateThePageItems();
        }
        public void Removefolder(AppFolder folder)
        {
            List<IApporFolder> finallist = new List<IApporFolder>();
            List<FinalTiles> folderapps = new List<FinalTiles>();
            List<FinalTiles> removeapp = originalCollection.OfType<FinalTiles>().ToList();
            List<AppFolder> removeappfromfolder = originalCollection.OfType<AppFolder>().ToList();

            if (removeappfromfolder.Any(x => x.Name == folder.Name))
            {
                AppFolder a = removeappfromfolder.First(x => x.Name == folder.Name);
                removeapp.AddRange(a.FolderApps);
                removeappfromfolder.Remove(x => x.Name == folder.Name);
            }
            finallist.AddRange(removeapp);
            finallist.AddRange(removeappfromfolder);
            originalCollection.Clear();
            originalCollection = new ObservableCollection<IApporFolder>(finallist.OrderBy(x => x.ListPos).ToList());
            RecalculateThePageItems();
        }
        public void PageChanged(PageChangedEventArgs e)
        {
            if (e.PageIndex != _selectedPage)
            {
                _selectedPage = e.PageIndex;
                RecalculateThePageItems();
            }
        }
        public void SizedChanged(PageSizeEventArgs e)
        {
            if (e.AppPageSize != _countPerPage)
            {
                _countPerPage = e.AppPageSize;
                RecalculateThePageItems();
            }
        }


    }
}