using appLauncher.Core.Interfaces;
using appLauncher.Core.Model; // For GlobalAppSettings, PageBackgrounds etc.
using appLauncher.Core.Pages;
// Import your new manual ServiceLocator
using appLauncher.Core.Services;
// Import your ViewModels and Models
using appLauncher.Core.ViewModels;

using System;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace appLauncher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Gets the Service Locator for dependency resolution.
        /// </summary>
        public static ServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            ConfigureServices(); // Call method to set up DI
        }

        /// <summary>
        /// Configures the services for dependency injection using the manual ServiceLocator.
        /// </summary>
        private void ConfigureServices()
        {
            ServiceLocator = new ServiceLocator();

            // Register services as singletons (or transients as appropriate)
            ServiceLocator.RegisterSingleton<ILoggingService, LoggingService>();
            ServiceLocator.RegisterSingleton<IFileService, FileService>();
            ServiceLocator.RegisterSingleton<ISettingsService, SettingsService>();
            ServiceLocator.RegisterSingleton<IImageService, ImageService>();
            ServiceLocator.RegisterSingleton<IPackageService, PackageService>();
            ServiceLocator.RegisterSingleton<IInstallationService, InstallationService>();
            ServiceLocator.RegisterSingleton<IUserService, UserService>();  

            // Register ViewModels as transient (new instance each time requested)
            ServiceLocator.RegisterTransient<MainViewModel, MainViewModel>();
            ServiceLocator.RegisterTransient<AboutViewModel, AboutViewModel>();
            ServiceLocator.RegisterTransient<AppBackgroundSettingsViewModel, AppBackgroundSettingsViewModel>();
            ServiceLocator.RegisterTransient<AppDetailViewModel, AppDetailViewModel>();
            ServiceLocator.RegisterTransient<AppLoadingViewModel, AppLoadingViewModel>();
            ServiceLocator.RegisterTransient<AppSettingsViewModel, AppSettingsViewModel>();
            ServiceLocator.RegisterTransient<AppTilesSettingsViewModel, AppTilesSettingsViewModel>();
            ServiceLocator.RegisterTransient<CreateFoldersViewModel, CreateFoldersViewModel>();
            ServiceLocator.RegisterTransient<FolderDetailViewModel, FolderDetailViewModel>();         
            ServiceLocator.RegisterTransient<InstallAppsViewModel, InstallAppsViewModel>();
            ServiceLocator.RegisterTransient<FolderNameViewModel, FolderNameViewModel>();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load Page {e.SourcePageType.FullName}: {e.Exception.Message}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended. Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // Save settings when suspending
            // Use the ServiceLocator to get the services
            var settingsService = ServiceLocator.Resolve<ISettingsService>();
            if (settingsService != null)
            {
                await settingsService.SaveAppSettingsAsync();
            }
            // Save package data when suspending
            var packageService = ServiceLocator.Resolve<IPackageService>();
            if (packageService != null)
            {
                await packageService.SaveAppCollectionAsync();
            }
            // Save image order when suspending
            var imageService = ServiceLocator.Resolve<IImageService>();
            if (imageService != null)
            {
                await imageService.SaveImageOrder();
            }

            deferral.Complete();
        }
    }
}
