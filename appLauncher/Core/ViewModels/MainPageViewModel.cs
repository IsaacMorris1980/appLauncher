using appLauncher.Core.Commands;
using appLauncher.Core.DataTemplateSelectors;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;
using appLauncher.Core.ViewModels;

using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace appLauncher.Core.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly INavigationService _navigationService;
        private readonly IImageService _imageService;

        private ImageBrush _backgroundBrush;
        public ImageBrush BackgroundBrush
        {
            get => _backgroundBrush;
            set => SetProperty(ref _backgroundBrush, value);
        }

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    FilterAndSortApps();
                }
            }
        }

        private string _selectedSortOption = "NameAZ";
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    FilterAndSortApps();
                }
            }
        }

        private object _appsCollection;
        public object AppsCollection
        {
            get => _appsCollection ?? _packageService.Apps;
            set => SetProperty(ref _appsCollection, value);
        }

        public ICommand ItemClickCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand ViewInfoCommand { get; }
        public ICommand ChangeSortCommand { get; }

        public MainPageViewModel(IPackageService packageService, INavigationService navigationService, IImageService imageService)
        {
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

            ItemClickCommand = new RelayCommand<object>(async item => await OnItemClickedAsync(item));
            EditItemCommand = new RelayCommand<object>(item => OnEditItem(item));
            ViewInfoCommand = new RelayCommand<object>(item => OnViewInfo(item));
            ChangeSortCommand = new RelayCommand<string>(sortKey => SelectedSortOption = sortKey);

            _appsCollection = _packageService.Apps;
        }

        public void LoadBackground()
        {
            BackgroundBrush = _imageService.LoadBackgroundImageAsync() as ImageBrush;
        }

        private void FilterAndSortApps()
        {
            if (_packageService?.Apps == null) return;

            var allItems = _packageService.Apps.GetOriginalCollection();

            // 1. Search filter query
            var filtered = string.IsNullOrWhiteSpace(SearchQuery)
                ? allItems
                : allItems.Where(x => x.Name != null && x.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            // 2. Sorting options
            switch (SelectedSortOption)
            {
                case "NameAZ":
                    filtered = filtered.OrderBy(x => x.Name).ToList();
                    break;
                case "NameZA":
                    filtered = filtered.OrderByDescending(x => x.Name).ToList();
                    break;
                case "InstallNewest":
                    filtered = filtered.OrderByDescending(x => (x is FinalTiles t && t.Pack != null) ? t.Pack.InstalledDate : DateTimeOffset.MinValue).ToList();
                    break;
                case "InstallOldest":
                    filtered = filtered.OrderBy(x => (x is FinalTiles t && t.Pack != null) ? t.Pack.InstalledDate : DateTimeOffset.MinValue).ToList();
                    break;
            }

            AppsCollection = new AppPaginationObservableCollection(filtered);
        }

        private async Task OnItemClickedAsync(object clickedItem)
        {
            if (clickedItem is AppFolder folder)
            {
                _navigationService.NavigateTo(typeof(FolderView), folder);
            }
            else if (clickedItem is FinalTiles app)
            {
                app.LaunchedCount += 1;
                await _packageService.LaunchApp(app.FullName);
            }
        }

        private void OnEditItem(object item)
        {
            if (item != null)
            {
                _navigationService.NavigateTo(typeof(ItemDetailEditPage), item);
            }
        }

        private void OnViewInfo(object item)
        {
            if (item != null)
            {
                _navigationService.NavigateTo(typeof(ItemDetailEditPage), item);
            }
        }
    }
}

