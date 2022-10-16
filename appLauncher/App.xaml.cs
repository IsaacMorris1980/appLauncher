
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;




        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                await initialiseLocalSettings();
                Analytics.TrackEvent("App is loading");
                GlobalVariables.bgimagesavailable = localSettings.Values["bgImageAvailable"] != null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            //Extends view into status bar/title bar, depending on the device used.
            try
            {
                var appView = ApplicationView.GetForCurrentView();
                appView.SetPreferredMinSize(new Size(360, 360));
                appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;


                if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop")
                {
                    Analytics.TrackEvent("Device is a Desktop");
                    appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                    appView.TitleBar.BackgroundColor = Colors.Transparent;
                    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                }

                if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile")
                {
                    Analytics.TrackEvent("Device is a mobile");
                    appView.SuppressSystemOverlays = true;

                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;




                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();

                    rootFrame.NavigationFailed += OnNavigationFailed;
                    rootFrame.Navigated += OnNavigated;


                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;

                    SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        rootFrame.CanGoBack ?
                        AppViewBackButtonVisibility.Visible :
                        AppViewBackButtonVisibility.Collapsed;

                    if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                    {
                        bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                        Analytics.TrackEvent("Splashscreen is loading");
                        splashScreen extendedSplash = new splashScreen(e.SplashScreen, loadState, ref rootFrame);
                        rootFrame.Content = extendedSplash;
                        Window.Current.Content = rootFrame;
                    }
                }

                if (e.PrelaunchActivated == false)
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
            else
            {
                e.Handled = true;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Initialises local settings if the app has been started for the first time
        /// or new settings have been introduced from an update.
        /// </summary>
        private async Task initialiseLocalSettings()
        {
            try
            {
                AppCenter.Start("f3879d12-8020-4309-9fbf-71d9d24bcf9b",
                     typeof(Analytics), typeof(Crashes));
                AppCenter.LogLevel = LogLevel.Verbose;
                await AppCenter.SetEnabledAsync(true);
                Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
                await Crashes.SetEnabledAsync(true);
                await Analytics.SetEnabledAsync(true);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }



        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Crashes.TrackError(new Exception("Failed to load Page " + e.SourcePageType.FullName));
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            try
            {
                Analytics.TrackEvent("App Suspending");
                var deferral = e.SuspendingOperation.GetDeferral();
                Analytics.TrackEvent("Saving Background Images and App order list");
                GlobalVariables.SaveAppColors();
                await GlobalVariables.SaveCollectionAsync();
                await GlobalVariables.SaveImageOrder();
                //TODO: Save application state and stop any background activity
                deferral.Complete();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
