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

        public AppPaginationObservableCollection(IEnumerable<IApporFolder> collection) : base(collection)
        {
            _selectedPage = SettingsHelper.totalAppSettings.LastPageNumber;
            _countPerPage = SettingsHelper.totalAppSettings.AppsPerPage;
            originalCollection = new ObservableCollection<IApporFolder>(collection);
            SearchList = new ObservableCollection<IApporFolder>(collection);
            MainPage.pageChanged += PageChanged;
            MainPage.pageSizeChanged += SizedChanged;
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
            if (apps.Any(x => x.ListPos == tiles.ListPos))
            {
                apps[tiles.ListPos] = tiles;
            }
            foreach (var item in folders)
            {
                if (item.FolderApps.Any(x => x.Name == tiles.Name))
                {
                    var index = item.FolderApps.IndexOf(item.FolderApps.First(x => x.Name == tiles.Name));
                    item.FolderApps[index] = tiles;
                }

            }
            List<IApporFolder> lists = new List<IApporFolder>();
            lists.AddRange(apps);
            lists.AddRange(folders);
            originalCollection = new ObservableCollection<IApporFolder>();
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
            originalCollection = new ObservableCollection<IApporFolder>();
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
        public void RemoveApp(string fullname)
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
