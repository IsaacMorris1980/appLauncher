using Microsoft.Identity.Client;

using Newtonsoft.Json;

using System.Net;
// this class is for later work just getting groundwork setup
namespace appLauncher.Core.Model
{
    public class NetworkSettings : ModelBase
    {
        private bool _sync = false;
        private IPEndPoint _remoteIP = null;
        private bool _wifiOnly = false;
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
    }
}