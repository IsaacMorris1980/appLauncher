using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UwpAppLauncher.Model;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;

namespace UwpAppLauncher.Helpers
{
    public sealed class OldAppCompatibility
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public async Task<List<PageBackground>> ConvertOldPageBackgroundFileAsync()
        {
            List<PageBackground> pageBackgrounds = new List<PageBackground>();
            StorageFolder backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> filesinfolder = await backgroundImageFolder.GetFilesAsync();

            if (await ((App)Application.Current).globalVariables.IsFilePresent("images.txt"))
            {

                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                string apps = await FileIO.ReadTextAsync(item);
                List<OldPageBackground> oldback = JsonConvert.DeserializeObject<List<OldPageBackground>>(apps);

                IEnumerable<StorageFile> newimagesnotsaved = filesinfolder.Where(x => !oldback.Any(y => y.BackgroundImageDisplayName == x.DisplayName));
                if (newimagesnotsaved.Count() <= 0)
                {
                    foreach (OldPageBackground items in oldback)
                    {
                        PageBackground pb = new PageBackground();
                        StorageFile sf = await ApplicationData.Current.LocalFolder.TryGetItemAsync(items.BackgroundImageDisplayName) as StorageFile;
                        pb.PageBackgroundName = items.BackgroundImageDisplayName;
                        if (sf != null)
                        {
                            pb.PageBackgroundImage = await ((App)Application.Current).imageHelper.ConvertImageFiletoByteArrayAsync(sf);

                        }
                        else
                        {
                            pb.PageBackgroundImage = new byte[1];
                        }
                        pageBackgrounds.Add(pb);
                    }
                    return pageBackgrounds;
                }
                else
                {
                    foreach (OldPageBackground items in oldback)
                    {
                        PageBackground pb = new PageBackground();
                        StorageFile sf = await ApplicationData.Current.LocalFolder.TryGetItemAsync(items.BackgroundImageDisplayName) as StorageFile;
                        pb.PageBackgroundName = items.BackgroundImageDisplayName;
                        if (sf != null)
                        {
                            pb.PageBackgroundImage = await ((App)Application.Current).imageHelper.ConvertImageFiletoByteArrayAsync(sf);

                        }
                        else
                        {
                            pb.PageBackgroundImage = new byte[1];
                        }
                        pageBackgrounds.Add(pb);
                    }
                    foreach (StorageFile itemed in newimagesnotsaved)
                    {
                        PageBackground pb = new PageBackground();
                        pb.PageBackgroundName = itemed.DisplayName;
                        pb.PageBackgroundImage = await ((App)Application.Current).imageHelper.ConvertImageFiletoByteArrayAsync(itemed);
                        pageBackgrounds.Add(pb);
                    }
                    return pageBackgrounds;
                }
            }
            else
            {
                if (filesinfolder.Count() > 0)
                {
                    foreach (StorageFile itemed in filesinfolder)
                    {
                        PageBackground pb = new PageBackground();
                        pb.PageBackgroundName = itemed.DisplayName;
                        pb.PageBackgroundImage = await ((App)Application.Current).imageHelper.ConvertImageFiletoByteArrayAsync(itemed);
                        pageBackgrounds.Add(pb);
                    }
                    return pageBackgrounds;
                }
            }

            return null;


        }
        public async Task<GlobalAppSettings> ConvertOldGlobalAppSettings()
        {
            if (await ((App)Application.Current).globalVariables.IsFilePresent("GlobalAppSettings.txt"))
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("GlobalAppSettings.txt");
                string apps = await FileIO.ReadTextAsync(item);
                OldGlobalAppSettings oldGlobalAppSettings = JsonConvert.DeserializeObject<OldGlobalAppSettings>(apps);
                GlobalAppSettings gas = new GlobalAppSettings();
                gas.AnalyticsReporting = oldGlobalAppSettings.disableAnalytics;
                gas.CrashReporting = oldGlobalAppSettings.disableCrashReporting;
                Color c = oldGlobalAppSettings.appForgroundColor.ToColor();
                int op = int.Parse(oldGlobalAppSettings.appForegroundOpacity);
                gas.ForgroundColor = Color.FromArgb(Convert.ToByte(op), c.R, c.G, c.B);
                c = oldGlobalAppSettings.appBackgroundColor.ToColor();
                op = int.Parse(oldGlobalAppSettings.appBackgroundOpacity);
                gas.BackgroundColor = Color.FromArgb(Convert.ToByte(op), c.R, c.G, c.B);
                return gas;

            }
            return new GlobalAppSettings();

        }
        public async Task<List<AppTile>> ConvertOldAppTiles()
        {
            List<AppTile> newapps = new List<AppTile>();
            if (await ((App)Application.Current).globalVariables.IsFilePresent("collection.txt"))
            {
                AppTile app = new AppTile();
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                string apps = await FileIO.ReadTextAsync(item);
                List<OldAppTile> oldapps = JsonConvert.DeserializeObject<List<OldAppTile>>(apps);
                int location = 0;
                foreach (OldAppTile items in oldapps)
                {

                    app = await ((App)Application.Current).packageHelper.GetApp(items.appTileFullName);
                    app.ForegroundColor = items.appTileLogoColor.ToColor();
                    app.ForegroundOpacity = items.appTileLogoOpacity;
                    app.BackgroundColor = items.appTileBackgroundColor.ToColor();
                    app.BackgroundOpacity = items.appTileBackgroundOpacity;
                    app.TextColor = items.appTileTextColor.ToColor();
                    app.TextOpacity = items.appTileTextOpacity;
                    newapps.Add(app);
                    location++;

                }
                return newapps;
            }
            return newapps;
        }
    }
}
