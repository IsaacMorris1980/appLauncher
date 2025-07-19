using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media; // For Brush

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the InstallApps page, handling the installation process
    /// of certificates, dependencies, and the main application package.
    /// </summary>
    public class InstallAppsViewModel : ViewModelBase
    {
        private readonly IInstallationService _installationService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;

        // UI-bound properties
        private string _installInfoText = "Select the 'Install Application' button to begin.";
        /// <summary>
        /// Gets or sets the main informational text displayed to the user.
        /// </summary>
        public string InstallInfoText
        {
            get => _installInfoText;
            set => SetProperty(ref _installInfoText, value);
        }

        private string _errorOrSuccessText = string.Empty;
        /// <summary>
        /// Gets or sets the text displaying success messages or error details.
        /// </summary>
        public string ErrorOrSuccessText
        {
            get => _errorOrSuccessText;
            set => SetProperty(ref _errorOrSuccessText, value);
        }

        private bool _isCertInstalled;
        /// <summary>
        /// Gets or sets a value indicating whether the certificate installation step is complete.
        /// </summary>
        public bool IsCertInstalled
        {
            get => _isCertInstalled;
            set => SetProperty(ref _isCertInstalled, value);
        }

        private bool _areDepsInstalled;
        /// <summary>
        /// Gets or sets a value indicating whether the dependencies installation step is complete.
        /// </summary>
        public bool AreDepsInstalled
        {
            get => _areDepsInstalled;
            set => SetProperty(ref _areDepsInstalled, value);
        }

        private bool _isAppInstalled;
        /// <summary>
        /// Gets or sets a value indicating whether the main application installation step is complete.
        /// </summary>
        public bool IsAppInstalled
        {
            get => _isAppInstalled;
            set => SetProperty(ref _isAppInstalled, value);
        }

        private bool _isInstalling;
        /// <summary>
        /// Gets or sets a value indicating whether an installation process is currently active.
        /// Used to disable the install button during operation.
        /// </summary>
        public bool IsInstalling
        {
            get => _isInstalling;
            set
            {
                if (SetProperty(ref _isInstalling, value))
                {
                    InstallApplicationCommand.RaiseCanExecuteChanged(); // Update command state
                }
            }
        }

        // General app settings brushes for page background and foreground
        public Brush AppForegroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppForegroundColorBrush;
        public Brush AppBackgroundColorBrush => _settingsService.AppSettings.DisplaySettings.AppBackgroundColorBrush;

        // Commands
        public ViewModelBase.AsyncCommand InstallApplicationCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallAppsViewModel"/> class.
        /// </summary>
        /// <param name="installationService">The service responsible for application installation operations.</param>
        /// <param name="settingsService">The service for managing application settings (for UI theme).</param>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public InstallAppsViewModel(IInstallationService installationService, ISettingsService settingsService, ILoggingService loggingService)
        {
            _installationService = installationService;
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
            InstallApplicationCommand = new ViewModelBase.AsyncCommand(ExecuteInstallApplicationAsync, () => !IsInstalling);
        }

        /// <summary>
        /// Executes the full application installation process.
        /// </summary>
        private async Task ExecuteInstallApplicationAsync()
        {
            IsInstalling = true;
            ErrorOrSuccessText = string.Empty; // Clear previous messages
            IsCertInstalled = false;
            AreDepsInstalled = false;
            IsAppInstalled = false;

            try
            {
                InstallInfoText = "Installing certificate...";
                string certResult = await _installationService.InstallCertificateAsync();
                if (certResult == "Success") // Assuming "Success" is the success message from your service
                {
                    IsCertInstalled = true;
                }
                else
                {
                    ErrorOrSuccessText = $"Certificate installation failed: {certResult}";
                    InstallInfoText = "Retry installation?";
                    return; // Stop if certificate fails
                }

                InstallInfoText = "Installing dependencies...";
                string depsResult = await _installationService.LoadDependenciesAsync();
                if (depsResult == "Success")
                {
                    AreDepsInstalled = true;
                }
                else
                {
                    ErrorOrSuccessText = $"Dependency installation failed: {depsResult}";
                    InstallInfoText = "Retry installation?";
                    return; // Stop if dependencies fail
                }

                InstallInfoText = "Installing application...";
                string appResult = await _installationService.InstallApplicationAsync();
                if (appResult == "Success")
                {
                    IsAppInstalled = true;
                    ErrorOrSuccessText = "Application installed successfully!";
                    InstallInfoText = "Install another application?";
                }
                else
                {
                    ErrorOrSuccessText = $"Application installation failed: {appResult}";
                    InstallInfoText = "Retry installation?";
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                ErrorOrSuccessText = $"An unexpected error occurred: {ex.Message}";
                InstallInfoText = "An error occurred during installation. Please check logs.";
            }
            finally
            {
                IsInstalling = false;
                // Optionally, clear checkboxes after a short delay if successful
                if (IsCertInstalled && AreDepsInstalled && IsAppInstalled)
                {
                    await Task.Delay(1500);
                    IsCertInstalled = false;
                    AreDepsInstalled = false;
                    IsAppInstalled = false;
                }
            }
        }
    }
}
