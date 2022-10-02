using appLauncher.Core.Helpers;
using appLauncher.Core.Pages;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Swordfish.NET.Collections.Auxiliary;

using System;
using System.Collections.Generic;
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
            App.Current.UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppCenter.Start("f3879d12-8020-4309-9fbf-71d9d24bcf9b",
                  typeof(Analytics), typeof(Crashes));

        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("UnhandedExceptionmessage", e.Exception.Message);
            result.Add("StackTrace", e.Exception.StackTrace);
            result.Add("TargetSite", e.Exception.TargetSite.Name);
            result.Add("ExceptionSource", e.Exception.Source);
            result.AddRange((IEnumerable<KeyValuePair<string, string>>)e.Exception.Data);

            Crashes.TrackError(e.Exception, result);
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Unhandedmessage", e.Message);
            result.Add("UnhandedExceptionmessage", e.Exception.Message);
            result.Add("StackTrace", e.Exception.StackTrace);
            result.Add("TargetSite", e.Exception.TargetSite.Name);
            result.Add("ExceptionSource", e.Exception.Source);
            result.AddRange((IEnumerable<KeyValuePair<string, string>>)e.Exception.Data);

            Crashes.TrackError(e.Exception, result);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Analytics.TrackEvent("Application has been launched");
            GlobalVariables.bgimagesavailable = (App.localSettings.Values["bgImageAvailable"] == null) ? false : true;
            //Extends view into status bar/title bar, depending on the device used.
            var appView = ApplicationView.GetForCurrentView();
            appView.SetPreferredMinSize(new Size(360, 360));
            appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;

            if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop")
            {
                appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appView.TitleBar.BackgroundColor = Colors.Transparent;
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }

            if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile")
            {
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            }


            Frame rootFrame = Window.Current.Content as Frame;




            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                //  rootFrame.Navigated += OnNavigated;


                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                //SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                //    rootFrame.CanGoBack ?
                //    AppViewBackButtonVisibility.Visible :
                //    AppViewBackButtonVisibility.Collapsed;

                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
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

        //private void OnBackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    Frame rootFrame = Window.Current.Content as Frame;

        //    if (rootFrame.CanGoBack)
        //    {
        //        e.Handled = true;
        //        rootFrame.GoBack();
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            var deferral = e.SuspendingOperation.GetDeferral();
            await packageHelper.SaveCollectionAsync();
            await ImageHelper.SaveImageOrder();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
