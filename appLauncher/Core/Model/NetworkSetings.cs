using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Networking.Connectivity;

namespace appLauncher.Core.Model
{
    public  class NetworkSettings:ModelBase,IDisposable
    {
        public bool _syncWifiOnly => true;
        public bool IsConnected { get; set; } = false;
        public ConnectionProfile ConnectionProfile { get; set; }    

        public NetworkSettings()
        {
           
        }
        public void Dispose()
        {

        }
    }
}
