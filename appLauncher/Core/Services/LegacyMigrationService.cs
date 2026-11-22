using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace appLauncher.Core.Services
{
    public class LegacyMigrationService : ILegacyMigrationService
    {
        private readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private readonly ISettingsService _settingsService;

        public LegacyMigrationService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<bool> HasLegacyFilesAsync()
        {
            return await HasLegacySettingsAsync() ||
                   await HasLegacyAppAndFoldersAsync() ||
                   await HasLegacyBackgroundImagesAsync();
        }

        public async Task<bool> HasLegacySettingsAsync()
        {
            return await _localFolder.TryGetItemAsync("globalappsettings.json") != null;
        }

        public async Task<bool> HasLegacyAppAndFoldersAsync()
        {
            return await _localFolder.TryGetItemAsync("allapps.json") != null ||
                   await _localFolder.TryGetItemAsync("legacyappfolders.json") != null ||
                   await _localFolder.TryGetItemAsync("legacyfinaltiles.json") != null ||
                   await _localFolder.TryGetItemAsync("folders.json") != null;
        }

        public async Task<bool> HasLegacyBackgroundImagesAsync()
        {
            return await _localFolder.TryGetItemAsync("images.json") != null;
        }

        public async Task MigrateAllLegacyDataAsync()
        {
            await MigrateSettingsAsync();
            await MigrateAppAndFoldersAsync();
            await MigrateBackgroundImagesAsync();
        }

        public async Task MigrateSettingsAsync()
        {
            if (_settingsService.TotalAppSettings.SettingsMigrated) return;

            var oldItem = await _localFolder.TryGetItemAsync("globalappsettings.json");
            if (oldItem != null && oldItem is StorageFile oldFile)
            {
                try
                {
                    string json = await FileIO.ReadTextAsync(oldFile);
                    var legacy = JsonConvert.DeserializeObject<LegacyGlobalAppSettings>(json);

                    if (legacy != null)
                    {
                        _settingsService.TotalAppSettings.Backgrounds.ImageRotationIntervalMinutes =
                            (int)legacy._imageRotationTime.TotalMinutes > 0 ? (int)legacy._imageRotationTime.TotalMinutes : 15;
                        _settingsService.TotalAppSettings.Network.IsOfflineMode = !legacy._isConnected;
                        _settingsService.TotalAppSettings.Network.WifiOnly = legacy._wifiOnly;
                    }
                }
                catch
                {
                    // Suppress or log parsing exceptions
                }
                finally
                {
                    await SafeDeleteAsync(oldFile);
                }
            }

            _settingsService.TotalAppSettings.SettingsMigrated = true;
            await _settingsService.SaveAppSettingsAsync();
        }

        public async Task MigrateAppAndFoldersAsync()
        {
            if (_settingsService.TotalAppSettings.AppsMigrated && _settingsService.TotalAppSettings.FoldersMigrated) return;

            var appAndFoldersData = new AppAndFoldersData();

            var appsItem = await _localFolder.TryGetItemAsync("allapps.json") ??
                           await _localFolder.TryGetItemAsync("legacyappfolders.json") ??
                           await _localFolder.TryGetItemAsync("legacyfinaltiles.json");

            if (appsItem != null && appsItem is StorageFile appsFile)
            {
                try
                {
                    string json = await FileIO.ReadTextAsync(appsFile);
                    var oldTiles = JsonConvert.DeserializeObject<List<LegacyFinalTiles>>(json) ?? new List<LegacyFinalTiles>();

                    var migratedTiles = new List<FinalTiles>();
                    foreach (var old in oldTiles)
                    {
                        migratedTiles.Add(new FinalTiles
                        {
                            FullName = old.FullName,
                            ListPos = old.ListPos,
                            FolderListPos = old.FolderListPos,
                            Favorite = old._favorite,
                            LaunchedCount = old.LaunchedCount,
                            BackColor = old._backColor,
                            LogoColor = old._logoColor,
                            TextColor = old._textColor
                        });
                    }
                    appAndFoldersData.Apps = migratedTiles;
                }
                catch { }
                finally { await SafeDeleteAsync(appsFile); }
            }

            var foldersItem = await _localFolder.TryGetItemAsync("folders.json");
            if (foldersItem != null && foldersItem is StorageFile foldersFile)
            {
                try
                {
                    string json = await FileIO.ReadTextAsync(foldersFile);
                    appAndFoldersData.Folders = JsonConvert.DeserializeObject<List<FolderItemModel>>(json) ?? new List<FolderItemModel>();
                }
                catch { }
                finally { await SafeDeleteAsync(foldersFile); }
            }

            string newJson = JsonConvert.SerializeObject(appAndFoldersData, Formatting.Indented);
            StorageFile newFile = await _localFolder.CreateFileAsync("appandfolders.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(newFile, newJson);

            _settingsService.TotalAppSettings.AppsMigrated = true;
            _settingsService.TotalAppSettings.FoldersMigrated = true;
            await _settingsService.SaveAppSettingsAsync();
        }

        public async Task MigrateBackgroundImagesAsync()
        {
            if (_settingsService.TotalAppSettings.ImagesMigrated) return;

            var bgData = new BackgroundImageCollection();

            var oldItem = await _localFolder.TryGetItemAsync("images.json");
            if (oldItem != null && oldItem is StorageFile oldFile)
            {
                try
                {
                    string json = await FileIO.ReadTextAsync(oldFile);
                    bgData.Images = JsonConvert.DeserializeObject<List<PageBackgrounds>>(json) ?? new List<PageBackgrounds>();
                }
                catch { }
                finally { await SafeDeleteAsync(oldFile); }
            }

            string newJson = JsonConvert.SerializeObject(bgData, Formatting.Indented);
            StorageFile newFile = await _localFolder.CreateFileAsync("backimages.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(newFile, newJson);

            _settingsService.TotalAppSettings.ImagesMigrated = true;
            await _settingsService.SaveAppSettingsAsync();
        }

        private async Task SafeDeleteAsync(StorageFile file)
        {
            try
            {
                await file.DeleteAsync();
            }
            catch
            {
                // Suppress IO exceptions if file is locked or missing
            }
        }
    }
}
