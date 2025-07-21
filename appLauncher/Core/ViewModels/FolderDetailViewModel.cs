using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the FolderDetail page, handling display of folder information,
    /// editing folder properties, and managing apps within the folder.
    /// </summary>
    public class FolderDetailViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;
        private readonly IInstallationService _appInstallService; // For launching apps
        private readonly INavigationService _navigationService; // Injected Navigation Service

        private AppFolder _currentFolder;
        /// <summary>
        /// Gets or sets the <see cref="AppFolder"/> object currently being viewed or edited.
        /// </summary>
        public AppFolder CurrentFolder
        {
            get => _currentFolder;
            set => SetProperty(ref _currentFolder, value);
        }

        private bool _isEditMode;
        /// <summary>
        /// Gets or sets a value indicating whether the page is in edit mode.
        /// Controls visibility of edit controls in the UI.
        /// </summary>
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (SetProperty(ref _isEditMode, value))
                {
                    // Re-evaluate save command can execute state
                    SaveChangesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _pageTitle;
        /// <summary>
        /// Gets or sets the title of the page (e.g., "Folder Name" or "Edit Folder Name").
        /// </summary>
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        private ObservableCollection<IApporFolder> _allAvailableApps;
        /// <summary>
        /// Gets the collection of all apps available to be added to the folder (i.e., not already in this folder).
        /// </summary>
        public ObservableCollection<IApporFolder> AllAvailableApps
        {
            get => _allAvailableApps;
            set => SetProperty(ref _allAvailableApps, value);
        }

        private FinalTiles _selectedAvailableApp;
        /// <summary>
        /// Gets or sets the currently selected app from the list of all available apps.
        /// </summary>
        public FinalTiles SelectedAvailableApp
        {
            get => _selectedAvailableApp;
            set
            {
                if (SetProperty(ref _selectedAvailableApp, value))
                {
                    AddAppToFolderCommand.RaiseCanExecuteChanged(); // Re-evaluate can execute
                }
            }
        }

        private FinalTiles _selectedAppInFolder;
        /// <summary>
        /// Gets or sets the currently selected app from the list of apps already in the folder.
        /// </summary>
        public FinalTiles SelectedAppInFolder
        {
            get => _selectedAppInFolder;
            set
            {
                if (SetProperty(ref _selectedAppInFolder, value))
                {
                    RemoveAppFromFolderCommand.RaiseCanExecuteChanged(); // Re-evaluate can execute
                }
            }
        }

        private ObservableCollection<IApporFolder> _searchResults;
        /// <summary>
        /// Gets the collection of search results for apps within the current folder.
        /// </summary>
        public ObservableCollection<IApporFolder> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;


        // Commands
        public ViewModelBase.Command ToggleEditModeCommand { get; private set; }
        public ViewModelBase.Command AddAppToFolderCommand { get; private set; }
        public ViewModelBase.Command RemoveAppFromFolderCommand { get; private set; }
        public ViewModelBase.AsyncCommand SaveChangesCommand { get; private set; }
        public ViewModelBase.Command<string> SearchAppsCommand { get; private set; }
        public ViewModelBase.AsyncCommand<IApporFolder> LaunchAppCommand { get; private set; }
        public ViewModelBase.Command<IApporFolder> ShowAppInfoCommand { get; private set; }
        public ViewModelBase.Command<IApporFolder> EditAppTileCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FolderDetailViewModel"/> class.
        /// </summary>
        /// <param name="packageService">The service for managing application packages.</param>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        /// <param name="appInstallService">The service for launching applications.</param>
        /// <param name="navigationService">The service for handling navigation.</param>
        public FolderDetailViewModel(IPackageService packageService, ILoggingService loggingService, ISettingsService settingsService, IInstallationService appInstallService, INavigationService navigationService)
        {
            _packageService = packageService;
            _settingsService = settingsService;
            _loggingService = loggingService;
            _appInstallService = appInstallService;
            _navigationService = navigationService;
            InitializeCommands();
            // Subscribe to settings changes to update brushes if the theme changes
            _settingsService.AppSettings.DisplaySettings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppDisplaySettings.AppForegroundColorBrush) ||
                    e.PropertyName == nameof(AppDisplaySettings.AppBackgroundColorBrush))
                {
                    OnPropertyChanged(nameof(AppForegroundColorBrush));
                    OnPropertyChanged(nameof(AppBackgroundColorBrush));
                }
            };
        }
        /// <summary>
        /// Sets the folder to be displayed/edited. This is typically called when navigating to the page.
        /// </summary>
        /// <param name="folder">The <see cref="AppFolder"/> object to display/edit.</param>
        public void SetFolder(AppFolder folder)
        {
            CurrentFolder = folder;
            if (CurrentFolder != null)
            {
                PageTitle = CurrentFolder.Name; // Initial title
                InitializeAvailableApps(); // Populate the list of apps not in this folder
                SearchResults = new ObservableCollection<IApporFolder>(CurrentFolder.FolderApps); // Initial search results
            }
        }
        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            ToggleEditModeCommand = new ViewModelBase.Command(() =>
            {
                IsEditMode = !IsEditMode;
                PageTitle = IsEditMode ? $"Edit \"{CurrentFolder?.Name}\" Folder" : CurrentFolder?.Name;
            });
            AddAppToFolderCommand = new ViewModelBase.Command(AddAppToFolder, () => SelectedAvailableApp != null);
            RemoveAppFromFolderCommand = new ViewModelBase.Command(RemoveAppFromFolder, () => SelectedAppInFolder != null);
            SaveChangesCommand = new ViewModelBase.AsyncCommand(SaveChangesAsync, () => IsEditMode && CurrentFolder != null);
            SearchAppsCommand = new ViewModelBase.Command<string>(PerformSearch);
            ShowAppInfoCommand = new ViewModelBase.Command<IApporFolder>(app =>
            {
                _navigationService.Navigate(typeof(Pages.AppDetailPage), app);
            });
            EditAppTileCommand = new ViewModelBase.Command<IApporFolder>(app =>
            {
                _navigationService.Navigate(typeof(Pages.AppDetailPage), Tuple.Create(app, true));
            });
        }
        /// <summary>
        /// Populates the <see cref="AllAvailableApps"/> collection with apps that are not currently in this folder.
        /// </summary>
        private void InitializeAvailableApps()
        {
            if (CurrentFolder == null) return;
            var allCurrentApps = _packageService.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            // Filter out apps that are already in the current folder
            var appsNotInThisFolder = allCurrentApps.OfType<FinalTiles>().Where(app => !CurrentFolder.FolderApps.OfType<FinalTiles>().Any(fa => fa.FullName == app.FullName)).ToList();
           AllAvailableApps = new ObservableCollection<IApporFolder>(appsNotInThisFolder);
                
        }
        /// <summary>
        /// Adds the currently selected available app to the folder.
        /// </summary>
        private void AddAppToFolder()
        {
            if (SelectedAvailableApp != null && CurrentFolder != null)
            {
                CurrentFolder.FolderApps.Add(SelectedAvailableApp);
                AllAvailableApps.Remove(SelectedAvailableApp);
                SelectedAvailableApp = null; // Clear selection
                RemoveAppFromFolderCommand.RaiseCanExecuteChanged(); // Update can execute for remove button
                OnPropertyChanged(nameof(CurrentFolder)); // Notify UI of FolderApps change
            }
        }
        /// <summary>
        /// Removes the currently selected app from the folder.
        /// </summary>
        private void RemoveAppFromFolder()
        {
            if (SelectedAppInFolder != null && CurrentFolder != null)
            {
                CurrentFolder.FolderApps.Remove(SelectedAppInFolder);
                AllAvailableApps.Add(SelectedAppInFolder); // Add back to available apps
                SelectedAppInFolder = null; // Clear selection
                AddAppToFolderCommand.RaiseCanExecuteChanged(); // Update can execute for add button
                OnPropertyChanged(nameof(CurrentFolder)); // Notify UI of FolderApps change
            }
        }

        /// <summary>
        /// Saves the changes made to the folder.
        /// </summary>
        private async Task SaveChangesAsync()
        {
            try
            {
                if (CurrentFolder != null)
                {
                    // The CurrentFolder object is already a reference to the one in the main collection,
                    // so changes are live. We just need to trigger a save of the entire collection.
                    await _packageService.SaveAppCollectionAsync();
                    IsEditMode = false; // Exit edit mode after saving
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Performs a search within the apps of the current folder.
        /// </summary>
        /// <param name="query">The search query.</param>
        private void PerformSearch(string query)
        {
            if (CurrentFolder == null) return;

            if (string.IsNullOrWhiteSpace(query))
            {
                SearchResults = new ObservableCollection<IApporFolder>(CurrentFolder.FolderApps);
            }
            else
            {
                // 15063 COMPATIBILITY FIX: Using ToLowerInvariant() for case-insensitive comparison
                string lowerQuery = query.ToLowerInvariant();
                var filteredApps = CurrentFolder.FolderApps.OfType<FinalTiles>()
                    .Where(app => app.Name.ToLowerInvariant().Contains(lowerQuery) ||
                                  app.Description.ToLowerInvariant().Contains(lowerQuery))
                    .Cast<IApporFolder>() // Explicitly cast to IApporFolder
                        .ToList();
                SearchResults = new ObservableCollection<IApporFolder>(filteredApps);
            }
        }

        /// <summary>
        /// Launches the specified application.
        /// </summary>
        /// <param name="app">The <see cref="FinalTiles"/> object representing the app to launch.</param>
        private async Task LaunchApp(FinalTiles app)
        {
            if (app != null)
            {
                await _appInstallService.LaunchApplicationAsync(app.FullName);
            }
        }
    }
}
