using appLauncher.Core.CustomEvent;
using appLauncher.Core.Pages;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class GlobalAppSettings : ModelBase
    {
        private string _appForegroundColor = "Orange";
        private string _appBackgroundColor = "Green";
        private TimeSpan _imageRotationTime = TimeSpan.FromSeconds(15);
        private int _appsPerScreen = 0;
        private int _lastPageNum = 0;
        private string _appVersion = string.Empty;
        private List<ColorComboItem> _appColors = new List<ColorComboItem>();
        private IPEndPoint _remoteIP = null;
        private bool _sync = false;
        private int _numofPages = 1;
        public event EventHandler<NetworkStatusChangedEventArgs> NetworkStatusChanged;
        private bool _wifiOnly = false;
        private bool _isConnected=false;
        private ConnectionProfile _connectionProfile;
        public bool CanEnablePreLaunch
        {
            get
            {
                return Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");
            }
        }
        public bool Sync
        {
            get
            {
                return _sync;
            }
            set
            {
                _sync = value;
            }
        }
        [JsonIgnore]
        public IPEndPoint RemoteIP
        {
            get
            {
                return _remoteIP;
            }
            set
            {
                _remoteIP = value;
            }
        }

        public GlobalAppSettings()
        {
            MainPage.pageSizeChanged += SetPageSize;
            MainPage.pageChanged += SetPageNumber;
            MainPage.numofPagesChanged += MainPage_numofPagesChanged;
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            _appVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            UpdateNetworkStatus();
            // Subscribe to network status changes
            // Remove this line, as UpdateNetworkStatus does not match the delegate signature
            // NetworkStatusChanged = new NetworkStatusChangedEventHandler(UpdateNetworkStatus);
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            Debug.WriteLine("NetworkMonitorService initialized and subscribed to NetworkStatusChanged.");

        }

        private void MainPage_numofPagesChanged(PageNumChangedArgs e)
        {
            NumOfPages = e.numofpages;
        }

        [JsonIgnore]
        public List<ColorComboItem> AppColors
        {
            get
            {
                return _appColors;
            }
            set
            {
                _appColors = value;
            }
        }
        [JsonIgnore]
        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
        }
        public void SetPageSize(PageSizeEventArgs e)
        {
            AppsPerPage = e.AppPageSize;
        }
        public void SetPageNumber(PageChangedEventArgs e)
        {
            LastPageNumber = e.PageIndex;
        }
        public int NumOfPages
        {
            get
            {
                return _numofPages;
            }
            set
            {
                _numofPages = value;
            }
        }
        public int LastPageNumber
        {
            get
            {
                return _lastPageNum;
            }
            set
            {
                _lastPageNum = value;
            }
        }
        public int AppsPerPage
        {
            get
            {
                return _appsPerScreen;
            }
            set
            {
                _appsPerScreen = value;
            }
        }
        public TimeSpan ImageRotationTime
        {
            get { return _imageRotationTime; }
            set { SetProperty(ref _imageRotationTime, value); }
        }
        public Color AppBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appBackgroundColor))
                {
                    return "Transparent".ToColor();
                }
                return _appBackgroundColor.ToColor();
            }
            set
            {
                SetProperty(ref _appBackgroundColor, value.ToString(), "AppBackgroundColorBrush");
            }
        }
        public Color AppForgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appForegroundColor))
                {
                    return "Blue".ToColor();
                }
                return _appForegroundColor.ToColor();
            }
            set
            {
                SetProperty(ref _appForegroundColor, value.ToString(), "AppForegroundColorBrush");
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppForegroundColorBrush

        {
            get
            {
                return new SolidColorBrush(AppForgroundColor);



            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColorBrush
        {
            get
            {

                return new SolidColorBrush(AppBackgroundColor);

            }
        }
        public bool WifiOnly
        {
            get => _wifiOnly;
            set
            {
                SetProperty(ref _wifiOnly, value);
            }
        }       
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                    // Raise a more specific event if needed, or rely on PropertyChanged
                    NetworkStatusChanged?.Invoke(this, new NetworkStatusChangedEventArgs(_isConnected));
                }
            }
        }
        public ConnectionProfile ConnectionProfile
        {
            get => _connectionProfile;
            set => SetProperty(ref _connectionProfile, value);
        }
        public bool IsOnWifi => ConnectionProfile.IsWlanConnectionProfile;
        private void UpdateNetworkStatus()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            IsConnected = (connectionProfile != null &&
                           connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);

            if (connectionProfile != null)
            {
                _connectionProfile = connectionProfile;
            }
        }
        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            // The event might not be on the UI thread.
            // If you need to update UI-bound properties, dispatch to the UI thread.
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateNetworkStatus();
                Debug.WriteLine($"Network status changed. IsConnected: {IsConnected}");
            });
        }
        // Dispose method to unsubscribe from the event
        public void Dispose()
        {
            NetworkInformation.NetworkStatusChanged -= NetworkInformation_NetworkStatusChanged;
            Debug.WriteLine("NetworkMonitorService unsubscribed from NetworkStatusChanged.");
        }

    }
}
