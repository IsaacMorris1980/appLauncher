using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services; // Import necessary services

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the About page, providing application information,
    /// contributor links, and background display settings.
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;

        // Properties for UI Binding
        private Brush _appBackgroundColorBrush;
        public Brush AppBackgroundColorBrush
        {
            get => _appBackgroundColorBrush;
            set => SetProperty(ref _appBackgroundColorBrush, value);
        }

        private Brush _appForegroundColorBrush;
        public Brush AppForegroundColorBrush
        {
            get => _appForegroundColorBrush;
            set => SetProperty(ref _appForegroundColorBrush, value);
        }

        private ObservableCollection<PageBackgrounds> _backgroundImages;
        public ObservableCollection<PageBackgrounds> BackgroundImages
        {
            get => _backgroundImages;
            set => SetProperty(ref _backgroundImages, value);
        }

        private string _appVersion;
        public string AppVersion
        {
            get => _appVersion;
            set => SetProperty(ref _appVersion, value);
        }

        // Hardcoded URLs for contributors and issues
        public string OriginalMaintainerName => "Colin Kiama";
        public string OriginalMaintainerUri => "https://github.com/colinkiama";

        public string CurrentMaintainerName => "Isaac Morris";
        public string CurrentMaintainerUri => "https://github.com/IsaacMorris1980";

        public string Contributor1Name => "Bruno Alexander Cremonese de Morais";
        public string Contributor1Uri => "https://github.com/brucremo";

        public string IssuesUri => "https://github.com/IsaacMorris1980/appLauncher/issues";

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="imageService">The service for managing background images.</param>
        public AboutViewModel(ISettingsService settingsService, IImageService imageService)
        {
            _settingsService = settingsService;
            _imageService = imageService;

            // Initialize properties from services
            UpdatePropertiesFromServices();

            // Subscribe to settings changes if you want the About page to update dynamically
            // while it's open (e.g., if user changes theme settings from another page).
            _settingsService.AppSettings.DisplaySettings.PropertyChanged += DisplaySettings_PropertyChanged;
            _imageService.ImagesRetrieved += ImageService_ImagesRetrieved;
        }

        /// <summary>
        /// Updates ViewModel properties based on the current state of injected services.
        /// </summary>
        private void UpdatePropertiesFromServices()
        {
            AppBackgroundColorBrush = _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;
            AppForegroundColorBrush = _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
            BackgroundImages = _imageService.BackgroundImages;
            AppVersion = _settingsService.AppSettings.VersionInfo.AppVersion;
        }
        /// <summary>
        /// Handles property changes in DisplaySettings to update ViewModel properties.
        /// </summary>
        private void DisplaySettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppDisplaySettings.AppBackgroundColorBrush))
            {
                AppBackgroundColorBrush = _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;
            }
            else if (e.PropertyName == nameof(AppDisplaySettings.AppForegroundColorBrush))
            {
                AppForegroundColorBrush = _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
            }
        }
        /// <summary>
        /// Handles ImagesRetrieved event from ImageService to update background images.
        /// </summary>
        private void ImageService_ImagesRetrieved(object sender, EventArgs e)
        {
            BackgroundImages = _imageService.BackgroundImages;
        }       
    }
}
