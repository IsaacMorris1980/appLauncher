using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the main application page, handling app display, settings, and image rotation.
    /// It orchestrates interactions between various services and provides data/commands for the UI.
    /// </summary>
    public class MainViewModel : ViewModelBase // Inherit from ViewModelBase for SetProperty and INotifyPropertyChanged
    {
        private readonly IPackageService _packageService;
        private readonly IImageService _imageService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;

        // Properties for UI Binding
        private PaginationObservableCollection<IApporFolder> _apps;
        /// <summary>
        /// Gets or sets the observable collection of apps for the main display grid.
        /// This collection handles pagination.
        /// </summary>
        public PaginationObservableCollection<IApporFolder> Apps
        {
            get => _apps;
            set => SetProperty(ref _apps, value);
        }

        private ObservableCollection<IApporFolder> _searchResults;
        /// <summary>
        /// Gets or sets the collection of search results for the AutoSuggestBox.
        /// </summary>
        public ObservableCollection<IApporFolder> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        /// <summary>
        /// Gets the current background brush from the ImageService.
        /// This property is updated when the ImageService notifies of a change.
        /// </summary>
        public Brush CurrentBackgroundBrush => _imageService.CurrentBackgroundBrush;

        private bool _isPreviousPageEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the "Previous Page" navigation button should be enabled.
        /// </summary>
        public bool IsPreviousPageEnabled
        {
            get => _isPreviousPageEnabled;
            set
            {
                if (SetProperty(ref _isPreviousPageEnabled, value))
                {
                    NavigatePreviousPageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isNextPageEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the "Next Page" navigation button should be enabled.
        /// </summary>
        public bool IsNextPageEnabled
        {
            get => _isNextPageEnabled;
            set
            {
                if (SetProperty(ref _isNextPageEnabled, value))
                {
                    NavigateNextPageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private int _currentPageNum;
        /// <summary>
        /// Gets or sets the current page number being displayed.
        /// </summary>
        public int CurrentPageNum
        {
            get => _currentPageNum;
            set => SetProperty(ref _currentPageNum, value);
        }

        private int _numOfPages;
        /// <summary>
        /// Gets or sets the total number of pages available.
        /// </summary>
        public int NumOfPages
        {
            get => _numOfPages;
            set => SetProperty(ref _numOfPages, value);
        }

        private int _appsPerScreen;
        /// <summary>
        /// Gets or sets the number of apps that can fit on one screen/page.
        /// </summary>
        public int AppsPerScreen
        {
            get => _appsPerScreen;
            set => SetProperty(ref _appsPerScreen, value);
        }

        private int _columns;
        /// <summary>
        /// Gets or sets the calculated number of columns for the app grid.
        /// </summary>
        public int Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        private int _rows;
        /// <summary>
        /// Gets or sets the calculated number of rows for the app grid.
        /// </summary>
        public int Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;

        // Commands
        public ViewModelBase.AsyncCommand LoadDataCommand { get; private set; }
        public ViewModelBase.AsyncCommand RescanAppsCommand { get; private set; }
        public ViewModelBase.Command<string> SearchCommand { get; private set; }
        public ViewModelBase.AsyncCommand<string> LaunchAppCommand { get; private set; }
        public ViewModelBase.Command NavigatePreviousPageCommand { get; private set; }
        public ViewModelBase.Command NavigateNextPageCommand { get; private set; }
        public ViewModelBase.Command<string> SortAppsCommand { get; private set; }
        public ViewModelBase.AsyncCommand<Tuple<int, int>> MoveAppCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="packageService">The service for managing application packages.</param>
        /// <param name="imageService">The service for managing background images.</param>
        /// <param name="settingsService">The service for managing application settings.</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public MainViewModel(IPackageService packageService, IImageService imageService, ISettingsService settingsService, ILoggingService loggingService)
        {
            _packageService = packageService;
            _imageService = imageService;
            _settingsService = settingsService;
            _loggingService = loggingService;

            InitializeCommands();
            SubscribeToServiceEvents();
        }

        /// <summary>
        /// Initializes the commands used by the ViewModel.
        /// </summary>
        private void InitializeCommands()
        {
            LoadDataCommand = new ViewModelBase.AsyncCommand(LoadInitialData);
            RescanAppsCommand = new ViewModelBase.AsyncCommand(RescanForNewApplications);
            SearchCommand = new ViewModelBase.Command<string>(PerformSearch);
            LaunchAppCommand = new ViewModelBase.AsyncCommand<string>(LaunchApplication);
            NavigatePreviousPageCommand = new ViewModelBase.Command(NavigateToPreviousPage, () => IsPreviousPageEnabled);
            NavigateNextPageCommand = new ViewModelBase.Command(NavigateToNextPage, () => IsNextPageEnabled);
            SortAppsCommand = new ViewModelBase.Command<string>(SortApps);
            MoveAppCommand = new ViewModelBase.AsyncCommand<Tuple<int, int>>(MoveApp);
        }

        /// <summary>
        /// Subscribes to events from the services to update ViewModel properties.
        /// </summary>
        private void SubscribeToServiceEvents()
        {
            // Update background brush when ImageService notifies of changes
            _imageService.ImagesRetrieved += (s, e) => OnPropertyChanged(nameof(CurrentBackgroundBrush));

            // Update app collection when PackageService notifies of changes
            _packageService.AppsRetrieved += (s, e) =>
            {
                Apps = _packageService.Apps; // Assign the updated collection
                UpdateTotalPages(); // Recalculate pages based on new app count
                UpdatePageNavigationStates(); // Recalculate page states after app list changes
            };

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
        /// Loads initial data for the application, including settings, images, and app collection.
        /// </summary>
        private async Task LoadInitialData()
        {
            try
            {
                await _settingsService.LoadAppSettingsAsync(); // Load all settings first
                _settingsService.SetApplicationResources(); // Apply theme based on loaded settings

                // Initialize ViewModel properties from loaded settings
                AppsPerScreen = _settingsService.AppSettings.PageSettings.AppsPerPage;
                CurrentPageNum = _settingsService.AppSettings.PageSettings.LastPageNumber;
                NumOfPages = _settingsService.AppSettings.PageSettings.NumOfPages;

                await _imageService.LoadBackgroundImages(); // Load background images
                OnPropertyChanged(nameof(CurrentBackgroundBrush)); // Trigger UI update for background

                await _packageService.LoadAppCollectionAsync(); // Load app collection
                Apps = _packageService.Apps; // Assign the loaded collection
                UpdateTotalPages(); // Recalculate pages based on initial app count
                UpdatePageNavigationStates(); // Update navigation buttons based on app count
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                // Potentially show a user-friendly error message via InAppNotification
            }
        }

        /// <summary>
        /// Recalculates the number of columns and rows based on actual window size.
        /// Then updates apps per screen and total number of pages.
        /// </summary>
        /// <param name="actualWidth">The actual width of the GridView.</param>
        /// <param name="actualHeight">The actual height of the GridView.</param>
        public void RecalculateLayout(double actualWidth, double actualHeight)
        {
            try
            {
                // Assuming fixed item sizes for calculation as in original code
                const int objectWidth = 94; // Example value from original code
                const int objectHeight = 108; // Example value from original code

                Columns = NumofRoworColumn(objectWidth, (int)actualWidth);
                Rows = NumofRoworColumn(objectHeight, (int)actualHeight);

                int newAppsPerScreen = Columns * Rows;
                if (newAppsPerScreen <= 0) newAppsPerScreen = 1; // Ensure at least 1 app per screen to prevent division by zero

                if (newAppsPerScreen != AppsPerScreen)
                {
                    AppsPerScreen = newAppsPerScreen;
                    _settingsService.AppSettings.PageSettings.AppsPerPage = AppsPerScreen;
                    UpdateTotalPages(); // Recalculate total pages based on new AppsPerScreen
                }
                UpdatePageNavigationStates();
                _settingsService.SaveAppSettingsAsync().ConfigureAwait(false); // Save updated page settings
            }
            catch (Exception ex)
            {
                _loggingService.LogExceptionAsync(ex).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Helper to calculate number of rows or columns that fit.
        /// </summary>
        private int NumofRoworColumn(int objectSize, int sizeToFit)
        {
            int amount = 0;
            int size = objectSize;
            while (size <= sizeToFit)
            {
                amount += 1;
                size += objectSize;
            }
            return amount;
        }

        /// <summary>
        /// Updates the total number of pages based on the current Apps collection and AppsPerScreen.
        /// </summary>
        private void UpdateTotalPages()
        {
            if (Apps == null || AppsPerScreen <= 0)
            {
                NumOfPages = 1;
                _settingsService.AppSettings.PageSettings.NumOfPages = 1;
                return;
            }

            double numberOfApps = Apps.GetOriginalCollection().Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / AppsPerScreen);
            NumOfPages = Math.Max(1, pagesToMake); // Ensure at least 1 page
            _settingsService.AppSettings.PageSettings.NumOfPages = NumOfPages;

            // Adjust CurrentPageNum if it's out of bounds after recalculation
            if (CurrentPageNum >= NumOfPages)
            {
                CurrentPageNum = NumOfPages - 1;
                if (CurrentPageNum < 0) CurrentPageNum = 0; // Ensure not negative
                _settingsService.AppSettings.PageSettings.LastPageNumber = CurrentPageNum;
            }
            Apps.SetCurrentPage(CurrentPageNum); // Update the observable collection's page
        }

        /// <summary>
        /// Updates the enabled/disabled state of previous and next page navigation buttons.
        /// </summary>
        private void UpdatePageNavigationStates()
        {
            IsPreviousPageEnabled = CurrentPageNum > 0;
            IsNextPageEnabled = CurrentPageNum < NumOfPages - 1;

            // Re-evaluate can execute for commands
            NavigatePreviousPageCommand.RaiseCanExecuteChanged();
            NavigateNextPageCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Navigates to the previous page of applications.
        /// </summary>
        private void NavigateToPreviousPage()
        {
            if (IsPreviousPageEnabled)
            {
                CurrentPageNum--;
                _settingsService.AppSettings.PageSettings.LastPageNumber = CurrentPageNum;
                Apps.SetCurrentPage(CurrentPageNum);
                UpdatePageNavigationStates();
                _settingsService.SaveAppSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Navigates to the next page of applications.
        /// </summary>
        private void NavigateToNextPage()
        {
            if (IsNextPageEnabled)
            {
                CurrentPageNum++;
                _settingsService.AppSettings.PageSettings.LastPageNumber = CurrentPageNum;
                Apps.SetCurrentPage(CurrentPageNum);
                UpdatePageNavigationStates();
                _settingsService.SaveAppSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Performs a search on the application collection.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        private void PerformSearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SearchResults = null; // Clear search results if search text is empty
            }
            else
            {
                // Ensure _packageService.Search is not null before querying
                if (_packageService.Search != null)
                {
                    SearchResults = new ObservableCollection<IApporFolder>(
                        _packageService.Search
                            .Where(x => x.Name.ToLower().Contains(searchText.ToLower()))
                            .ToList()
                    );
                }
                else
                {
                    SearchResults = new ObservableCollection<IApporFolder>();
                }
            }
        }

        /// <summary>
        /// Launches the selected application.
        /// </summary>
        /// <param name="fullName">The full package name of the app to launch.</param>
        private async Task LaunchApplication(string fullName)
        {
            try
            {
                bool launched = await _packageService.LaunchApplication(fullName);
                if (launched)
                {
                    // Optionally update launched count or other metrics
                    var app = Apps.GetOriginalCollection().OfType<FinalTiles>().FirstOrDefault(x => x.FullName == fullName);
                    if (app != null)
                    {
                        app.LaunchedCount++; // Assuming LaunchedCount is on FinalTiles
                        await _packageService.SaveAppCollectionAsync(); // Save updated count
                    }
                }
                else
                {
                    // Handle launch failure (e.g., show notification)
                    await _loggingService.LogExceptionAsync(new Exception($"Failed to launch application: {fullName}"));
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Rescans for new or uninstalled applications and updates the collection.
        /// </summary>
        private async Task RescanForNewApplications()
        {
            try
            {
                await _packageService.RescanForNewApplications();
                await _packageService.SaveAppCollectionAsync(); // Save changes after rescan
                UpdateTotalPages(); // Recalculate pages after rescan
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Sorts the application list based on the specified sort type.
        /// </summary>
        /// <param name="sortType">The type of sort to apply (e.g., "AppAZ", "DevZA").</param>
        private void SortApps(string sortType)
        {
            if (Apps != null)
            {
                Apps.SortOriginalCollection(sortType); // Assuming this method re-sorts and updates the view
                UpdateTotalPages(); // Recalculate pages after sort, as order might affect last page
                UpdatePageNavigationStates(); // Re-evaluate page states after sort
            }
        }

        /// <summary>
        /// Handles the drag-and-drop movement of an item within the app collection.
        /// </summary>
        /// <param name="indices">A tuple containing the initial index and the new target index.</param>
        public async Task MoveApp(Tuple<int, int> indices)
        {
            try
            {
                if (Apps != null && indices != null)
                {
                    Apps.MoveApp(indices.Item1, indices.Item2); // Assuming MoveApp exists on PaginationObservableCollection
                    await _packageService.SaveAppCollectionAsync(); // Save the new order
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
            }
        }
    }
}
