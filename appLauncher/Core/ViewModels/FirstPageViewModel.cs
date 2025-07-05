using appLauncher.Core.Commands; // Import your RelayCommand namespace
using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;


namespace appLauncher.Core.ViewModels
{
    public class FirstPageViewModel : ViewModelBase
    {
        // Manually implemented properties (replace [ObservableProperty] usage)
        private string _currentPageName = "FirstPage";
        public string CurrentPageName
        {
            get => _currentPageName;
            set
            {
                if (SetProperty(ref _currentPageName, value))
                {
                    // Logic from OnCurrentPageNameChanged partial method
                    IsSearchingVisible = value == "mainpage";
                    Debug.WriteLine($"Navigating to {value} page");
                }
            }
        }

        private bool _isSearchingVisible;
        public bool IsSearchingVisible
        {
            get => _isSearchingVisible;
            set => SetProperty(ref _isSearchingVisible, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _currentTimeLeft = _updateTimer;
                    _searchDelay.Start();
                }
            }
        }

        private int _currentPage = 0;
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private int _numOfPages = 0;
        public int NumOfPages
        {
            get => _numOfPages;
            set => SetProperty(ref _numOfPages, value);
        }

        // ObservableCollection for page indicators
        public ObservableCollection<PageIndicatorViewModel> PageIndicators { get; } = new ObservableCollection<PageIndicatorViewModel>();
        public ObservableCollection<NavFontIcon> NavMenuItems { get; } = new ObservableCollection<NavFontIcon>();


        // Commands using the custom RelayCommand implementation
        public ICommand NavigateCommand { get; }
        public ICommand SearchTextChangedCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PageIndicatorSelectedCommand { get; }

        private DispatcherTimer _searchDelay;
        private string _previousSearchText = string.Empty;
        private int _currentTimeLeft = 0;
        private readonly int _updateTimer = 1500;

        // Event for the View to subscribe to for navigation
        public event EventHandler<NavigationRequestedEventArgs> NavigationRequested;
        public event EventHandler<NotificationRequestedEventArgs> NotificationRequested;


        public FirstPageViewModel()
        {
            // Initialize commands
            NavigateCommand = new RelayCommand<NavFontIcon>(OnNavigate);
            SearchTextChangedCommand = new RelayCommand<string>(OnSearchTextChanged);
            PreviousPageCommand = new RelayCommand(OnPreviousPage);
            NextPageCommand = new RelayCommand(OnNextPage);
            PageIndicatorSelectedCommand = new RelayCommand<object>(OnPageIndicatorSelected);

            // Initialize DispatcherTimer for search delay
            _searchDelay = new DispatcherTimer();
            _searchDelay.Tick += SearchDelay_Tick;
            _searchDelay.Interval = TimeSpan.FromMilliseconds(100);

            // Subscribe to events from Helper classes (if they are global/static)
            MainPage.numofPagesChanged += SetupPageIndicators;
            PackageHelper.pageVariables = new PageChangingVariables();

            // Populate initial navigation icons
            SetupMainPageIcons();
        }

        private void OnSearchTextChanged(string newText)
        {
            SearchText = newText; // Property setter will handle timer logic
        }

        private void SearchDelay_Tick(object sender, object e)
        {
            if (_currentTimeLeft == 0)
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    PackageHelper.Apps.Search(SearchText);
                    _searchDelay.Stop();
                    return;
                }
                if (SearchText.Equals(_previousSearchText))
                {
                    PackageHelper.Apps.Search(SearchText);
                    _searchDelay.Stop();
                }
            }
            else
            {
                _currentTimeLeft -= (int)_searchDelay.Interval.TotalMilliseconds;
                _previousSearchText = SearchText;
            }
        }

        private void OnNavigate(NavFontIcon icon)
        {
            Debug.WriteLine($"Navigating based on Tag: {icon.Tag}");

            switch (icon.Tag.ToString())
            {
                case "previouspage":
                    // Instruct the View to go back
                    NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs("Going back to previous page", 1000));
                    NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.GoBack));
                    break;
                case "apps":
                    NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs("Navigating to Main Page", 1000));
                    NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(typeof(MainPage)));
                    break;
                case "filterapps":
                    SetupFilterPageIcons();
                    break;
                case "appnameaz":
                    PackageHelper.Apps.GetFilteredApps("alphaAZ");
                    break;
                case "appnameza":
                    PackageHelper.Apps.GetFilteredApps("alphaZA");
                    break;
                case "devnameaz":
                    PackageHelper.Apps.GetFilteredApps("devAZ");
                    break;
                case "devnameza":
                    PackageHelper.Apps.GetFilteredApps("devZA");
                    break;
                case "installnewest":
                    PackageHelper.Apps.GetFilteredApps("installnewest");
                    break;
                case "installoldest":
                    PackageHelper.Apps.GetFilteredApps("installoldest");
                    break;
                case "favorites":
                    PackageHelper.Apps.SetCollection(AnyFavorites());
                    break;
                case "mostused":
                    PackageHelper.Apps.SetCollection(AnyMostUsed());
                    break;
                case "backtoFilterOption":
                    SetupMainPageIcons(); // Go back to main navigation
                    break;
                case "edit":
                    NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs("Editing the app or folder", 1000));
                    NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(icon.NavLocation, icon.AppOrFolder));
                    break;
                case "rescan":
                    NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs("Rescanning for new applications", 1000));
                    // Await in ViewModel requires async Task method
                    // For SDK 15063, you might need to handle async operations carefully
                    // or let the View handle the await if it's UI-bound.
                    // For simplicity, calling the method directly here.
                    _ = PackageHelper.RescanForNewApplications();
                    break;
                case "about":
                    NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs("Navigating to About Page", 1000));
                    NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(icon.NavLocation));
                    break;
                case "install":
                    SetupInstalorRemoveIcons();
                    break;
                case "launch":
                    if (icon.AppOrFolder is FinalTiles app)
                    {
                        // Launch app logic
                        NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs($"Launching {app.Name}", 1000));
                        PackageHelper.Apps.LaunchApp(app);
                    }
                    if (icon.AppOrFolder is AppFolder appFolder)
                    {
                        NotificationRequested?.Invoke(this, new NotificationRequestedEventArgs($"Opening folder: {appFolder.Name}", 1000));
                        NavigationRequested?.Invoke(this,new NavigationRequestedEventArgs(icon.NavLocation,icon.AppOrFolder));   
                    }
                        break;
                default:
                    break;
            }
        }

        private void OnPreviousPage()
        {
            if (PackageHelper.pageVariables.IsPrevious)
            {
                // Instruct the View to navigate
                NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.ChangePage, CurrentPage - 1));
            }
        }

        private void OnNextPage()
        {
            if (PackageHelper.pageVariables.IsNext)
            {
                // Instruct the View to navigate
                NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.ChangePage, CurrentPage + 1));
            }
        }

        private void OnPageIndicatorSelected(object selectedTag)
        {
            if (selectedTag is int pageIndex)
            {
                SettingsHelper.totalAppSettings.LastPageNumber = pageIndex;
                // Instruct the View to navigate
                NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(NavigationType.ChangePage, pageIndex));
                UpdateIndicator(new PageChangedEventArgs(pageIndex));
            }
        }

        private void SetupMainPageIcons()
        {
            NavMenuItems.Clear();
            NavMenuItems.Add(new NavFontIcon() { Tag = "previouspage", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "previous page", Glyph = "\uE72B", Tip = "Back to previous page" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "apps", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "Apps", Glyph = "\uE71D", NavLocation = typeof(MainPage), Tip = "Return to applications" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "filterapps", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "Filter Apps", Glyph = "\uE71C", NavLocation = typeof(FilteringPage), Tip = "Filter applications" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "install", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "Install/Remove", Glyph = "\uE77D", Tip = "Install or remove applications" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "rescan", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "Rescan Apps", Glyph = "\uE72C", Tip = "Rescan the computer for new applications" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "about", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "About", Glyph = "\uE946", NavLocation = typeof(AboutPage), Tip = "About this application" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "settings", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "Settings", Glyph = "\uE713", NavLocation = typeof(SettingsPage), Tip = "Application Settings" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "help", FontFamily = new FontFamily("Segoe MDL2 Assets"), Name = "Help", Glyph = "\uE897", NavLocation = typeof(HelpPage), Tip = "Get Help with this application" });
        }

        private void SetupFilterPageIcons()
        {
            NavMenuItems.Clear();
            NavMenuItems.Add(new NavFontIcon() { Tag = "backtoFilterOption", FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\uE72B", DisplayName = "Back to main nav" });
            NavMenuItems.Add(new NavFontIcon() { Tag = "appname", FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\uE80F", DisplayName = "App Name", NavLocation = typeof(SortingPage) });
            NavMenuItems.Add(new NavFontIcon() { Tag = "developername", FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\uE7EE", DisplayName = "Developer Name", NavLocation = typeof(SortingPage) });
            NavMenuItems.Add(new NavFontIcon() { Tag = "installeddate", FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\uED35", DisplayName = "Install Date", NavLocation = typeof(SortingPage) });
            NavMenuItems.Add(new NavFontIcon() { Tag = "appsize", FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\uE825", DisplayName = "App Size", NavLocation = typeof(SortingPage) });
        }

        private void SetupInstalorRemoveIcons()
        {
            NavMenuItems.Clear();
            NavFontIcon back = new NavFontIcon() { Tag = "backtoFilterOption", FontFamily = new FontFamily("Segoe MDL2 Assets"), Glyph = "\uE72B" };
            NavFontIcon installApps = new NavFontIcon() { AppOrFolder = null, FontFamily = new FontFamily("Segoe MDL2 Assets"), NavLocation = null, Glyph = "\uE716", Tag = "installapp" };
            NavFontIcon uninstallApps = new NavFontIcon() { AppOrFolder = null, FontFamily = new FontFamily("Segoe MDL2 Assets"), NavLocation = null, Glyph = "\uE77F", Tag = "uninstall" };
            NavMenuItems.Add(back);
            NavMenuItems.Add(installApps);
            NavMenuItems.Add(uninstallApps);
        }

        private void SetupPageIndicators(PageNumChangedArgs e)
        {
            NumOfPages = e.numofpages;
            PageIndicators.Clear();
            for (int i = 0; i < e.numofpages; i++)
            {
                PageIndicators.Add(new PageIndicatorViewModel
                {
                    PageNum = i,
                    Selected = (i == SettingsHelper.totalAppSettings.LastPageNumber),
                    ToolTip = $"Page {i + 1}"
                });
            }
            UpdateIndicator(new PageChangedEventArgs(SettingsHelper.totalAppSettings.LastPageNumber));
        }

        private void UpdateIndicator(PageChangedEventArgs e)
        {
            CurrentPage = e.PageIndex;
            PackageHelper.pageVariables.IsPrevious = e.PageIndex > 0;
            PackageHelper.pageVariables.IsNext = e.PageIndex < NumOfPages - 1;

            foreach (var item in PageIndicators)
            {
                item.Selected = (item.PageNum == e.PageIndex);
            }
            PackageHelper.Apps.PageChanged(new PageChangedEventArgs(e.PageIndex));
        }

        private AppFolder AnyFavorites()
        {
            AppFolder folder = new AppFolder()
            {
                Name = "Favorites",
                Description = "Apps that are marked as favorite",
                ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                InstalledDate = DateTime.Now
            };

            var apps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            foreach (var item in folders)
            {
                apps.AddRange(item.FolderApps.ToList());
            }
            folder.FolderApps = apps.Where(x => x.Favorite == true).ToList();
            return folder;
        }

        private AppFolder AnyMostUsed()
        {
            AppFolder folder = new AppFolder()
            {
                Name = "Most Used",
                Description = "Apps that are launched more than 5 times using this app",
                ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                InstalledDate = DateTime.Now
            };
            var apps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            foreach (var item in folders)
            {
                apps.AddRange(item.FolderApps.ToList());
            }
            folder.FolderApps = apps.Where(x => x.LaunchedCount > 5).ToList();
            return folder;
        }

       

        // Custom EventArgs for Navigation
        public enum NavigationType { NavigateToPage, GoBack, ChangePage }

        public class NavigationRequestedEventArgs : EventArgs
        {
            public NavigationType Type { get; }
            public Type TargetPageType { get; }
            public object Parameter { get; }
            public int PageIndex { get; } // For page changing logic

            public NavigationRequestedEventArgs(Type targetPageType, object parameter = null)
            {
                Type = NavigationType.NavigateToPage;
                TargetPageType = targetPageType;
                Parameter = parameter;
            }
            public NavigationRequestedEventArgs(NavigationType type)
            {
                Type = type;
            }
            public NavigationRequestedEventArgs(NavigationType type, int pageIndex)
            {
                Type = type;
                PageIndex = pageIndex;
            }
        }

        // Custom EventArgs for Notifications
        public class NotificationRequestedEventArgs : EventArgs
        {
            public string Message { get; }
            public int Duration { get; }

            public NotificationRequestedEventArgs(string message, int duration)
            {
                Message = message;
                Duration = duration;
            }
        }
    }
    public class PageIndicatorViewModel : ViewModelBase
    {
        private int _pageNum;
        public int PageNum
        {
            get => _pageNum;
            set => SetProperty(ref _pageNum, value);
        }

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                if (SetProperty(ref _selected, value))
                {
                    OnPropertyChanged(nameof(IndicatorBrush)); // Notify UI when Selected changes
                }
            }
        }

        private string _toolTip;
        public string ToolTip
        {
            get => _toolTip;
            set => SetProperty(ref _toolTip, value);
        }

        public SolidColorBrush IndicatorBrush => Selected ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Gray);
    }
}