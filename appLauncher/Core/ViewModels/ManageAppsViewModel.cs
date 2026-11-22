using appLauncher.Core.Commands;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace appLauncher.Core.ViewModels
{
    public class ManageAppsViewModel : ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly INavigationService _navigationService;

        private bool _isInstallMode = true;
        public bool IsInstallMode
        {
            get => _isInstallMode;
            set
            {
                if (SetProperty(ref _isInstallMode, value))
                {
                    OnPropertyChanged(nameof(ModeTitle));
                    OnPropertyChanged(nameof(ActionButtonText));
                    _ = LoadItemsAsync();
                }
            }
        }

        public string ModeTitle => IsInstallMode ? "Install Applications" : "Remove Applications";
        public string ActionButtonText => IsInstallMode ? "Start Installation Wizard" : "Remove Selected";

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ObservableCollection<object> AppItems { get; } = new ObservableCollection<object>();

        public ICommand ToggleModeCommand { get; }
        public ICommand ExecuteActionCommand { get; }

        public ManageAppsViewModel(IPackageService packageService, INavigationService navigationService)
        {
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            ToggleModeCommand = new RelayCommand<bool>(mode => IsInstallMode = mode);
            ExecuteActionCommand = new RelayCommand<object>(async selectedItem => await ExecuteActionAsync(selectedItem));
        }

        public async Task LoadItemsAsync()
        {
            IsLoading = true;
            AppItems.Clear();

            try
            {
                if (IsInstallMode)
                {
                    StatusMessage = "Ready to install new application package.";
                }
                else
                {
                    StatusMessage = "Loading installed applications...";
                    foreach (var app in _packageService.Apps.GetOriginalCollection())
                    {
                        AppItems.Add(app);
                    }
                    StatusMessage = "Ready";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteActionAsync(object item)
        {
            IsLoading = true;
            try
            {
                if (IsInstallMode)
                {
                    // Step 1: Install Certificate
                    StatusMessage = "Step 1: Selecting and installing certificate...";
                     await _packageService.InstallCertificateAsync();

                    // Step 2: Load/Install Dependencies
                    StatusMessage = "Step 2: Checking and installing dependencies...";
                    await _packageService.LoadDependenciesAsync();

                    // Step 3: Install Final Application Package
                    StatusMessage = "Step 3: Running application installer...";
                    await _packageService.InstallApplicationAsync();

                    StatusMessage = "Application pipeline completed successfully!";
                }
                else
                {
                    if (item == null) return;

                    // Assuming item can be cast or its FullName/Identifier accessed for removal
                    // If your item type is known (e.g., FinalTiles or a package identifier string), adjust accordingly:
                    StatusMessage = "Removing application...";

                    // If item is your app model, retrieve its identifier/fullname:
                    // string fullName = ...; 
                    await _packageService.RemoveApplicationAsync(((FinalTiles)item).FullName);

                    AppItems.Remove(item);
                    StatusMessage = "Application removed successfully!";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Action failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
