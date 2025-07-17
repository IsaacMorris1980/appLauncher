using appLauncher.Core.Brushes;
using appLauncher.Core.Extensions;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services; // Add this using for ILoggingService, IFileUtilityService, and ISettingsService

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Services
{
    /// <summary>
    /// Provides services for managing application background images, including loading, saving, and rotation.
    /// This service utilizes IFileUtilityService for file operations, ILoggingService for error logging,
    /// and ISettingsService for accessing application display settings.
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly ILoggingService _loggingService;
        private readonly IFileService _fileUtilityService;
        private readonly ISettingsService _settingsService;

        public ObservableCollection<PageBackgrounds> BackgroundImages { get; private set; } = new ObservableCollection<PageBackgrounds>();
        private int _imageCurrentSelection = 0;
        private ThreadPoolTimer _threadpoolTimer;
        private PageImageBrush _images;
        private Brush _currentBackgroundBrush;
        private SolidColorBrush _backColor = new SolidColorBrush();

        /// <summary>
        /// Gets the currently active background brush for the application.
        /// This property can be bound to in the UI to display the background.
        /// </summary>
        public Brush CurrentBackgroundBrush
        {
            get { return _currentBackgroundBrush; }
            private set
            {
                if (_currentBackgroundBrush != value)
                {
                    _currentBackgroundBrush = value;
                    // In a full MVVM setup, you'd typically raise PropertyChanged here if this is bound
                    // directly to a UI element and not just updated via the ImagesRetrieved event.
                }
            }
        }

        /// <summary>
        /// Event fired when background images have been retrieved or updated,
        /// signaling that the UI might need to refresh its background.
        /// </summary>
        public event EventHandler ImagesRetrieved;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="loggingService">The logging service for error handling.</param>
        /// <param name="fileUtilityService">The file utility service for file presence checks and I/O.</param>
        /// <param name="settingsService">The settings service to access application settings like image rotation time and background color.</param>
        public ImageService(ILoggingService loggingService, IFileService fileUtilityService, ISettingsService settingsService)
        {
            _loggingService = loggingService;
            _fileUtilityService = fileUtilityService;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Sets the next background image in the collection or a solid color if no images are available.
        /// This method updates <see cref="CurrentBackgroundBrush"/> and is typically called by the rotation timer.
        /// </summary>
        public async Task SetNextBackgroundImage()
        {
            // Ensure UI updates happen on the UI thread
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (BackgroundImages.Count > 0)
                {
                    Debug.WriteLine($"ImageService: Setting background image. Current Count: {BackgroundImages.Count}");
                    // Cycle through images
                    _imageCurrentSelection = (_imageCurrentSelection >= BackgroundImages.Count - 1) ? 0 : _imageCurrentSelection + 1;
                    PageBackgrounds nextImage = BackgroundImages[_imageCurrentSelection];

                    // Attempt to create brush from image bytes
                    if (nextImage.BackgroundImageBytes != null && nextImage.BackgroundImageBytes.Length > 0)
                    {
                        try
                        {
                            // Create a RandomAccessStream from the byte array
                            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                            {
                                stream.WriteAsync(nextImage.BackgroundImageBytes.AsBuffer()).AsTask().Wait(); // Write bytes to stream
                                stream.Seek(0); // Reset stream position to beginning
                                _images = new PageImageBrush(stream); // Create PageImageBrush from stream
                                CurrentBackgroundBrush = _images;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error if image brush creation fails and fallback to solid color
                            _loggingService.LogExceptionAsync(ex).Wait(); // Log synchronously to avoid deadlocks in UI thread context
                            _backColor.Color = _settingsService.AppSettings.DisplaySettings.AppBackgroundColor;
                            CurrentBackgroundBrush = _backColor;
                            Debug.WriteLine("ImageService: Error creating image brush, falling back to solid color.");
                        }
                    }
                    else
                    {
                        // Fallback to solid color if image bytes are missing or empty
                        _backColor.Color = _settingsService.AppSettings.DisplaySettings.AppBackgroundColor;
                        CurrentBackgroundBrush = _backColor;
                        Debug.WriteLine("ImageService: Background image bytes were null or empty, falling back to solid color.");
                    }
                }
                else
                {
                    // No background images available, use the app's default background color
                    _backColor.Color = _settingsService.AppSettings.DisplaySettings.AppBackgroundColor;
                    CurrentBackgroundBrush = _backColor;
                    Debug.WriteLine("ImageService: No background images, using solid color from settings.");
                }
            });
            GC.WaitForPendingFinalizers(); // Allow garbage collection of previous image resources
        }

        /// <summary>
        /// Adds a new page background to the collection if it doesn't already exist.
        /// </summary>
        /// <param name="pageBackgrounds">The <see cref="PageBackgrounds"/> object to add.</param>
        public void AddPageBackground(PageBackgrounds pageBackgrounds)
        {
            if (BackgroundImages.Any(x => x.BackgroundImageDisplayName == pageBackgrounds.BackgroundImageDisplayName))
            {
                Debug.WriteLine($"ImageService: Background image '{pageBackgrounds.BackgroundImageDisplayName}' already exists, skipping add.");
                return;
            }
            else
            {
                BackgroundImages.Add(pageBackgrounds);
                Debug.WriteLine($"ImageService: Added background image '{pageBackgrounds.BackgroundImageDisplayName}'. Total: {BackgroundImages.Count}");
            }
        }

        /// <summary>
        /// Removes a page background from the collection based on its display name.
        /// </summary>
        /// <param name="pageBackgroundDisplayName">The display name of the background image to remove.</param>
        public void RemovePageBackground(string pageBackgroundDisplayName)
        {
            var itemToRemove = BackgroundImages.FirstOrDefault(x => x.BackgroundImageDisplayName == pageBackgroundDisplayName);
            if (itemToRemove != null)
            {
                BackgroundImages.Remove(itemToRemove);
                Debug.WriteLine($"ImageService: Removed background image '{pageBackgroundDisplayName}'. Total: {BackgroundImages.Count}");
            }
            else
            {
                Debug.WriteLine($"ImageService: Background image '{pageBackgroundDisplayName}' not found for removal.");
            }
        }

        /// <summary>
        /// Loads saved background images from local storage and starts the rotation timer.
        /// This method should be called during application startup.
        /// </summary>
        public async Task LoadBackgroundImages()
        {
            // Stop any existing timer before loading new images to prevent multiple timers running
            _threadpoolTimer?.Cancel();
            _threadpoolTimer = null;

            string imagesJson = await _fileUtilityService.ReadTextFromFileAsync("images.json");
            if (!string.IsNullOrEmpty(imagesJson))
            {
                try
                {
                    BackgroundImages = new ObservableCollection<PageBackgrounds>(JsonConvert.DeserializeObject<List<PageBackgrounds>>(imagesJson));
                    Debug.WriteLine($"ImageService: Loaded {BackgroundImages.Count} background images from file.");
                }
                catch (Exception es)
                {
                    await _loggingService.LogExceptionAsync(es); // Log the exception
                    BackgroundImages = new ObservableCollection<PageBackgrounds>(); // Ensure collection is initialized even on error
                    Debug.WriteLine("ImageService: Error loading images.json, initializing empty collection.");
                }
            }
            else
            {
                BackgroundImages = new ObservableCollection<PageBackgrounds>(); // Initialize empty if file not present
                Debug.WriteLine("ImageService: images.json not found, initializing empty collection.");
            }

            // Set the initial background image immediately after loading
            await SetNextBackgroundImage();

            // Start the periodic timer for image rotation if rotation time is set
            // Ensure settings are loaded before accessing _settingsService.AppSettings.DisplaySettings.ImageRotationTime
            if (_settingsService.AppSettings.DisplaySettings.ImageRotationTime.TotalSeconds > 0)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _threadpoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                    {
                        await SetNextBackgroundImage();
                    }, _settingsService.AppSettings.DisplaySettings.ImageRotationTime.Subtract(TimeSpan.FromSeconds(2))); // Subtract a bit for smooth transition
                    Debug.WriteLine($"ImageService: Started image rotation timer with interval: {_settingsService.AppSettings.DisplaySettings.ImageRotationTime}");
                });
            }
            else
            {
                Debug.WriteLine("ImageService: Image rotation time is zero, timer not started.");
            }

            ImagesRetrieved?.Invoke(this, EventArgs.Empty); // Notify subscribers that images have been loaded/updated
        }

        /// <summary>
        /// Saves the current order and data of background images to local storage.
        /// </summary>
        public async Task SaveImageOrder()
        {
            try
            {
                if (BackgroundImages.Any())
                {
                    string imageOrder = JsonConvert.SerializeObject(BackgroundImages.ToList(), Formatting.Indented);
                    await _fileUtilityService.WriteTextToFileAsync("images.json", imageOrder); // Use FileUtilityService
                    Debug.WriteLine($"ImageService: Saved {BackgroundImages.Count} background images to file.");
                }
                else // If no images, ensure the file is removed or empty
                {
                    await _fileUtilityService.DeleteFileAsync("images.json"); // Use FileUtilityService
                    Debug.WriteLine("ImageService: No background images to save, deleted images.json.");
                }
            }
            catch (Exception es)
            {
                await _loggingService.LogExceptionAsync(es); // Log the exception
            }
        }

        /// <summary>
        /// Converts a <see cref="StorageFile"/> representing an image into a byte array.
        /// </summary>
        /// <param name="fileName">The <see cref="StorageFile"/> to convert.</param>
        /// <returns>A byte array representing the image data, or null if an error occurs.</returns>
        public async Task<byte[]> ConvertImageFiletoByteArrayAsync(StorageFile fileName)
        {
            // Leverage the FileUtilityService for reading bytes from the file
            return await _fileUtilityService.ReadBytesFromFileAsync(fileName);
        }
    }
}