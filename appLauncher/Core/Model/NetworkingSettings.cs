using appLauncher.Core.CustomEvent;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.UI.Core;

namespace appLauncher.Core.Model
{
    public class NetworkingSettings : ModelBase
    {
        public event EventHandler<NetworkStatusChangedEventArgs> NetworkStatusChanged;

        private bool _sync = false;
        private IPEndPoint _remoteIP = null;
        private bool _wifiOnly = false;
        private bool _isConnected;
        private ConnectionProfile _connectionProfile;
        public NetworkingSettings() 
        {
            UpdateNetworkStatus();
            // Subscribe to network status changes
            // Remove this line, as UpdateNetworkStatus does not match the delegate signature
            // NetworkStatusChanged = new NetworkStatusChangedEventHandler(UpdateNetworkStatus);
           NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            Debug.WriteLine("NetworkMonitorService initialized and subscribed to NetworkStatusChanged.");
        }
        // Add more network-specific properties here if needed
        public bool WifiOnly
        {
            get => _wifiOnly;
            set
            {
                SetProperty(ref _wifiOnly, value);
            }
        }
        public bool Sync
        {
            get => _sync;
            set => SetProperty(ref _sync, value);
        }

        [JsonIgnore] // Or implement a custom JsonConverter for IPEndPoint
        public IPEndPoint RemoteIP
        {
            get => _remoteIP;
            set => SetProperty(ref _remoteIP, value);
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
