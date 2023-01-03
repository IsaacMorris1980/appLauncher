using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using UwpAppLauncher.Interfaces;
using UwpAppLauncher.Model;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UwpAppLauncher.Helpers
{
    public sealed class packageHelper
    {
        public ReadOnlyObservableCollection<AppTile> searchApps { get; set; }
        public ObservableCollection<IApporFolder> appTiles { get; set; }
        public async Task LoadCollectionAsync()
        {
            List<FolderTile> orderedfolderlist = new List<FolderTile>();
            List<AppTile> orderedapplist;
            List<AppTile> apporder = new List<AppTile>();
            List<AppTile> listapptiles = await GetTiles();
            searchApps = new ReadOnlyObservableCollection<AppTile>(new ObservableCollection<AppTile>(listapptiles.OrderByDescending(x => x.AppFullName).ToList()));
            if (await ((App)Application.Current).globalVariables.IsFilePresent("collection.json"))
            {
                try
                {

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    orderedapplist = JsonConvert.DeserializeObject<List<AppTile>>(apps);
                    for (int i = 0; i < orderedapplist.Count() - 1; i++)
                    {
                        AppTile app = await ((App)Application.Current).packageHelper.GetApp(orderedapplist[i].AppFullName);
                        app.BackgroundColor = orderedapplist[i].BackgroundColor;
                        app.ForegroundColor = orderedapplist[i].ForegroundColor;
                        app.TextColor = orderedapplist[i].TextColor;
                        app.TextOpacity = orderedapplist[i].TextOpacity;
                        app.ForegroundOpacity = orderedapplist[i].ForegroundOpacity;
                        app.BackgroundOpacity = orderedapplist[i].BackgroundOpacity;
                        apporder.Add(app);
                    }
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during reordering app list to last apps position");
                    Crashes.TrackError(e);
                }





            }
            if (await ((App)Application.Current).globalVariables.IsFilePresent("folders.json"))
            {

                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("folders.json");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                orderedfolderlist = JsonConvert.DeserializeObject<List<FolderTile>>(apps);
                for (int i = 0; i < orderedfolderlist.Count(); i++)
                {
                    List<AppTile> appsinfolder = orderedfolderlist[i].FolderApps;

                    foreach (AppTile items in appsinfolder)
                    {
                        AppTile app = await ((App)Application.Current).packageHelper.GetApp(items.AppFullName);
                        app.BackgroundColor = ((AppTile)items).BackgroundColor;
                        app.ForegroundColor = ((AppTile)items).ForegroundColor;
                        app.TextColor = ((AppTile)items).TextColor;
                        app.BackgroundOpacity = ((AppTile)items).BackgroundOpacity;
                        app.ForegroundOpacity = ((AppTile)items).ForegroundOpacity;
                        app.TextOpacity = ((AppTile)items).TextOpacity;
                        // items = app;
                    }
                    for (int j = 0; j < appsinfolder.Count(); j++)
                    {
                        IApporFolder items = orderedfolderlist[i].FolderApps[j];
                        if (items is AppTile)
                        {

                            //   orderedfolderlist[i].FolderApps[j] = app;
                        }

                    }

                }
            }
            IEnumerable<IApporFolder> appandfolder = apporder.Concat<IApporFolder>(orderedfolderlist);
            if (appandfolder.Count() > 0)
            {
                appTiles = new ObservableCollection<IApporFolder>(appandfolder.OrderBy(x => x.Listlocation));

            }
            else
            {

                appTiles = new ObservableCollection<IApporFolder>(listapptiles.Cast<IApporFolder>().OrderBy(x => x.Listlocation).ToList());
            }

        }
        //public async Task<FolderTile> GetFolder(string Foldername)
        //{
        //    List<IApporFolder> appsinfolder = orderedfolderlist[i].FolderApps;
        //    for (int j = 0; j < appsinfolder.Count(); j++)
        //    {
        //        IApporFolder items = orderedfolderlist[i].FolderApps[j];
        //        if (items is AppTile)
        //        {
        //            AppTile app = await ((App)Application.Current).packageHelper.GetApp(((AppTile)items).AppFullName);
        //            app.BackgroundColor = ((AppTile)items).BackgroundColor;
        //            app.ForegroundColor = ((AppTile)items).ForegroundColor;
        //            app.TextColor = ((AppTile)items).TextColor;
        //            app.BackgroundOpacity = ((AppTile)items).BackgroundOpacity;
        //            app.ForegroundOpacity = ((AppTile)items).ForegroundOpacity;
        //            app.TextOpacity = ((AppTile)items).TextOpacity;
        //            orderedfolderlist[i].FolderApps[j] = app;
        //        }

        //    }
        //}
        public async Task<List<AppTile>> GetTiles()
        {
            List<AppTile> appTiles = new List<AppTile>();
            try
            {
                PackageManager packageManager = new PackageManager();
                List<Package> appslist = (List<Package>)packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
                int location = 0;
                for (int i = 0; i < appslist.Count() - 1; i++)
                {
                    Package pack = appslist[i];
                    IReadOnlyList<AppListEntry> entries = await pack.GetAppListEntriesAsync();
                    if (entries.Count() > 0)
                    {
                        try
                        {
                            AppTile app = new AppTile
                            {
                                AppEntry = entries[0],
                                Listlocation = location,
                                Name = entries[0].DisplayInfo.DisplayName,
                                AppDescription = pack.Description,
                                AppFullName = pack.Id.FullName,
                                AppDeveloper = pack.PublisherDisplayName,
                                AppInstalledDate = pack.InstallDate
                            };
                            byte[] temp = await ((App)Application.Current).imageHelper.ExtractLogo(entries[0]);
                            app.AppFullSizeLogo = temp;
                            app.FolderSizeLogo = await ((App)Application.Current).imageHelper.ResizeImage(temp, 10, 10);
                            appTiles.Add(app);
                            location++;
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            AppTile apps = new AppTile
                            {
                                AppEntry = entries[0],
                                Listlocation = location,
                                AppFullName = pack.Id.FullName,
                                AppDescription = pack.Description,
                                AppDeveloper = pack.PublisherDisplayName,
                                AppInstalledDate = pack.InstallDate,
                                Name = entries[0].DisplayInfo.DisplayName,

                            };
                            byte[] temp = await ((App)Application.Current).imageHelper.ExtractLogo(entries[0]);
                            apps.AppFullSizeLogo = temp;
                            apps.FolderSizeLogo = await ((App)Application.Current).imageHelper.ResizeImage(temp, 10, 10);
                            appTiles.Add(apps);
                            location++;
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return appTiles;
        }
        public async Task<AppListEntry> GetEntry(string fullname)
        {
            PackageManager pkgmgr = new PackageManager();
            Package pack = pkgmgr.FindPackage(fullname);
            IReadOnlyList<AppListEntry> entries = await pack.GetAppListEntriesAsync();
            return entries[0];


        }
        public async Task<AppTile> GetApp(string fullname)
        {
            PackageManager pkgmgr = new PackageManager();
            Package pac = pkgmgr.FindPackage(fullname);
            IReadOnlyList<AppListEntry> entries = await pac.GetAppListEntriesAsync();
            AppTile app = new AppTile
            {
                AppEntry = entries[0],
                AppFullName = fullname,
                Name = entries[0].DisplayInfo.DisplayName,
                AppDescription = pac.Description,
                AppDeveloper = pac.PublisherDisplayName,
                AppInstalledDate = pac.InstallDate
            };
            byte[] temp = await ((App)Application.Current).imageHelper.ExtractLogo(entries[0]);
            app.AppFullSizeLogo = temp;
            app.FolderSizeLogo = await ((App)Application.Current).imageHelper.ResizeImage(temp, 10, 10);
            return app;


        }
        public async Task SaveCollectionAsync()
        {
            try
            {
                List<AppTile> savapps = appTiles.Cast<AppTile>().ToList();
                List<FolderTile> folders = appTiles.Cast<FolderTile>().ToList();
                if (savapps.Count > 0)
                {
                    string appstring = JsonConvert.SerializeObject(savapps, Formatting.Indented);
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(item, appstring);
                }
                if (folders.Count > 0)
                {
                    string folderstring = JsonConvert.SerializeObject(folders, Formatting.Indented);
                    StorageFile folderfile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("folders.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(folderfile, folderstring);
                }
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during saving app list positions");
                Crashes.TrackError(es);
            }
        }

    }
}
