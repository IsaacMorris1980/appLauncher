using appLauncher.Core.Commands;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace appLauncher.Core.ViewModels
{
    public class FirstPageViewModel : ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly INavigationService _navigationService;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage = "Loading applications...";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    UpdateIndicatorSelection();
                    UpdatePaginationFlags();
                }
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    RebuildIndicators();
                    UpdatePaginationFlags();
                }
            }
        }

        // Integrated navigation page flags (replacing PageChangingVariables)
        private bool _isNext;
        public bool IsNext
        {
            get => _isNext;
            set => SetProperty(ref _isNext, value);
        }

        private bool _isPrevious;
        public bool IsPrevious
        {
            get => _isPrevious;
            set => SetProperty(ref _isPrevious, value);
        }

        public ObservableCollection<PageIndicatorModel> PageIndicators { get; } = new ObservableCollection<PageIndicatorModel>();

        public ICommand InitializeAppCommand { get; }
        public ICommand RescanAppsCommand { get; }
        public ICommand GoToPageCommand { get; }
        public ICommand NavigateAppsCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public FirstPageViewModel(IPackageService packageService, INavigationService navigationService)
        {
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            InitializeAppCommand = new RelayCommand(async () => await InitializeAsync());
            RescanAppsCommand = new RelayCommand(async () => await RescanAsync());

            GoToPageCommand = new RelayCommand<object>(indexObj =>
            {
                if (indexObj is int targetIndex)
                {
                    CurrentPage = targetIndex;
                }
            });

            NextPageCommand = new RelayCommand(() =>
            {
                if (IsNext) CurrentPage++;
            }, () => IsNext);

            PreviousPageCommand = new RelayCommand(() =>
            {
                if (IsPrevious) CurrentPage--;
            }, () => IsPrevious);

            NavigateAppsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(MainPage)));
            NavigateSettingsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(AppSettings)));
            NavigateBackCommand = new RelayCommand(() => { if (_navigationService.CanGoBack) _navigationService.GoBack(); });
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            StatusMessage = "Fetching installed applications...";

            try
            {
                await _packageService.LoadCollectionAsync();
                StatusMessage = "Ready!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading apps: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RescanAsync()
        {
            IsLoading = true;
            StatusMessage = "Rescanning applications...";

            try
            {
                await _packageService.LoadCollectionAsync();
                StatusMessage = "Rescan complete!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Rescan failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void UpdatePagination(int calculatedTotalPages, int targetPage)
        {
            TotalPages = calculatedTotalPages;
            CurrentPage = Math.Min(Math.Max(targetPage, 0), Math.Max(0, TotalPages - 1));
        }

        private void RebuildIndicators()
        {
            PageIndicators.Clear();
            for (int i = 0; i < TotalPages; i++)
            {
                PageIndicators.Add(new PageIndicatorModel(i, GoToPageCommand));
            }
            UpdateIndicatorSelection();
        }

        private void UpdateIndicatorSelection()
        {
            for (int i = 0; i < PageIndicators.Count; i++)
            {
                PageIndicators[i].IsSelected = (i == CurrentPage);
            }
        }

        private void UpdatePaginationFlags()
        {
            IsPrevious = CurrentPage > 0;
            IsNext = CurrentPage < TotalPages - 1;
        }
    }
}
