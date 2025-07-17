using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the AppLoading page, responsible for initializing application data
    /// and navigating to the main content once loading is complete.
    /// </summary>
    public class AppLoadingViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IPackageService _packageService;
        private readonly IImageService _imageService;
        private readonly INavigationService _navigationService;
        private readonly ILoggingService _loggingService;

        private bool _isLoading = true;
        /// <summary>
        /// Gets or sets a value indicating whether the application is currently loading data.
        /// Bound to the ProgressRing's IsActive property.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _loadingMessage = "Loading application data...";
        /// <summary>
        /// Gets or sets the message displayed to the user during the loading process.
        /// </summary>
        public string LoadingMessage
        {
            get => _loadingMessage;
            set => SetProperty(ref _loadingMessage, value);
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;

        // Command to initiate the loading process
        public ViewModelBase.AsyncCommand InitializeLoadingCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLoadingViewModel"/> class.
        /// </summary>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="packageService">The service for managing application packages.</param>
        /// <param name="imageService">The service for managing background images.</param>
        /// <param name="navigationService">The service for handling page navigation.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public AppLoadingViewModel(ISettingsService settingsService, IPackageService packageService, IImageService imageService, INavigationService navigationService, ILoggingService loggingService)
        {
            _settingsService = settingsService;
            _packageService = packageService;
            _imageService = imageService;
            _navigationService = navigationService;
            _loggingService = loggingService;

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
            InitializeLoadingCommand = new ViewModelBase.AsyncCommand(ExecuteInitializeLoadingAsync);
        }

        /// <summary>
        /// Executes the full application loading sequence.
        /// </summary>
        private async Task ExecuteInitializeLoadingAsync()
        {
            IsLoading = true;
            LoadingMessage = "Loading application settings...";
            try
            {
                // Load settings first, as other services might depend on them (e.g., ImageService needs ImageRotationTime)
                await _settingsService.LoadAppSettingsAsync();

                LoadingMessage = "Scanning installed applications...";
                await _packageService.LoadAppCollectionAsync();

                LoadingMessage = "Loading background images...";
                await _imageService.LoadBackgroundImages();

                LoadingMessage = "Ready!";
                await Task.Delay(500); // Small delay for message to be seen

                // Navigate to the main page
                _navigationService.Navigate(typeof(Pages.MainPage));
                // Note: Removing from BackStack should ideally be handled by the NavigationService
                // or the Frame itself if it's the root navigation.
                // If you need to explicitly clear the backstack, it would be a NavigationService method.
                // Example: _navigationService.NavigateAndClearBackStack(typeof(Pages.MainPage));
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                LoadingMessage = $"Error loading: {ex.Message}. Please restart the app.";
                IsLoading = false; // Stop loading indicator on error
            }
        }
    }
}
