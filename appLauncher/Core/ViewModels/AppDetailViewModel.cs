using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using Microsoft.Toolkit.Uwp.Helpers; // For ToColor() extension

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Windows.UI; // For Color
using Windows.UI.Xaml.Media; // For SolidColorBrush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the AppDetail page, handling both display of app information
    /// and editing of its tile appearance properties.
    /// </summary>
    public class AppDetailViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;

        private FinalTiles _currentApp;
        /// <summary>
        /// Gets or sets the <see cref="FinalTiles"/> object currently being viewed or edited.
        /// </summary>
        public FinalTiles CurrentApp
        {
            get => _currentApp;
            set => SetProperty(ref _currentApp, value);
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
                    // Update preview when mode changes
                    UpdatePreview();
                    // Re-evaluate save command can execute state
                    SaveChangesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // Properties for the "Favorite" checkbox (from AppInformation)
        private bool _isFavorite;
        /// <summary>
        /// Gets or sets a value indicating whether the app is marked as a favorite.
        /// This property is two-way bound to the UI checkbox.
        /// </summary>
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (SetProperty(ref _isFavorite, value))
                {
                    if (CurrentApp != null)
                    {
                        CurrentApp.Favorite = value;
                        // Save the change immediately
                        _packageService.SaveAppCollectionAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        // Properties for the preview GridView (from EditApp)
        public ObservableCollection<FinalTiles> PreviewApps { get; private set; } = new ObservableCollection<FinalTiles>();
        public ObservableCollection<ColorComboItem> AvailableColors { get; private set; }

        // UI-bound properties for colors and opacity (from EditApp)
        private ColorComboItem _selectedTileLogoColorItem;
        public ColorComboItem SelectedTileLogoColorItem
        {
            get => _selectedTileLogoColorItem;
            set
            {
                if (SetProperty(ref _selectedTileLogoColorItem, value))
                {
                    if (CurrentApp != null && value != null)
                    {
                        Color newColor = value.ColorName.ToColor();
                        newColor.A = CurrentApp.LogoColor.A; // Preserve opacity
                        CurrentApp.LogoColor = newColor;
                        UpdatePreview();
                    }
                }
            }
        }

        private double _logoOpacity;
        public double LogoOpacity
        {
            get => _logoOpacity;
            set
            {
                if (SetProperty(ref _logoOpacity, value))
                {
                    if (CurrentApp != null)
                    {
                        byte opacity = Convert.ToByte((value / 10.0) * 255);
                        Color currentColor = CurrentApp.LogoColor;
                        CurrentApp.LogoColor = Color.FromArgb(opacity, currentColor.R, currentColor.G, currentColor.B);
                        UpdatePreview();
                    }
                }
            }
        }

        private ColorComboItem _selectedTileBackColorItem;
        public ColorComboItem SelectedTileBackColorItem
        {
            get => _selectedTileBackColorItem;
            set
            {
                if (SetProperty(ref _selectedTileBackColorItem, value))
                {
                    if (CurrentApp != null && value != null)
                    {
                        Color newColor = value.ColorName.ToColor();
                        newColor.A = CurrentApp.BackColor.A; // Preserve opacity
                        CurrentApp.BackColor = newColor;
                        UpdatePreview();
                    }
                }
            }
        }

        private double _tileBackOpacity;
        public double TileBackOpacity
        {
            get => _tileBackOpacity;
            set
            {
                if (SetProperty(ref _tileBackOpacity, value))
                {
                    if (CurrentApp != null)
                    {
                        byte opacity = Convert.ToByte((value / 10.0) * 255);
                        Color currentColor = CurrentApp.BackColor;
                        CurrentApp.BackColor = Color.FromArgb(opacity, currentColor.R, currentColor.G, currentColor.B);
                        UpdatePreview();
                    }
                }
            }
        }

        private ColorComboItem _selectedTileTextColorItem;
        public ColorComboItem SelectedTileTextColorItem
        {
            get => _selectedTileTextColorItem;
            set
            {
                if (SetProperty(ref _selectedTileTextColorItem, value))
                {
                    if (CurrentApp != null && value != null)
                    {
                        Color newColor = value.ColorName.ToColor();
                        newColor.A = CurrentApp.TextColor.A; // Preserve opacity
                        CurrentApp.TextColor = newColor;
                        UpdatePreview();
                    }
                }
            }
        }

        private double _tileTextOpacity;
        public double TileTextOpacity
        {
            get => _tileTextOpacity;
            set
            {
                if (SetProperty(ref _tileTextOpacity, value))
                {
                    if (CurrentApp != null)
                    {
                        byte opacity = Convert.ToByte((value / 10.0) * 255);
                        Color currentColor = CurrentApp.TextColor;
                        CurrentApp.TextColor = Color.FromArgb(opacity, currentColor.R, currentColor.G, currentColor.B);
                        UpdatePreview();
                    }
                }
            }
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;


        // Commands
        public Command ToggleEditModeCommand { get; private set; }
        public AsyncCommand SaveChangesCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AppDetailViewModel"/> class.
        /// </summary>
        /// <param name="packageService">The service for managing application packages.</param>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public AppDetailViewModel(IPackageService packageService, ISettingsService settingsService, ILoggingService loggingService)
        {
            _packageService = packageService;
            _settingsService = settingsService;
            _loggingService = loggingService;

            AvailableColors = new ObservableCollection<ColorComboItem>(_settingsService.GetStaticColorPropertyBag());

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
        /// Sets the app tile for the ViewModel. This is typically called when navigating to the page.
        /// </summary>
        /// <param name="appTile">The <see cref="FinalTiles"/> object to display/edit.</param>
        public void SetApp(FinalTiles appTile)
        {
            CurrentApp = appTile;
            if (CurrentApp != null)
            {
                IsFavorite = CurrentApp.Favorite; // Initialize IsFavorite from the app tile
                UpdatePropertiesFromCurrentApp(); // Initialize edit properties
                UpdatePreview(); // Show initial preview
            }
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            ToggleEditModeCommand = new Command(() => IsEditMode = !IsEditMode);
            SaveChangesCommand = new AsyncCommand(SaveChangesAsync, () => IsEditMode && CurrentApp != null);
        }

        /// <summary>
        /// Updates the preview area with the current app's appearance.
        /// </summary>
        private void UpdatePreview()
        {
            PreviewApps.Clear();
            if (CurrentApp != null)
            {
                // Create a temporary copy for preview if you don't want live updates on the main object
                // For simplicity, we're adding the CurrentApp directly, which reflects live changes.
                PreviewApps.Add(CurrentApp);
            }
        }

        /// <summary>
        /// Updates ViewModel properties (like selected colors and opacities) based on the current app.
        /// </summary>
        private void UpdatePropertiesFromCurrentApp()
        {
            if (CurrentApp != null)
            {
                SelectedTileLogoColorItem = _settingsService.MatchColor(CurrentApp.LogoColor);
                LogoOpacity = ReturnOpacity((int)CurrentApp.LogoColor.A);

                SelectedTileBackColorItem = _settingsService.MatchColor(CurrentApp.BackColor);
                TileBackOpacity = ReturnOpacity((int)CurrentApp.BackColor.A);

                SelectedTileTextColorItem = _settingsService.MatchColor(CurrentApp.TextColor);
                TileTextOpacity = ReturnOpacity((int)CurrentApp.TextColor.A);
            }
            else
            {
                // Clear properties if no app is selected
                SelectedTileLogoColorItem = null;
                LogoOpacity = 0;
                SelectedTileBackColorItem = null;
                TileBackOpacity = 0;
                SelectedTileTextColorItem = null;
                TileTextOpacity = 0;
            }
        }

        /// <summary>
        /// Saves the changes made to the app tile settings.
        /// </summary>
        private async Task SaveChangesAsync()
        {
            try
            {
                if (CurrentApp != null)
                {
                    // The CurrentApp object is already a reference to the one in the main collection,
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
        /// Converts an opacity byte value (0-255) to a 0-10 scale.
        /// </summary>
        private int ReturnOpacity(int opacity)
        {
            return (int)Math.Round((opacity / 255.0) * 10.0, 0);
        }
    }
}
