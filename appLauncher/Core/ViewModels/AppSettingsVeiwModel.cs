using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using Microsoft.Toolkit.Uwp.Helpers; // For ToColor() extension

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI; // For Color
using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the AppSettings page, managing application display settings.
    /// </summary>
    public class AppSettingsViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;
        private readonly IImageService _imageService; // To update background image rotation


        // Properties for UI Binding
        public ObservableCollection<ColorComboItem> AvailableColors { get; private set; }

        private ColorComboItem _selectedAppTextColorItem;
        private bool _isOn;
        private bool _sync;
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (SetProperty(ref _isOn, value))
                {
                    _settingsService.AppSettings.NetworkSettings.WifiOnly = value;
                }
            }
        }
        public bool IsSync
        {
            get => _sync;
            set
            {
                if (SetProperty(ref _sync, value))
                { 
                _settingsService.AppSettings.NetworkSettings.Sync = value;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the selected item for application text color.
        /// </summary>
        public ColorComboItem SelectedAppTextColorItem
        {
            get => _selectedAppTextColorItem;
            set
            {
                if (SetProperty(ref _selectedAppTextColorItem, value))
                {
                    if (value != null)
                    {
                        // Update the underlying setting, preserving opacity
                        Color newColor = value.ColorName.ToColor();
                        newColor.A = _settingsService.AppSettings.DisplaySettings.AppForgroundColor.A;
                        _settingsService.AppSettings.DisplaySettings.AppForgroundColor = newColor;
                        OnPropertyChanged(nameof(AppForegroundColorBrush)); // Notify UI of brush change
                    }
                }
            }
        }

        private double _appTextOpacity;
        /// <summary>
        /// Gets or sets the opacity for application text (0-10 scale).
        /// </summary>
        public double AppTextOpacity
        {
            get => _appTextOpacity;
            set
            {
                if (SetProperty(ref _appTextOpacity, value))
                {
                    // Update the underlying setting
                    byte opacity = Convert.ToByte((value / 10.0) * 255);
                    Color currentColor = _settingsService.AppSettings.DisplaySettings.AppForgroundColor;
                    _settingsService.AppSettings.DisplaySettings.AppForgroundColor = Color.FromArgb(opacity, currentColor.R, currentColor.G, currentColor.B);
                    OnPropertyChanged(nameof(AppForegroundColorBrush)); // Notify UI of brush change
                }
            }
        }

        private ColorComboItem _selectedAppBackColorItem;
        /// <summary>
        /// Gets or sets the selected item for application background color.
        /// </summary>
        public ColorComboItem SelectedAppBackColorItem
        {
            get => _selectedAppBackColorItem;
            set
            {
                if (SetProperty(ref _selectedAppBackColorItem, value))
                {
                    if (value != null)
                    {
                        // Update the underlying setting, preserving opacity
                        Color newColor = value.ColorName.ToColor();
                        newColor.A = _settingsService.AppSettings.DisplaySettings.AppBackgroundColor.A;
                        _settingsService.AppSettings.DisplaySettings.AppBackgroundColor = newColor;
                        OnPropertyChanged(nameof(AppBackgroundColorBrush)); // Notify UI of brush change
                    }
                }
            }
        }

        private double _appBackOpacity;
        /// <summary>
        /// Gets or sets the opacity for application background (0-10 scale).
        /// </summary>
        public double AppBackOpacity
        {
            get => _appBackOpacity;
            set
            {
                if (SetProperty(ref _appBackOpacity, value))
                {
                    // Update the underlying setting
                    byte opacity = Convert.ToByte((value / 10.0) * 255);
                    Color currentColor = _settingsService.AppSettings.DisplaySettings.AppBackgroundColor;
                    _settingsService.AppSettings.DisplaySettings.AppBackgroundColor = Color.FromArgb(opacity, currentColor.R, currentColor.G, currentColor.B);
                    OnPropertyChanged(nameof(AppBackgroundColorBrush)); // Notify UI of brush change
                }
            }
        }

        private string _imageRotationTimeInput;
        /// <summary>
        /// Gets or sets the text input for image rotation time in seconds.
        /// </summary>
        public string ImageRotationTimeInput
        {
            get => _imageRotationTimeInput;
            set => SetProperty(ref _imageRotationTimeInput, value);
        }

        // Brushes for UI elements that reflect current settings
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;

        // Commands
        public ViewModelBase.AsyncCommand SaveSettingsCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsViewModel"/> class.
        /// </summary>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        /// <param name="imageService">The service for managing background images (to re-start timer).</param>
        public AppSettingsViewModel(ISettingsService settingsService, ILoggingService loggingService, IImageService imageService)
        {
            _settingsService = settingsService;
            _loggingService = loggingService;
            _imageService = imageService;

            AvailableColors = new ObservableCollection<ColorComboItem>(_settingsService.GetStaticColorPropertyBag());

            InitializePropertiesFromSettings();
            InitializeCommands();

            // Subscribe to settings changes to update brushes if the theme changes from other parts of the app
            _settingsService.AppSettings.DisplaySettings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppDisplaySettings.AppForegroundColorBrush))
                {
                    OnPropertyChanged(nameof(AppForegroundColorBrush));
                }
                else if (e.PropertyName == nameof(AppDisplaySettings.AppBackgroundColorBrush))
                {
                    OnPropertyChanged(nameof(AppBackgroundColorBrush));
                }
            };
        }

        /// <summary>
        /// Initializes ViewModel properties from the current application settings.
        /// </summary>
        private void InitializePropertiesFromSettings()
        {
            SelectedAppTextColorItem = _settingsService.MatchColor(_settingsService.AppSettings.DisplaySettings.AppForgroundColor);
            AppTextOpacity = ReturnOpacity(_settingsService.AppSettings.DisplaySettings.AppForgroundColor.A);

            SelectedAppBackColorItem = _settingsService.MatchColor(_settingsService.AppSettings.DisplaySettings.AppBackgroundColor);
            AppBackOpacity = ReturnOpacity(_settingsService.AppSettings.DisplaySettings.AppBackgroundColor.A);

            ImageRotationTimeInput = _settingsService.AppSettings.DisplaySettings.ImageRotationTime.TotalSeconds.ToString();
        }

        /// <summary>
        /// Initializes the commands for the ViewModel.
        /// </summary>
        private void InitializeCommands()
        {
            SaveSettingsCommand = new ViewModelBase.AsyncCommand(ExecuteSaveSettingsAsync);
        }

        /// <summary>
        /// Executes the save settings operation.
        /// </summary>
        private async Task ExecuteSaveSettingsAsync()
        {
            try
            {
                // Parse and set ImageRotationTime
                if (int.TryParse(ImageRotationTimeInput, out int timeInSeconds) && timeInSeconds >= 0)
                {
                    _settingsService.AppSettings.DisplaySettings.ImageRotationTime = TimeSpan.FromSeconds(timeInSeconds);
                }
                else
                {
                    // Handle invalid input, revert to default or show error
                    _settingsService.AppSettings.DisplaySettings.ImageRotationTime = TimeSpan.FromSeconds(15); // Default
                    ImageRotationTimeInput = "15"; // Update UI to reflect default
                    await _loggingService.LogExceptionAsync(new ArgumentException("Invalid Image Rotation Time input. Reverted to default (15s)."));
                }

                // Apply current settings to global resources
                _settingsService.SetApplicationResources();

                // Save all application settings
                await _settingsService.SaveAppSettingsAsync();

                // Re-start image rotation timer with new settings
                await _imageService.SetNextBackgroundImage(); // Set current image based on new settings
                await _imageService.LoadBackgroundImages(); // Reload images and restart timer with new interval

                System.Diagnostics.Debug.WriteLine("Settings saved successfully.");
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Converts an opacity byte value (0-255) to a 0-10 scale for the slider.
        /// </summary>
        /// <param name="opacity">The opacity value (0-255).</param>
        /// <returns>Opacity on a 0-10 scale.</returns>
        private double ReturnOpacity(byte opacity)
        {
            return Math.Round((opacity / 255.0) * 10.0, 0);
        }
    }
}
