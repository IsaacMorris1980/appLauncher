using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using GoogleAnalyticsv4SDK.Interfaces;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Application = Windows.UI.Xaml.Application;

namespace appLauncher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public List<IEvent> reportEvents;
        public GoogleAnalyticsv4SDK.Helpers.GoogleAnalyticsEndpoints reportCrashandAnalytics;
        public BackgroundWorker UDPServer = new BackgroundWorker();
        public BackgroundWorker TCPServer = new BackgroundWorker();
        public BackgroundWorker UDPClient = new BackgroundWorker();
        public BackgroundWorker TCPClient = new BackgroundWorker();
        private bool isnetworkstatuschangedregistered = false;
        private NetworkStatusChangedEventHandler networkstatuschangedhandler;
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

        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //    Dictionary<string, string> result = new Dictionary<string, string>();
            //    result.Add("UnhandedExceptionmessage", e.Exception.Message);
            //    result.Add("StackTrace", e.Exception.StackTrace);
            //    result.Add("TargetSite", e.Exception.TargetSite.Name);
            //    result.Add("ExceptionSource", e.Exception.Source);
            //    result.AddRange((IEnumerable<KeyValuePair<string, string>>)e.Exception.Data);

        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            //Dictionary<string, string> result = new Dictionary<string, string>();
            //result.Add("Unhandedmessage", e.Message);
            //result.Add("UnhandedExceptionmessage", e.Exception.Message);
            //result.Add("StackTrace", e.Exception.StackTrace);
            //result.Add("TargetSite", e.Exception.TargetSite.Name);
            //result.Add("ExceptionSource", e.Exception.Source);
            //result.AddRange((IEnumerable<KeyValuePair<string, string>>)e.Exception.Data);

        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

            try
            {
                bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

                UDPServer.WorkerSupportsCancellation = true;
                UDPServer.DoWork += UDPServer_DoWork;
                UDPClient.WorkerSupportsCancellation = true;
                UDPClient.DoWork += UDPClient_DoWork;
                TCPServer.WorkerSupportsCancellation = true;
                TCPServer.DoWork += TCPServer_DoWork;
                TCPClient.WorkerSupportsCancellation = true;
                TCPClient.DoWork += TCPClient_DoWork;

                //Extends view into status bar/title bar, depending on the device used.
                await SettingsHelper.LoadAppSettingsAsync();
                ApplicationView appView = ApplicationView.GetForCurrentView();
                appView.SetPreferredMinSize(new Size(360, 360));
                appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                IObservableMap<string, string> qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                SettingsHelper.SetApplicationResources();
                OnNetworkStatusChange(new object());
                NetworkStatusChange();


                Frame rootFrame = Window.Current.Content as Frame;



                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
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
                    if (canEnablePrelaunch)
                    {
                        TryEnablePrelaunch();
                    }
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
            catch (Exception es)
            {

            }
        }
        private void TCPClient_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //           throw new NotImplementedException();
        }

        private void TCPClient_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SettingsHelper.totalAppSettings.RemoteIP != null)
            {
                TcpClient client = new TcpClient();
                client.ConnectAsync(SettingsHelper.totalAppSettings.RemoteIP.Address, 13000);
                using (var stream = client.GetStream())
                {
                    string content = JsonConvert.SerializeObject(SettingsHelper.totalAppSettings);
                    byte[] sendbody = Encoding.UTF8.GetBytes(content);
                    stream.WriteAsync(sendbody, 0, sendbody.Length);
                }
            }
        }

        private void TCPServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void TCPServer_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 13000);
            Task.Factory.StartNew(async () =>
            {
                listener.Start();
                while (true)
                {

                    TcpClient a = await listener.AcceptTcpClientAsync();
                    NetworkStream b = a.GetStream();
                    byte[] message = new byte[1024];
                    string test = string.Empty;
                    using (var stream = a.GetStream())
                    {
                        int ab = b.Read(message, 0, message.Length);
                        if (ab == 0)
                        {
                            return;
                        }
                        test += Encoding.UTF8.GetString(message, 0, ab);
                        SettingsHelper.totalAppSettings = JsonConvert.DeserializeObject<GlobalAppSettings>(test);
                    }
                    listener.Stop();
                }
            });
        }

        private void UDPClient_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void UDPClient_DoWork(object sender, DoWorkEventArgs e)
        {
            UdpClient client = new UdpClient();
            await Task.Factory.StartNew(async () =>
               {
                   while (true)
                   {
                       IPEndPoint ipep = new IPEndPoint(IPAddress.Broadcast, 32123);
                       IReadOnlyList<HostName> hosts = NetworkInformation.GetHostNames();
                       byte[] data = Encoding.UTF8.GetBytes(hosts.ToString());
                       var a = await client.SendAsync(data, data.Length, ipep);
                       if (a > -1)
                       {
                           break;
                       }
                   }
               });

        }

        private void UDPServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void UDPServer_DoWork(object sender, DoWorkEventArgs e)
        {
            UdpClient server = new UdpClient();

            //start listening for messages and copy the messages back to the client
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 32123);
                    UdpReceiveResult data = await server.ReceiveAsync();
                    var hosts = NetworkInformation.GetHostNames();
                    var es = Encoding.UTF8.GetString(data.Buffer);
                    if (es != hosts.ToString())
                    {
                        SettingsHelper.totalAppSettings.RemoteIP = data.RemoteEndPoint;
                        Console.WriteLine(data.RemoteEndPoint.Address);
                        Console.WriteLine(es);
                        break;
                    }


                }
            });
            UDPServer.Dispose();
        }
        private void OnNetworkStatusChange(object sender)
        {
            SettingsHelper.totalAppSettings.Sync = false;
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
            {


                if (!profile.IsWlanConnectionProfile)
                {

                    //                    SettingsHelper.totalAppSettings.Sync = false;
                    return;
                }
                //              SettingsHelper.totalAppSettings.Sync = true;
                if (!((App)Current).UDPServer.IsBusy)
                {
                    ((App)Current).UDPServer.RunWorkerAsync();
                }
                if (!((App)Current).UDPClient.IsBusy)
                {
                    ((App)Current).UDPClient.RunWorkerAsync();
                }
                if (!((App)Current).TCPServer.IsBusy)
                {
                    ((App)Current).TCPServer.RunWorkerAsync();
                }

            }
        }

        private void NetworkStatusChange()
        {
            networkstatuschangedhandler = new NetworkStatusChangedEventHandler(OnNetworkStatusChange);
            if (isnetworkstatuschangedregistered == false)
            {
                NetworkInformation.NetworkStatusChanged += networkstatuschangedhandler;
                isnetworkstatuschangedregistered = true;
            }

        }
        private void TryEnablePrelaunch()
        {
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
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
            try
            {
                await PackageHelper.SaveCollectionAsync();
                await ImageHelper.SaveImageOrder();
                await SettingsHelper.SaveAppSettingsAsync();
            }
            catch (Exception es)
            {

            }

            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
