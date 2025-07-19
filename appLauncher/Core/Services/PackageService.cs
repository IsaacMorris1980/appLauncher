using appLauncher.Core.Extensions;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services; // Add this using for ILoggingService and IFileUtilityService

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage;

namespace appLauncher.Core.Services
{
    /// <summary>
    /// Provides services for managing installed application packages (apps/games).
    /// Handles loading, saving, launching, rescanning, and removing applications.
    /// </summary>
    public class PackageService : IPackageService
    {
        private readonly ILoggingService _loggingService; // Dependency for logging
        private readonly IFileService _fileUtilityService; // Dependency for file operations

        public List<IApporFolder> Search { get; private set; }
        public PaginationObservableCollection Apps { get; private set; }

        /// <summary>
        /// Event fired when the application collection has been retrieved and updated.
        /// </summary>
        public event EventHandler AppsRetrieved;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageService"/> class.
        /// </summary>
        /// <param name="loggingService">The logging service for error handling.</param>
        /// <param name="fileUtilityService">The file utility service for file presence checks and I/O.</param>
        public PackageService(ILoggingService loggingService, IFileService fileUtilityService)
        {
            _loggingService = loggingService;
            _fileUtilityService = fileUtilityService;
        }

        /// <summary>
        /// Loads the application collection from local storage, merging with currently installed apps.
        /// </summary>
        public async Task LoadAppCollectionAsync()
        {
            PackageManager pm = new PackageManager();
            List<FinalTiles> allInstalledApps = await GetInstalledApps(); // Get all currently installed apps
            List<IApporFolder> loadedAppsAndFolders = new List<IApporFolder>();
            List<FinalTiles> savedTiles = new List<FinalTiles>();
            List<AppFolder> savedFolders = new List<AppFolder>();
            bool filesExist = false;

            // Try to read saved app data file
            string appsJson = await _fileUtilityService.ReadTextFromFileAsync("allapps.json");
            if (!string.IsNullOrEmpty(appsJson))
            {
                try
                {
                    savedTiles = JsonConvert.DeserializeObject<List<FinalTiles>>(appsJson);
                    filesExist = true;
                }
                catch (Exception ex)
                {
                    await _loggingService.LogExceptionAsync(ex);
                    // Continue without saved tiles if deserialization fails
                }
            }
            // Try to read saved folder data file
            string foldersJson = await _fileUtilityService.ReadTextFromFileAsync("folders.json");
            if (!string.IsNullOrEmpty(foldersJson))
            {
                try
                {
                    savedFolders = JsonConvert.DeserializeObject<List<AppFolder>>(foldersJson);
                    filesExist = true;
                }
                catch (Exception ex)
                {
                    await _loggingService.LogExceptionAsync(ex);
                    // Continue without saved folders if deserialization fails
                }
            }
            if (filesExist)
            {
                // Process saved individual app tiles
                if (savedTiles != null && savedTiles.Count > 0)
                {
                    foreach (FinalTiles savedTile in savedTiles)
                    {
                        try
                        {
                            // Find the corresponding currently installed app
                            FinalTiles installedApp = allInstalledApps.Find(x => x.FullName == savedTile.FullName);
                            if (installedApp != null)
                            {
                                // Apply saved customization settings to the installed app
                                installedApp.BackColor = savedTile.BackColor;
                                installedApp.LogoColor = savedTile.LogoColor;
                                installedApp.TextColor = savedTile.TextColor;
                                installedApp.ListPos = savedTile.ListPos;
                                installedApp.FolderListPos = savedTile.FolderListPos;
                                installedApp.Favorite = savedTile.Favorite;
                                await installedApp.SetLogo(); // Ensure logo is set for the updated app
                                loadedAppsAndFolders.Add(installedApp);
                            }
                            // If installedApp is null, it means the app was uninstalled, so we don't add it.
                        }
                        catch (Exception ex)
                        {
                            await _loggingService.LogExceptionAsync(ex);
                        }
                    }
                }

                // Process saved folders
                if (savedFolders != null && savedFolders.Count > 0)
                {
                    foreach (var savedFolder in savedFolders)
                    {
                        // Create a new folder instance to avoid modifying the original savedFolder directly
                        AppFolder currentFolder = new AppFolder
                        {
                            Name = savedFolder.Name,
                            BackColor = savedFolder.BackColor,                           
                            TextColor = savedFolder.TextColor,
                            ListPos = savedFolder.ListPos,
                            Favorite = savedFolder.Favorite,
                            FolderApps = new List<IApporFolder>()
                        };
                        

                        foreach (var folderApp in savedFolder.FolderApps.ToList()) // Iterate on a copy to allow modification
                        {
                            FinalTiles installedAppInFolder = allInstalledApps.FirstOrDefault(x => x.FullName == folderApp.FullName);
                            if (installedAppInFolder != null)
                            {
                                // Apply saved customization settings to the installed app within the folder
                                installedAppInFolder.BackColor = folderApp.BackColor;
                                installedAppInFolder.LogoColor = folderApp.LogoColor;
                                installedAppInFolder.TextColor = folderApp.TextColor;
                                installedAppInFolder.ListPos = folderApp.ListPos;
                                installedAppInFolder.FolderListPos = folderApp.FolderListPos;
                                installedAppInFolder.Favorite = folderApp.Favorite;
                                await installedAppInFolder.SetLogo();
                                currentFolder.FolderApps.Add(installedAppInFolder);
                            }
                            // If installedAppInFolder is null, it means the app was uninstalled, so we don't add it to the folder.
                        }
                        // Only add folder if it still contains apps after filtering uninstalled ones
                        if (currentFolder.FolderApps.Any())
                        {
                            loadedAppsAndFolders.Add(currentFolder);
                        }
                    }
                }
            }
            else
            {
                // If no saved files, initialize with all currently installed apps
                foreach (var item in allInstalledApps)
                {
                    await item.SetLogo();
                }
                loadedAppsAndFolders.AddRange(allInstalledApps);
            }
            // Initialize Apps and Search collections
            Apps = new PaginationObservableCollection(loadedAppsAndFolders.OrderBy(x => x.Name).ToList());
            Search = loadedAppsAndFolders.OrderBy(x => x.Name).ToList();
            Apps.SetCurrentPage(0); // Ensure pagination is calculated

            AppsRetrieved?.Invoke(this, EventArgs.Empty); // Notify subscribers that apps are loaded
        }

        /// <summary>
        /// Retrieves a list of all currently installed main applications for the current user.
        /// </summary>
        /// <returns>A list of <see cref="FinalTiles"/> representing the installed applications.</returns>
        private async Task<List<FinalTiles>> GetInstalledApps()
        {
            PackageManager pm = new PackageManager();
            List<Package> packages = pm.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();
            List<FinalTiles> listApps = new List<FinalTiles>();
            int loc = 0; // Initial position for new apps

            foreach (Package item in packages)
            {
                try
                {
                    IReadOnlyList<AppListEntry> appsEntry = await item.GetAppListEntriesAsync();
                    if (appsEntry.Count > 0)
                    {
                        // Use the first AppListEntry for the package
                        FinalTiles finalTile = new FinalTiles()
                        {
                            Pack = item,
                            Entry = appsEntry[0],
                            ListPos = loc,
                        };
                        await finalTile.SetLogo(); // Set the logo for the app tile
                        listApps.Add(finalTile);
                        loc += 1; // Increment position for the next app
                    }
                }
                catch (Exception es)
                {
                    // Log any exceptions encountered during package or app entry retrieval
                    await _loggingService.LogExceptionAsync(es);
                }
            }
            return listApps;
        }
        /// <summary>
        /// Saves the current application and folder collection to local storage.
        /// </summary>
        public async Task SaveAppCollectionAsync()
        {
            try
            {
                // Save individual app tiles
                List<FinalTiles> saveApps = Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
                if (saveApps.Any()) // Only save if there are apps to save
                {
                    string saveappsstring = JsonConvert.SerializeObject(saveApps, Formatting.Indented);
                    await _fileUtilityService.WriteTextToFileAsync("allapps.json", saveappsstring);
                }
                else // If no apps, ensure the file is removed or empty
                {
                    await _fileUtilityService.DeleteFileAsync("allapps.json");
                }

                // Save app folders
                List<AppFolder> saveFolders = Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
                if (saveFolders.Any()) // Only save if there are folders to save
                {
                    string savefolderstring = JsonConvert.SerializeObject(saveFolders, Formatting.Indented);
                    await _fileUtilityService.WriteTextToFileAsync("folders.json", savefolderstring);
                }
                else // If no folders, ensure the file is removed or empty
                {
                    await _fileUtilityService.DeleteFileAsync("folders.json");
                }
            }
            catch (Exception es)
            {
                await _loggingService.LogExceptionAsync(es);
            }
        }

        /// <summary>
        /// Launches an application given its full package name.
        /// </summary>
        /// <param name="fullname">The full package name of the application to launch.</param>
        /// <returns>True if the application was launched successfully; otherwise, false.</returns>
        public async Task<bool> LaunchApplication(string fullname)
        {
            try
            {
                PackageManager pm = new PackageManager();
                Package package = pm.FindPackagesForUser("", fullname).FirstOrDefault();
                if (package != null)
                {
                    IReadOnlyList<AppListEntry> listEntry = await package.GetAppListEntriesAsync();
                    if (listEntry.Count > 0)
                    {
                        return await listEntry[0].LaunchAsync();
                    }
                }
            }
            catch (Exception es)
            {
                await _loggingService.LogExceptionAsync(es);
            }
            return false;
        }

        /// <summary>
        /// Rescans for newly installed or uninstalled applications and updates the collection.
        /// </summary>
        public async Task RescanForNewApplications()
        {
            List<FinalTiles> newScanApps = await GetInstalledApps(); // Get current state of installed apps
            List<IApporFolder> currentCollection = Apps.GetOriginalCollection().ToList(); // Get a mutable copy of the existing collection

            // Identify newly installed apps (present in newScanApps but not in currentCollection as FinalTiles)
            var addedApps = newScanApps.Where(x => !currentCollection.OfType<FinalTiles>().Any(y => y.FullName == x.FullName)).ToList();

            // Identify uninstalled apps (present in currentCollection as FinalTiles but not in newScanApps)
            var removedApps = currentCollection.OfType<FinalTiles>().Where(x => !newScanApps.Any(y => y.FullName == x.FullName)).ToList();

            // Add new apps to the collection
            int loc = currentCollection.Count;
            foreach (var item in addedApps)
            {
                item.ListPos = loc + 1; // Assign a new position
                currentCollection.Add(item);
                loc += 1;
            }

            // Remove uninstalled apps from the main collection and any folders
            foreach (var removedApp in removedApps)
            {
                // Remove from the main flat list
                currentCollection.Remove(removedApp);

                // Remove from any folders that might contain this app
                foreach (var folder in currentCollection.OfType<AppFolder>())
                {
                    folder.FolderApps.Remove(x => x.FullName == removedApp.FullName);
                }
            }

            // Re-initialize Apps and Search with the updated and sorted collection
            Apps = new PaginationObservableCollection(currentCollection.OrderBy(x => x.Name));
            Apps.SetCurrentPage(0);
            Search = new List<IApporFolder>(currentCollection.OrderBy(x => x.Name)); // Update search list as well
        }

        /// <summary>
        /// Removes an application from the search collection (and implicitly from folders if present).
        /// This is typically called after an app is uninstalled.
        /// </summary>
        /// <param name="fullName">The full package name of the application to remove.</param>
        public void RemoveFromSearch(string fullName)
        {
            if (Search == null) return;

            // Filter out the removed application from the main search list
            var newSearchList = Search.Where(item =>
            {
                if (item is FinalTiles app)
                {
                    return app.FullName != fullName; // Keep if not the app to be removed
                }
                else if (item is AppFolder folder)
                {
                    // If it's a folder, remove the app from its internal list
                    folder.FolderApps.Remove(x => x.FullName == fullName);
                    return folder.FolderApps.Any(); // Keep the folder only if it still contains apps
                }
                return true; // Keep other types of items
            }).ToList();

            Search = newSearchList; // Update the Search property
        }
    }
}