using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace appLauncher.Core.Helpers
{
    public class TestPackageHelper : ModelBase
    {
        public ObservableCollection<IApporFolder> AppsAndFolders = new ObservableCollection<IApporFolder>();
        private List<IApporFolder> bindableList = new List<IApporFolder>();
        private int selectedPage = 0;
        private int appsPerPage = 0;
        private bool isSearched = false;
        private bool isFiltered = false;
        private string searchByText = string.Empty;
        public void SearchResults(string searchText)
        {
            if (AppsAndFolders.Count <= 0)
            {
                isSearched = false;
                return;
            }
            if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
            {
                isSearched = false;
                bindableList.Clear();
                bindableList = AppsAndFolders.OrderBy(x => x.Name).Skip(selectedPage * appsPerPage).Take(appsPerPage).ToList();
                return;
            }
            isSearched = true;

            bindableList = AppsAndFolders.Where(x => x.Name.ToLower().StartsWith(searchText.ToLower())).Skip(0 * appsPerPage).Take(appsPerPage).ToList();
        }
        public void MoveApp(int oldIndex, int newIndex)
        {
            AppsAndFolders.Move(oldIndex, newIndex);
            bindableList = AppsAndFolders.Skip(selectedPage * appsPerPage).Take(appsPerPage).OrderBy(x => x.Name).ToList();
        }
        public void AddApp(FinalTiles app)
        {
            AppsAndFolders.Add(app);
            bindableList = AppsAndFolders.OrderBy(x => x.Name).Skip(selectedPage * appsPerPage).Take(appsPerPage).ToList();
        }
        public void Addfolder(AppFolder folder)
        {
            AppsAndFolders.Add(folder);
            bindableList = AppsAndFolders.OrderBy(x => x.Name).Skip(selectedPage * appsPerPage).Take(appsPerPage).ToList();
        }
        public void UpdateApp(FinalTiles tile)
        {
            List<FinalTiles> allApps = AppsAndFolders.OfType<FinalTiles>().ToList();

            for (int i = 0; i < allApps.Count; i++)
            {
                if (allApps[i].FullName == tile.FullName)
                {
                    allApps[i] = tile;
                }
            }
            List<AppFolder> allFolders = AppsAndFolders.OfType<AppFolder>().ToList();
            for (int i = 0; i < allFolders.Count; i++)
            {
                List<FinalTiles> allFolderApps = allFolders[i].FolderApps;
                for (int j = 0; j < allFolderApps.Count; j++)
                {

                    if (allFolderApps[i].FullName == tile.FullName)
                    {
                        allFolderApps[i] = tile;
                        return;
                    }
                }

            }
        }
        public void UpdaetFolder(AppFolder folder)
        {
            List<AppFolder> allFolders = AppsAndFolders.OfType<AppFolder>().ToList();
            for (int i = 0; i < allFolders.Count; i++)
            {
                if (allFolders[i].Name == folder.Name)
                {
                    allFolders[i] = folder;
                    return;
                }
            }
        }
        public void ChnagePage(int pageNumber)
        {
            selectedPage = pageNumber;
            bindableList = AppsAndFolders.OrderBy(x => x.Name).Skip(selectedPage * appsPerPage).Take(appsPerPage).ToList();
        }
        public int GetIndex(IApporFolder item)
        {
            return AppsAndFolders.IndexOf(AppsAndFolders.First(x => x.Name == item.Name));
        }
        public void FilterApps(string filter)
        {
            List<IApporFolder> orderList;
            List<FinalTiles> apptiles;
            List<AppFolder> appfolders;
            switch (filter)
            {
                case "alphaAZ":
                    orderList = AppsAndFolders.OrderBy(y => y.Name).ToList();
                    for (int i = 0; i < orderList.Count(); i++)
                    {
                        orderList[i].ListPos = i;
                    }
                    AppsAndFolders = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "alphaZA":
                    orderList = AppsAndFolders.OrderByDescending(y => y.Name).ToList();
                    for (int i = 0; i < orderList.Count(); i++)
                    {
                        orderList[i].ListPos = i;
                    }
                    AppsAndFolders = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "devAZ":
                    apptiles = AppsAndFolders.OfType<FinalTiles>().ToList();
                    appfolders = AppsAndFolders.OfType<AppFolder>().ToList();
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
                    AppsAndFolders = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "devZA":
                    apptiles = AppsAndFolders.OfType<FinalTiles>().ToList();
                    appfolders = AppsAndFolders.OfType<AppFolder>().ToList();
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
                    AppsAndFolders = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "installNewest":
                    apptiles = AppsAndFolders.OfType<FinalTiles>().ToList();
                    appfolders = AppsAndFolders.OfType<AppFolder>().ToList();
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
                    AppsAndFolders = new ObservableCollection<IApporFolder>(orderList);
                    break;
                case "installOldest":
                    apptiles = AppsAndFolders.OfType<FinalTiles>().ToList();
                    appfolders = AppsAndFolders.OfType<AppFolder>().ToList();
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
                    AppsAndFolders = new ObservableCollection<IApporFolder>(orderList);


                    break;
                default:
                    return;
            }
            SetProperty(ref bindableList, AppsAndFolders.Skip(selectedPage * appsPerPage).Take(appsPerPage).ToList());
        }

    }
}
