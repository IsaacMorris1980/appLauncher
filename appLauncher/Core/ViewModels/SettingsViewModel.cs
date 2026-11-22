using appLauncher.Core.Commands;
using appLauncher.Core.Enums;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;

        public GlobalAppSettings AppSettings => _settingsService?.TotalAppSettings;

        private int _rotationTimeSeconds = 15;
        public int RotationTimeSeconds
        {
            get => _rotationTimeSeconds;
            set
            {
                if (SetProperty(ref _rotationTimeSeconds, value))
                {
                    if (AppSettings != null)
                    {
                        AppSettings.ImageRotationTime = TimeSpan.FromSeconds(Math.Max(value, 1));
                    }
                }
            }
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand NavigateBackCommand { get; }

        public SettingsViewModel(ISettingsService settingsService, INavigationService navigationService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            if (AppSettings?.ImageRotationTime != null)
            {
                _rotationTimeSeconds = (int)AppSettings.ImageRotationTime.TotalSeconds;
            }

            SaveSettingsCommand = new RelayCommand(async () => await SaveSettingsAsync());
            NavigateBackCommand = new RelayCommand(() =>
            {
                if (_navigationService.CanGoBack) _navigationService.GoBack();
            });
        }

        public async Task SaveSettingsAsync()
        {
            _settingsService.SetApplicationResources();
            await _settingsService.SaveAppSettingsAsync();
        }
    }
}
