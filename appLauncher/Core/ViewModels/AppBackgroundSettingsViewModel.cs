using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage; // For FileOpenPicker, StorageFile
using Windows.UI.Popups; // For MessageDialog (for user feedback)
using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the AppBackgroundSettings page, managing background image selection and removal.
    /// </summary>
    public class AppBackgroundSettingsViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly IImageService _imageService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Gets the observable collection of background images managed by the ImageService.
        /// This collection is directly bound to the ListView in the UI.
        /// </summary>
        public ObservableCollection<PageBackgrounds> BackgroundImages => _imageService.BackgroundImages;

        private PageBackgrounds _selectedBackgroundImage;
        /// <summary>
        /// Gets or sets the currently selected background image in the ListView.
        /// </summary>
        public PageBackgrounds SelectedBackgroundImage
        {
            get => _selectedBackgroundImage;
            set
            {
                if (SetProperty(ref _selectedBackgroundImage, value))
                {
                    RemoveImageCommand.RaiseCanExecuteChanged(); // Update CanExecute for remove button
                }
            }
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;

        // Commands
        public ViewModelBase.AsyncCommand AddImageCommand { get; private set; }
        public ViewModelBase.Command RemoveImageCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBackgroundSettingsViewModel"/> class.
        /// </summary>
        /// <param name="imageService">The service for managing background images.</param>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public AppBackgroundSettingsViewModel(IImageService imageService, ISettingsService settingsService, ILoggingService loggingService)
        {
            _imageService = imageService;
            _settingsService = settingsService;
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
        /// Initializes the commands for the ViewModel.
        /// </summary>
        private void InitializeCommands()
        {
            AddImageCommand = new ViewModelBase.AsyncCommand(ExecuteAddImageAsync);
            RemoveImageCommand = new ViewModelBase.Command(ExecuteRemoveImage, () => SelectedBackgroundImage != null);
        }

        /// <summary>
        /// Executes the process to add new background images from file picker.
        /// </summary>
        private async Task ExecuteAddImageAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
                };

                // Add supported file types
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".jpe");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".svg");
                picker.FileTypeFilter.Add(".tif");
                picker.FileTypeFilter.Add(".tiff");
                picker.FileTypeFilter.Add(".bmp");
                picker.FileTypeFilter.Add(".jif");
                picker.FileTypeFilter.Add(".jfif");
                picker.FileTypeFilter.Add(".gif");
                picker.FileTypeFilter.Add(".gifv");

                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                if (files != null && files.Any())
                {
                    foreach (StorageFile item in files)
                    {
                        // Convert image file to byte array using ImageService's method
                        byte[] imageBytes = await _imageService.ConvertImageFiletoByteArrayAsync(item);

                        if (imageBytes != null)
                        {
                            _imageService.AddPageBackground(new PageBackgrounds
                            {
                                BackgroundImageDisplayName = item.DisplayName,
                                FilePath = item.Path, // Storing path for potential future use/reference
                                BackgroundImageBytes = imageBytes
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to convert image '{item.DisplayName}' to bytes.");
                        }
                    }
                    await _imageService.SaveImageOrder(); // Save changes after adding
                    await _imageService.SetNextBackgroundImage(); // Update background immediately
                    System.Diagnostics.Debug.WriteLine("Background images added and saved.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Operation cancelled: No files selected.");
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                System.Diagnostics.Debug.WriteLine($"An error occurred while adding background images: {ex.Message}");
                // Optionally, show a message dialog to the user
                await new MessageDialog($"Error adding images: {ex.Message}", "Error").ShowAsync();
            }
        }

        /// <summary>
        /// Executes the process to remove the currently selected background image.
        /// </summary>
        private async void ExecuteRemoveImage()
        {
            if (SelectedBackgroundImage != null)
            {
                try
                {
                    // Confirm with the user before removing
                    var dialog = new MessageDialog($"Are you sure you want to remove '{SelectedBackgroundImage.BackgroundImageDisplayName}'?", "Confirm Removal");
                    dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(async (command) =>
                    {
                        _imageService.RemovePageBackground(SelectedBackgroundImage.BackgroundImageDisplayName);
                        await _imageService.SaveImageOrder(); // Save changes after removal
                        await _imageService.SetNextBackgroundImage(); // Update background immediately
                        SelectedBackgroundImage = null; // Clear selection after removal
                        System.Diagnostics.Debug.WriteLine("Background image removed and saved.");
                    })));
                    dialog.Commands.Add(new UICommand("No"));
                    dialog.DefaultCommandIndex = 1; // "No" is default
                    await dialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    await _loggingService.LogExceptionsAsync(ex);
                    System.Diagnostics.Debug.WriteLine($"An error occurred while removing background image: {ex.Message}");
                    await new MessageDialog($"Error removing image: {ex.Message}", "Error").ShowAsync();
                }
            }
        }
    }
}
