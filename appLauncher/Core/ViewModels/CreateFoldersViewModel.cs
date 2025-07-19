using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls; // For ContentDialogResult
using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the CreateFolders page, managing the creation of new app folders
    /// and the assignment of apps to them.
    /// </summary>
    public class CreateFoldersViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly ILoggingService _loggingService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService; // Assuming navigation service is injected
        private AppFolder _createdFolder;
        /// <summary>
        /// Gets or sets the <see cref="AppFolder"/> being created.
        /// </summary>
        public AppFolder CreatedFolder
        {
            get => _createdFolder;
            set => SetProperty(ref _createdFolder, value);
        }

        // Removed FolderNameInput as it's now in FolderNameViewModel
        // private string _folderNameInput;
        // public string FolderNameInput { get => _folderNameInput; set => SetProperty(ref _folderNameInput, value); }


        private bool _isFolderCreationInitiated = false;
        /// <summary>
        /// Gets or sets a value indicating whether the folder naming process has started.
        /// Used to control UI visibility before and after naming the folder.
        /// </summary>
        public bool IsFolderCreationInitiated
        {
            get => _isFolderCreationInitiated;
            set => SetProperty(ref _isFolderCreationInitiated, value);
        }

        private ObservableCollection<FinalTiles> _allAvailableApps;
        /// <summary>
        /// Gets the collection of all apps available to be added to the folder.
        /// </summary>
        public ObservableCollection<FinalTiles> AllAvailableApps
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

        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;


        public ViewModelBase.AsyncCommand InitializeCreationCommand { get; private set; }
        public ViewModelBase.Command AddAppToFolderCommand { get; private set; }
        public ViewModelBase.Command RemoveAppFromFolderCommand { get; private set; }
        public ViewModelBase.AsyncCommand SaveFolderCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFoldersViewModel"/> class.
        /// </summary>
        /// <param name="packageService">The service for managing application packages.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        /// <param name="settingsService">The service for managing application settings (for colors).</param>
        /// <param name="navigationService">The navigation service (optional, if navigating after save).</param>
        public CreateFoldersViewModel(IPackageService packageService, ILoggingService loggingService, ISettingsService settingsService, INavigationService navigationService = null)
        {
            _packageService = packageService;
            _loggingService = loggingService;
            _settingsService = settingsService;
            _navigationService = navigationService; // Assign navigation service

            CreatedFolder = new AppFolder(); // Initialize an empty new folder

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
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            InitializeCreationCommand = new ViewModelBase.AsyncCommand(InitializeCreationProcessAsync);
            AddAppToFolderCommand = new ViewModelBase.Command(AddAppToFolder, () => SelectedAvailableApp != null);
            RemoveAppFromFolderCommand = new ViewModelBase.Command(RemoveAppFromFolder, () => SelectedAppInFolder != null);
            SaveFolderCommand = new ViewModelBase.AsyncCommand(SaveFolderAsync, () => !string.IsNullOrWhiteSpace(CreatedFolder?.Name));
        }

        /// <summary>
        /// Initiates the folder creation process, including prompting for the folder name.
        /// This method is called when the page loads.
        /// </summary>
        private async Task InitializeCreationProcessAsync()
        {
            try
            {
                // Instantiate FolderNamePage and get its ViewModel for input
                var dialog = new Pages.FolderNamePage();
                var dialogViewModel = dialog.FolderName; // Get the ViewModel instance from the dialog

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary && dialogViewModel.IsFolderNameValid)
                {
                    CreatedFolder.Name = dialogViewModel.FolderNameInput; // Get name from dialog's ViewModel
                    // Set default colors for the new folder based on current app settings
                    CreatedFolder.BackColor = _settingsService.AppSettings.DisplaySettings.AppBackgroundColor;
                    CreatedFolder.TextColor = _settingsService.AppSettings.DisplaySettings.AppForgroundColor;
                    IsFolderCreationInitiated = true; // Show the rest of the UI
                    InitializeAvailableApps(); // Populate the list of apps not in any folder
                }
                else
                {
                    // User cancelled the folder creation or input was invalid.
                    // You might want to navigate back or close the page here.
                    // For now, IsFolderCreationInitiated remains false, keeping the UI hidden.
                    System.Diagnostics.Debug.WriteLine("Folder creation cancelled by user or invalid name.");
                    // If the page is opened as a modal (e.g., from a button), you might close it.
                    // If it's a primary navigation target, you might navigate back.
                    _navigationService?.GoBack(); // Example: navigate back if creation is cancelled
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                // Handle error, maybe show a message to the user
            }
        }

        /// <summary>
        /// Populates the <see cref="AllAvailableApps"/> collection with apps that are not currently in any folder.
        /// This ensures a clean list for the new folder.
        /// </summary>
        private void InitializeAvailableApps()
        {
            var allCurrentApps = _packageService.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var allCurrentFolders = _packageService.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();

            // Get apps that are not in any existing folder
            var appsNotInAnyFolder = allCurrentApps
                .Where(app => !allCurrentFolders.Any(folder => folder.FolderApps.Any(fa => fa.FullName == app.FullName)))
                .ToList();

            AllAvailableApps = new ObservableCollection<FinalTiles>(appsNotInAnyFolder);
        }

        /// <summary>
        /// Adds the currently selected available app to the folder.
        /// </summary>
        private void AddAppToFolder()
        {
            if (SelectedAvailableApp != null && CreatedFolder != null)
            {
                CreatedFolder.FolderApps.Add(SelectedAvailableApp);
                AllAvailableApps.Remove(SelectedAvailableApp);
                SelectedAvailableApp = null; // Clear selection
                RemoveAppFromFolderCommand.RaiseCanExecuteChanged(); // Update can execute for remove button
                OnPropertyChanged(nameof(CreatedFolder)); // Notify UI of FolderApps change
            }
        }

        /// <summary>
        /// Removes the currently selected app from the folder.
        /// </summary>
        private void RemoveAppFromFolder()
        {
            if (SelectedAppInFolder != null && CreatedFolder != null)
            {
                CreatedFolder.FolderApps.Remove(SelectedAppInFolder);
                AllAvailableApps.Add(SelectedAppInFolder); // Add back to available apps
                SelectedAppInFolder = null; // Clear selection
                AddAppToFolderCommand.RaiseCanExecuteChanged(); // Update can execute for add button
                OnPropertyChanged(nameof(CreatedFolder)); // Notify UI of FolderApps change
            }
        }

        /// <summary>
        /// Saves the newly created folder to the application's package collection.
        /// </summary>
        private async Task SaveFolderAsync()
        {
            try
            {
                if (CreatedFolder != null && !string.IsNullOrWhiteSpace(CreatedFolder.Name))
                {
                    // Check if a folder with this name already exists to prevent duplicates
                    // Use ToLowerInvariant() for case-insensitive comparison compatible with 15063
                    if (_packageService.Apps.GetOriginalCollection().OfType<AppFolder>().Any(f => f.Name.ToLowerInvariant() == CreatedFolder.Name.ToLowerInvariant()))
                    {
                        // Handle case where folder name already exists (e.g., show error message)
                        System.Diagnostics.Debug.WriteLine($"Folder with name '{CreatedFolder.Name}' already exists.");
                        // Optionally, you could show a ContentDialog here to inform the user.
                        return;
                    }

                    _packageService.Apps.Add(CreatedFolder); // Add the new folder
                    await _packageService.SaveAppCollectionAsync(); // Save changes to disk

                    // Optionally, navigate to the new folder's detail page or back to main page
                    System.Diagnostics.Debug.WriteLine($"Folder '{CreatedFolder.Name}' saved successfully.");
                    _navigationService?.Navigate(typeof(Pages.FolderDetailPage), CreatedFolder); // Navigate to the new folder's detail page
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
            }
        }
    }
}
