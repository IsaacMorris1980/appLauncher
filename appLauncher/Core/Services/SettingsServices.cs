using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services; // Ensure these using statements are correct for your project structure

using Microsoft.Toolkit.Uwp.Helpers; // For ToColor() extension

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Reflection; // For Color reflection
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI; // For Color
using Windows.UI.Xaml; // For Application.Current.Resources

namespace appLauncher.Core.Services
{
    /// <summary>
    /// Provides services for managing application settings, including loading, saving, and applying UI resources.
    /// This service utilizes IFileUtilityService for file operations and ILoggingService for error logging.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly ILoggingService _loggingService;
        private readonly IFileService _fileUtilityService;

        /// <summary>
        /// Gets the root application settings object, which contains all sub-settings.
        /// This property is the primary access point for all application settings.
        /// </summary>
        public AppSettings AppSettings { get; private set; } = new AppSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="loggingService">The logging service for error handling.</param>
        /// <param name="fileUtilityService">The file utility service for file presence checks and I/O operations.</param>
        public SettingsService(ILoggingService loggingService, IFileService fileUtilityService)
        {
            _loggingService = loggingService;
            _fileUtilityService = fileUtilityService;
        }

        /// <summary>
        /// Loads application settings from local storage. If no settings file is found or loading fails,
        /// default settings are used. This method also ensures that static color properties and
        /// app version info are correctly initialized.
        /// </summary>
        public async Task LoadAppSettingsAsync()
        {
            // Initialize AppColors as it's a static list and not serialized with the settings.
            AppSettings.DisplaySettings.AppColors = GetStaticColorPropertyBag();

            string settingsJson = await _fileUtilityService.ReadTextFromFileAsync("appsettings.json"); // Use FileUtilityService
            if (!string.IsNullOrEmpty(settingsJson))
            {
                try
                {
                    // Deserialize the entire AppSettings graph
                    AppSettings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);

                    // Re-populate AppColors after deserialization (as it's [JsonIgnore]d)
                    AppSettings.DisplaySettings.AppColors = GetStaticColorPropertyBag();
                }
                catch (Exception es)
                {
                    await _loggingService.LogExceptionAsync(es); // Changed method name
                    // If loading fails, re-initialize with defaults
                    AppSettings = new AppSettings();
                    AppSettings.DisplaySettings.AppColors = GetStaticColorPropertyBag();
                }
            }
            else
            {
                // If file not present, initialize with defaults
                AppSettings = new AppSettings();
                AppSettings.DisplaySettings.AppColors = GetStaticColorPropertyBag();
            }

            // AppVersionInfo is dynamic and should always reflect the current package version,
            // so it's best to initialize it after loading other settings.
            AppSettings.VersionInfo = new AppVersionInfo();

            // Apply the loaded/default display settings to the application's UI resources
            SetApplicationResources();
        }

        /// <summary>
        /// Saves the current application settings to local storage.
        /// </summary>
        public async Task SaveAppSettingsAsync()
        {
            try
            {
                // Serialize the entire AppSettings graph.
                // Ensure properties like brushes and IPEndPoint are marked with [JsonIgnore]
                // or have custom converters if they need to be serialized in a specific way.
                var settingsJson = JsonConvert.SerializeObject(AppSettings, Formatting.Indented);

                await _fileUtilityService.WriteTextToFileAsync("appsettings.json", settingsJson); // Use FileUtilityService
            }
            catch (Exception es)
            {
                await _loggingService.LogExceptionAsync(es); // Changed method name
            }
        }

        /// <summary>
        /// Retrieves a list of all static color properties from the Windows.UI.Colors class.
        /// </summary>
        /// <returns>A list of <see cref="ColorComboItem"/> representing available colors.</returns>
        public List<ColorComboItem> GetStaticColorPropertyBag()
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            List<ColorComboItem> map = new List<ColorComboItem>();
            foreach (var prop in typeof(Colors).GetProperties(flags))
            {
                ColorComboItem colorItem = new ColorComboItem();
                colorItem.ColorName = prop.Name;
                colorItem.ColorBrush = new Windows.UI.Xaml.Media.SolidColorBrush(prop.Name.ToColor());
                map.Add(colorItem);
            }
            return map;
        }

        /// <summary>
        /// Matches a given <see cref="Color"/> to its corresponding <see cref="ColorComboItem"/>
        /// based on the static color properties.
        /// </summary>
        /// <param name="c">The <see cref="Color"/> to match.</param>
        /// <returns>The matching <see cref="ColorComboItem"/> or null if no match is found.</returns>
        public ColorComboItem MatchColor(Color c)
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            Type t = typeof(Colors);
            ColorComboItem colorItem = new ColorComboItem();
            foreach (var prop in t.GetProperties(flags))
            {
                if (prop.Name.ToColor() == c)
                {
                    colorItem.ColorName = prop.Name;
                    colorItem.ColorBrush = new Windows.UI.Xaml.Media.SolidColorBrush(prop.Name.ToColor());
                    return colorItem; // Return once matched
                }
            }
            return null; // Return null if no match found
        }

        /// <summary>
        /// Applies the application's display settings (foreground and background colors)
        /// to the global application resources, affecting the UI theme.
        /// </summary>
        public void SetApplicationResources()
        {
            // Access display settings via the AppSettings object and its nested DisplaySettings
            Application.Current.Resources["AppBarButtonForegroundPointerOver"] = AppSettings.DisplaySettings.AppForegroundColorBrush;
            Application.Current.Resources["AppBarButtonBackgroundPointerOver"] = AppSettings.DisplaySettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackground"] = AppSettings.DisplaySettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackgroundPointerOver"] = AppSettings.DisplaySettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxForeground"] = AppSettings.DisplaySettings.AppForegroundColorBrush;
            Application.Current.Resources["ComboBoxDropDownBackground"] = AppSettings.DisplaySettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxPlaceHolderForeground"] = AppSettings.DisplaySettings.AppForegroundColorBrush;
        }
    }
}