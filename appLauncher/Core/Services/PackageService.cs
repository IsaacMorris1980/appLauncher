using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace appLauncher.Core.Services
{
    public class PackageService : IPackageService
    {
        private readonly PackageManager _pkgMgr = new PackageManager();
        private readonly IImageService _imageService;
        private List<IApporFolder> _layoutItems = new List<IApporFolder>();
        public IReadOnlyList<IApporFolder> Items => _layoutItems;
        private List<FinalTiles> _allInstalledAppsCache;
       

        // Constructor injection of IImageService and collection[cite: 2]
        public PackageService(IImageService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
           
        }

        public async Task LoadCollectionAsync()
        {
            // 1. Lightweight scan of installed packages (No logo decoding upfront)[cite: 2]
            _allInstalledAppsCache = await GetInstalledPackagesLightweightAsync();

            // 2. Load saved layout structure from applayout.json using layout reconciliation[cite: 2]
            LayoutLoadResult layoutResult = await LoadSavedLayoutAsync(_allInstalledAppsCache);

            List<IApporFolder> finalAppList = new List<IApporFolder>();

            if (layoutResult != null && layoutResult.Items != null && layoutResult.Items.Count > 0)
            {
                finalAppList.AddRange(layoutResult.Items);

                // Auto-discover newly installed apps not yet present in the saved layout[cite: 2]
                var newApps = _allInstalledAppsCache.Where(app => !layoutResult.ActiveFullNames.Contains(app.FullName)).ToList();
                if (newApps.Count > 0)
                {
                    finalAppList.AddRange(newApps);
                }
            }
            else
            {
                // Fallback: No saved layout exists, use all installed apps[cite: 2]
                finalAppList.AddRange(_allInstalledAppsCache);
            }

            _layoutItems = finalAppList.OrderBy(x => x.Name).ToList();

            // 3. Desktop optimization: Pre-load all layout logos upfront if on desktop.[cite: 2]
            if (!IsMobileDevice())
            {
                await LoadLogosForItemsAsync(finalAppList);
            }
        }

        public async Task SaveCollectionAsync()
        {
            try
            {
                if (Items == null) return;

                var itemsToSave = _layoutItems;
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                string json = JsonConvert.SerializeObject(itemsToSave, Formatting.Indented, settings);

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("applayout.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
                // Handle serialization/storage exception[cite: 2]
            }
        }

        public async Task<bool> LaunchApp(string fullname)
        {
            try
            {
                Package pm = _pkgMgr.FindPackageForUser("", fullname);
                IReadOnlyList<AppListEntry> listEntry = await pm.GetAppListEntriesAsync();
                return await listEntry[0].LaunchAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task LoadLogosForItemsAsync(IEnumerable<IApporFolder> items)
        {
            if (items == null) return;

            // Adaptive concurrency budget: 4 slots for Mobile, 16 slots for Desktop[cite: 2]
            int maxConcurrency = IsMobileDevice() ? 4 : 16;
            using (var semaphore = new SemaphoreSlim(maxConcurrency))
            {
                var tasks = new List<Task>();

                foreach (var item in items)
                {
                    if (item is FinalTiles tile)
                    {
                        tasks.Add(LoadSingleLogoViaServiceAsync(tile, semaphore));
                    }
                    else if (item is AppFolder folder)
                    {
                        foreach (var folderApp in folder.FolderApps)
                        {
                            tasks.Add(LoadSingleLogoViaServiceAsync(folderApp, semaphore));
                        }
                    }
                }

                await Task.WhenAll(tasks);
            }
        }

        private async Task<List<FinalTiles>> GetInstalledPackagesLightweightAsync()
        {
            List<Package> packages = _pkgMgr.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();
            List<FinalTiles> listApps = new List<FinalTiles>();
            int loc = 0;

            foreach (Package item in packages)
            {
                IReadOnlyList<AppListEntry> appsEntry = await item.GetAppListEntriesAsync();
                if (appsEntry.Count > 0)
                {
                    listApps.Add(new FinalTiles()
                    {
                        Pack = item,
                        Entry = appsEntry[0],
                        ListPos = loc++,
                    });
                }
            }
            return listApps;
        }

        private async Task<LayoutLoadResult> LoadSavedLayoutAsync(List<FinalTiles> allInstalledApps)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("applayout.json");
            if (!(item is StorageFile file)) return null;

            string json = await FileIO.ReadTextAsync(file);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var savedItems = JsonConvert.DeserializeObject<List<IApporFolder>>(json, settings);

            if (savedItems == null) return null;

            var reconciledList = new List<IApporFolder>();
            var activeFullNames = new HashSet<string>();

            foreach (var savedItem in savedItems)
            {
                if (savedItem is FinalTiles savedTile)
                {
                    var match = allInstalledApps.Find(x => x.FullName == savedTile.FullName);
                    if (match != null)
                    {
                        ApplyTileProperties(match, savedTile);
                        reconciledList.Add(match);
                        activeFullNames.Add(match.FullName);
                    }
                }
                else if (savedItem is AppFolder savedFolder)
                {
                    foreach (var folderApp in savedFolder.FolderApps)
                    {
                        var match = allInstalledApps.FirstOrDefault(x => x.FullName == folderApp.FullName);
                        if (match != null)
                        {
                            ApplyTileProperties(match, folderApp);
                            activeFullNames.Add(match.FullName);
                        }
                    }
                    reconciledList.Add(savedFolder);
                }
            }

            return new LayoutLoadResult
            {
                Items = reconciledList,
                ActiveFullNames = activeFullNames
            };
        }

        private async Task LoadSingleLogoViaServiceAsync(FinalTiles tile, SemaphoreSlim semaphore)
        {
            try
            {
                if (tile.Entry != null)
                {
                    var availableLogos = GetAvailableLogoSizes(tile.Entry);
                    var selection = SelectOptimalLogo(availableLogos, targetSize: 50);

                    if (selection?.StreamRef != null)
                    {
                        tile.AppLogo = await _imageService.LoadLogoAsync(
                            selection.StreamRef,
                            tile.FullName,
                            selection.TargetDecodeWidth,
                            semaphore);
                    }
                }
            }
            catch
            {
                // Catch decoding errors gracefully per tile[cite: 2]
            }
        }

        private List<LogoSizeInfo> GetAvailableLogoSizes(AppListEntry entry)
        {
            var availableLogos = new List<LogoSizeInfo>();
            if (entry?.DisplayInfo == null) return availableLogos;

            var candidateSizes = new[]
            {
                new Size(256, 256),
                new Size(192, 192),
                new Size(128, 128),
                new Size(96, 96),
                new Size(64, 64),
                new Size(50, 50),
                new Size(48, 48),
                new Size(32, 32),
                new Size(24, 24),
                new Size(16, 16)
            };

            foreach (var size in candidateSizes)
            {
                try
                {
                    var streamRef = entry.DisplayInfo.GetLogo(size);
                    if (streamRef != null)
                    {
                        availableLogos.Add(new LogoSizeInfo
                        {
                            RequestedSize = size,
                            StreamRef = streamRef
                        });
                    }
                }
                catch
                {
                    // Size not declared in manifest; skip.[cite: 2]
                }
            }

            return availableLogos;
        }

        private LogoSelectionResult SelectOptimalLogo(List<LogoSizeInfo> availableLogos, double targetSize = 50)
        {
            if (availableLogos == null || availableLogos.Count == 0)
                return null;

            var bestMatch = availableLogos
                .OrderBy(x => Math.Abs(x.RequestedSize.Width - targetSize))
                .ThenByDescending(x => x.RequestedSize.Width)
                .First();

            int? decodeWidth = (bestMatch.RequestedSize.Width >= targetSize) ? (int)targetSize : (int?)null;

            return new LogoSelectionResult
            {
                StreamRef = bestMatch.StreamRef,
                TargetDecodeWidth = decodeWidth
            };
        }

        private bool IsMobileDevice()
        {
            return AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyTileProperties(FinalTiles target, FinalTiles source)
        {
            target.BackColor = source.BackColor;
            target.LogoColor = source.LogoColor;
            target.TextColor = source.TextColor;
            target.ListPos = source.ListPos;
            target.FolderListPos = source.FolderListPos;
            target.Favorite = source.Favorite;
        }

        public async Task<string> InstallCertificateAsync()
        {
            try
            {
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.List
                };
                picker.FileTypeFilter.Add(".cer");

                var file = await picker.PickSingleFileAsync();
                if (file == null) return "Selection cancelled";

                var cert = new X509Certificate2(file.Path);
                using (var store = new X509Store(StoreName.TrustedPublisher, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.MaxAllowed);
                    store.Add(cert);
                }
                return "Certificate Installed";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> LoadDependenciesAsync()
        {
            try
            {
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.List
                };
                picker.FileTypeFilter.Add(".appx");
                picker.FileTypeFilter.Add(".appxbundle");

                var files = await picker.PickMultipleFilesAsync();
                if (files == null || files.Count == 0) return string.Empty;

                foreach (var file in files)
                {
                    await _pkgMgr.AddPackageAsync(new Uri(file.Path), null, DeploymentOptions.None);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Installs a primary application package, reads its manifest to identify it, and adds it to collections.
        /// </summary>
        public async Task<string> InstallApplicationAsync()
        {
            try
            {
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.List
                };
                picker.FileTypeFilter.Add(".appx");
                picker.FileTypeFilter.Add(".appxbundle");

                var file = await picker.PickSingleFileAsync();
                if (file == null) return "Selection cancelled";

                // 1. Extract the true package identity name from AppxManifest.xml inside the installer
                string targetPackageName = await GetPackageNameFromInstallerAsync(file);

                // 2. Install the package asynchronously[cite: 2]
                var deploymentOperation = _pkgMgr.AddPackageAsync(new Uri(file.Path), null, DeploymentOptions.None);
                var result = await deploymentOperation;

                if (deploymentOperation.Status != AsyncStatus.Completed)
                {
                    return $"Installation failed: {result.ErrorText}";
                }

                // 3. Locate the freshly installed package and add it to the active collection
                var installedPackages = _pkgMgr.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
                foreach (var package in installedPackages)
                {
                    if (!string.IsNullOrEmpty(targetPackageName) &&
                        package.Id.Name.Equals(targetPackageName, StringComparison.OrdinalIgnoreCase))
                    {
                        IReadOnlyList<AppListEntry> appsEntry = await package.GetAppListEntriesAsync();
                        if (appsEntry.Count > 0)
                        {
                            var newTile = new FinalTiles()
                            {
                                Pack = package,
                                Entry = appsEntry[0],
                                ListPos = 0
                            };

                            // Load logo for the newly installed tile
                            if (newTile.Entry != null)
                            {
                                var availableLogos = GetAvailableLogoSizes(newTile.Entry);
                                var selection = SelectOptimalLogo(availableLogos, targetSize: 50);
                                if (selection?.StreamRef != null)
                                {
                                    using (var semaphore = new SemaphoreSlim(1))
                                    {
                                        newTile.AppLogo = await _imageService.LoadLogoAsync(
                                            selection.StreamRef,
                                            newTile.FullName,
                                            selection.TargetDecodeWidth,
                                            semaphore);
                                    }
                                }
                            }

                            if (_layoutItems != null)
                            {
                                _layoutItems.Add(newTile);
                            }
                            if (Items != null && Items != _layoutItems)
                            {
                                _layoutItems.Add(newTile);
                            }
                            break;
                        }
                    }
                }

                // 4. Save updated collection state
                await SaveCollectionAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Failed to install: {ex.Message}";
            }
        }

        public async Task<string> RemoveApplicationAsync(string fullname)
        {
            try
            {
                var deploymentOperation = _pkgMgr.RemovePackageAsync(fullname, RemovalOptions.None);
                var deploymentResult = await deploymentOperation;

                if (deploymentOperation.Status == AsyncStatus.Error)
                {
                    Debug.WriteLine($"Error code: {deploymentOperation.ErrorCode} Error text: {deploymentResult.ErrorText}");
                    return $"Error code {deploymentOperation.ErrorCode} Error text: {deploymentResult.ErrorText}";
                }

                if (deploymentOperation.Status == AsyncStatus.Canceled)
                {
                    Debug.WriteLine("Removal canceled");
                    return "Removal Canceled";
                }

                if (deploymentOperation.Status == AsyncStatus.Completed)
                {
                    Debug.WriteLine("Removal succeeded");

                    // Synchronize collections[cite: 2]
                    if (_layoutItems != null)
                    {
                        _layoutItems.Remove(newTile);
                    }
                    if (Items != null && Items != _layoutItems)
                    {
                        _layoutItems.RemoveApp(fullname);
                    }

                    await SaveCollectionAsync();

                    return "Removal Succeeded";
                }

                return "Removal status unknown";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Removal exception: {ex.Message}");
                return $"Failed to remove: {ex.Message}";
            }
        }

        /// <summary>
        /// Helper to inspect the embedded AppxManifest.xml inside an installer package.
        /// </summary>
        private async Task<string> GetPackageNameFromInstallerAsync(StorageFile file)
        {
            try
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        var manifestEntry = archive.GetEntry("AppxManifest.xml");
                        if (manifestEntry != null)
                        {
                            using (var manifestStream = manifestEntry.Open())
                            {
                                var xDoc = XDocument.Load(manifestStream);
                                XNamespace ns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
                                var identity = xDoc.Descendants(ns + "Identity").FirstOrDefault();
                                return identity?.Attribute("Name")?.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading manifest: {ex.Message}");
            }

            return string.Empty;
        }
        public void UpdateFolderContent(AppFolder folder, IEnumerable<FinalTiles> updatedApps)
        {
            folder.FolderApps.Clear();
            foreach (var app in updatedApps)
            {
                folder.FolderApps.Add(app);
            }
        }
        public void UpdateApp(FinalTiles app)
        {
            var existingApp = _appsCollection?.FirstOrDefault(a => a.FullName == app.FullName);
            if (existingApp != null)
            {
                existingApp.BackColor = app.BackColor;
                existingApp.LogoColor = app.LogoColor;
                existingApp.TextColor = app.TextColor;
                existingApp.ListPos = app.ListPos;
                existingApp.FolderListPos = app.FolderListPos;
                existingApp.Favorite = app.Favorite;
            }
        }
    } 
}


