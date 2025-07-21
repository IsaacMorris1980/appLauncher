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
    /// ViewModel for the AppTilesSettings page, managing the appearance settings for app tiles.
    /// Allows applying settings to a single selected app or all apps.
    /// </summary>
    public class AppTilesSettingsViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;

        // Collections for UI binding
        public ObservableCollection<FinalTiles> AllAvailableApps { get; private set; }
        public ObservableCollection<ColorComboItem> AvailableColors { get; private set; }
        public ObservableCollection<FinalTiles> PreviewApps { get; private set; } = new ObservableCollection<FinalTiles>();

        private FinalTiles _selectedApp;
        /// <summary>
        /// Gets or sets the currently selected app from the list.
        /// When set, its current colors/opacities are loaded into the sliders/comboboxes.
        /// </summary>
        public FinalTiles SelectedApp
        {
            get => _selectedApp;
            set
            {
                if (SetProperty(ref _selectedApp, value))
                {
                    UpdateEditControlsFromSelectedApp();
                    PreviewChangesCommand.RaiseCanExecuteChanged();
                    SaveChangesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isAllAppsMode;
        /// <summary>
        /// Gets or sets a value indicating whether settings apply to all apps (true) or a single selected app (false).
        /// </summary>
        public bool IsAllAppsMode
        {
            get => _isAllAppsMode;
            set
            {
                if (SetProperty(ref _isAllAppsMode, value))
                {
                    // If switching to all apps mode, clear selected app and reset controls
                    if (value)
                    {
                        SelectedApp = null;
                        ResetEditControlsForGlobalMode();
                    }
                    else
                    {
                        // If switching to single app mode, ensure a default app is selected if possible
                        if (AllAvailableApps.Any() && SelectedApp == null)
                        {
                            SelectedApp = AllAvailableApps.FirstOrDefault();
                        }
                        else if (SelectedApp != null)
                        {
                            UpdateEditControlsFromSelectedApp(); // Re-initialize if an app was already selected
                        }
                        else
                        {
                            // No apps available, keep controls disabled
                            ResetEditControlsForGlobalMode(); // Essentially disable if no app to select
                        }
                    }
                    PreviewChangesCommand.RaiseCanExecuteChanged();
                    SaveChangesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // Properties for Logo Color
        private ColorComboItem _selectedTileLogoColorItem;
        public ColorComboItem SelectedTileLogoColorItem
        {
            get => _selectedTileLogoColorItem;
            set
            {
                if (SetProperty(ref _selectedTileLogoColorItem, value))
                {
                    if (value != null)
                    {
                        UpdateTargetAppColor(value.ColorName.ToColor(), LogoOpacity,"logo");
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
                    UpdateTargetAppColor( _selectedTileLogoColorItem?.ColorName.ToColor(), value,"logo");
                }
            }
        }

        // Properties for Background Color
        private ColorComboItem _selectedTileBackColorItem;
        public ColorComboItem SelectedTileBackColorItem
        {
            get => _selectedTileBackColorItem;
            set
            {
                if (SetProperty(ref _selectedTileBackColorItem, value))
                {
                    if (value != null)
                    {
                        UpdateTargetAppColor( value.ColorName.ToColor(), TileBackOpacity,"back");
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
                    UpdateTargetAppColor( _selectedTileBackColorItem?.ColorName.ToColor(), value,"back");
                }
            }
        }

        // Properties for Text Color
        private ColorComboItem _selectedTileTextColorItem;
        public ColorComboItem SelectedTileTextColorItem
        {
            get => _selectedTileTextColorItem;
            set
            {
                if (SetProperty(ref _selectedTileTextColorItem, value))
                {
                    if (value != null)
                    {
                        UpdateTargetAppColor( value.ColorName.ToColor(), TileTextOpacity,"text");
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
                    UpdateTargetAppColor(_selectedTileTextColorItem?.ColorName.ToColor(), value,"text");
                }
            }
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;

        public string AppToggleTip => IsAllAppsMode ? "Settings will apply to all applications." : "Settings will apply to the selected application.";


        // Commands
        public ViewModelBase.Command ToggleAllAppsModeCommand { get; private set; }
        public ViewModelBase.Command<FinalTiles> AppSelectionChangedCommand { get; private set; } // For ListBox/ComboBox selection
        public ViewModelBase.Command PreviewChangesCommand { get; private set; }
        public ViewModelBase.AsyncCommand SaveChangesCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AppTilesSettingsViewModel"/> class.
        /// </summary>
        /// <param name="packageService">The service for managing application packages.</param>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public AppTilesSettingsViewModel(IPackageService packageService, ISettingsService settingsService, ILoggingService loggingService)
        {
            _packageService = packageService;
            _settingsService = settingsService;
            _loggingService = loggingService;

            // Initialize collections
            AllAvailableApps = new ObservableCollection<FinalTiles>(_packageService.Apps.GetOriginalCollection().OfType<FinalTiles>());
            AvailableColors = new ObservableCollection<ColorComboItem>(_settingsService.GetStaticColorPropertyBag());

            InitializeCommands();

            // Initial setup: Default to single app mode and select the first app if available
            IsAllAppsMode = false;
            if (AllAvailableApps.Any())
            {
                SelectedApp = AllAvailableApps.FirstOrDefault();
            }
            else
            {
                // If no apps, disable controls
                ResetEditControlsForGlobalMode();
            }

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
        /// Initializes the commands for the ViewModel.
        /// </summary>
        private void InitializeCommands()
        {
            ToggleAllAppsModeCommand = new ViewModelBase.Command(() => IsAllAppsMode = !IsAllAppsMode);
            AppSelectionChangedCommand = new ViewModelBase.Command<FinalTiles>(app => SelectedApp = app);
            PreviewChangesCommand = new ViewModelBase.Command(ExecutePreviewChanges, CanExecutePreviewChanges);
            SaveChangesCommand = new ViewModelBase.AsyncCommand(ExecuteSaveChangesAsync, CanExecuteSaveChanges);
        }

        /// <summary>
        /// Updates the preview area with the current app's appearance.
        /// </summary>
        private void ExecutePreviewChanges()
        {
            PreviewApps.Clear();
            if (SelectedApp != null)
            {
                // Create a temporary copy for preview if you don't want live updates on the main object
                // For simplicity, we're adding the SelectedApp directly, which reflects live changes.
                PreviewApps.Add(SelectedApp);
            }
        }

        /// <summary>
        /// Determines if the preview changes command can execute.
        /// </summary>
        private bool CanExecutePreviewChanges()
        {
            return SelectedApp != null;
        }

        /// <summary>
        /// Saves the changes made to the app tile settings.
        /// </summary>
        private async Task ExecuteSaveChangesAsync()
        {
            try
            {
                if (IsAllAppsMode)
                {
                    // Apply changes to all apps
                    foreach (var app in _packageService.Apps.GetOriginalCollection().OfType<FinalTiles>())
                    {
                        app.TextColor = SelectedApp.TextColor;
                        app.LogoColor = SelectedApp.LogoColor;
                        app.BackColor = SelectedApp.BackColor;
                    }
                }
                else if (SelectedApp != null)
                {
                    // The SelectedApp object is already a reference to the one in the main collection,
                    // so changes are live.
                    // No explicit update needed for the collection item itself, just save.
                }
                else
                {
                    // No app selected in single mode, nothing to save
                    return;
                }

                await _packageService.SaveAppCollectionAsync(); // Save changes to disk
                System.Diagnostics.Debug.WriteLine("App tile settings saved successfully.");
                ResetAppTilePageUI(); // Reset UI state after saving
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                System.Diagnostics.Debug.WriteLine($"Error saving app tile settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Determines if the save changes command can execute.
        /// </summary>
        private bool CanExecuteSaveChanges()
        {
            // Can save if in all apps mode, or if a specific app is selected in single app mode
            return IsAllAppsMode || SelectedApp != null;
        }

        /// <summary>
        /// Updates the color properties of the target app (or the temporary SelectedApp)
        /// based on the selected color item and opacity slider value.
        /// </summary>
        /// <param name="targetColor">The color property to update (e.g., SelectedApp.LogoColor).</param>
        /// <param name="newBaseColor">The base color from the selected ColorComboItem.</param>
        /// <param name="opacityValue">The opacity value from the slider (0-10).</param>
        private void UpdateTargetAppColor(Color? newBaseColor, double opacityValue,string brush)
        {
            if (SelectedApp == null) return; // Cannot update if no app is selected
          
            switch (brush)
            {
                case "back":
                  Color  baseColor = newBaseColor ?? SelectedApp.BackColor; // Use new base color if provided, else keep current
                    byte opacity = Convert.ToByte((opacityValue / 10.0) * 255);
                    SelectedApp.BackColor = Color.FromArgb(opacity, baseColor.R, baseColor.G, baseColor.B);
                    break;
                case "logo":
                    Color baseColor1 = newBaseColor ?? SelectedApp.LogoColor; // Use new base color if provided, else keep current
                    byte opacity1 = Convert.ToByte((opacityValue / 10.0) * 255);
                    SelectedApp.LogoColor = Color.FromArgb(opacity1, baseColor1.R, baseColor1.G, baseColor1.B);
                    break;
                case "text":
                    Color baseColor2 = newBaseColor ?? SelectedApp.TextColor; // Use new base color if provided, else keep current
                    byte opacity2 = Convert.ToByte((opacityValue / 10.0) * 255);
                    SelectedApp.TextColor = Color.FromArgb(opacity2, baseColor2.R, baseColor2.G, baseColor2.B);
                    break;
                default:
                    break;
            }
    

            // Notify property changed for the specific color brush on the SelectedApp
            // This assumes FinalTiles has INotifyPropertyChanged and raises for its brushes
            SelectedApp.OnPropertyChanged(nameof(SelectedApp.LogoBrush));
            SelectedApp.OnPropertyChanged(nameof(SelectedApp.BackBrush));
            SelectedApp.OnPropertyChanged(nameof(SelectedApp.TextBrush));
        }

        /// <summary>
        /// Initializes the UI controls for editing based on the currently selected app's properties.
        /// </summary>
        private void UpdateEditControlsFromSelectedApp()
        {
            if (SelectedApp != null)
            {
                SelectedTileLogoColorItem = _settingsService.MatchColor(SelectedApp.LogoColor);
                LogoOpacity = ReturnOpacity(SelectedApp.LogoColor.A);

                SelectedTileBackColorItem = _settingsService.MatchColor(SelectedApp.BackColor);
                TileBackOpacity = ReturnOpacity(SelectedApp.BackColor.A);

                SelectedTileTextColorItem = _settingsService.MatchColor(SelectedApp.TextColor);
                TileTextOpacity = ReturnOpacity(SelectedApp.TextColor.A);

                ExecutePreviewChanges(); // Update preview immediately
            }
            else
            {
                // If no app is selected (e.g., when switching to "All Apps" mode)
                ResetEditControlsForGlobalMode();
            }
        }

        /// <summary>
        /// Resets the edit controls' values, typically when no specific app is selected
        /// or when switching to "All Apps" mode.
        /// Sets them to default or a state that indicates no individual app is being edited.
        /// </summary>
        private void ResetEditControlsForGlobalMode()
        {
            // Set to null or default values
            SelectedTileLogoColorItem = null;
            LogoOpacity = 0; // Or some default neutral value
            SelectedTileBackColorItem = null;
            TileBackOpacity = 0;
            SelectedTileTextColorItem = null;
            TileTextOpacity = 0;
            PreviewApps.Clear(); // Clear preview when no app is selected
        }

        /// <summary>
        /// Resets the UI state after changes are saved or mode is toggled.
        /// This method is called from the ViewModel to affect UI state.
        /// </summary>
        private void ResetAppTilePageUI()
        {
            // After saving, ensure the correct mode is reflected and controls are updated.
            // If in "All Apps" mode, settings are applied globally, and we might want to reset the selected app.
            if (IsAllAppsMode)
            {
                SelectedApp = null; // Clear selection
                ResetEditControlsForGlobalMode();
            }
            else if (SelectedApp != null)
            {
                // If in single app mode, re-load the app's properties to ensure UI reflects saved state
                // This might involve re-fetching the app from _packageService.Apps or simply re-setting the SelectedApp
                UpdateEditControlsFromSelectedApp();
            }
            PreviewApps.Clear(); // Clear preview after saving
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
