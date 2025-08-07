using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;

namespace appLauncher.Core.Model
{
    public class AppVersionInfo
    {
        public string AppVersion { get; private set; }

        public bool CanEnablePreLaunch
        {
            get
            {
                return Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");
            }
        }
        public void TryEnablePrelaunch()
        {
            if (CanEnablePreLaunch)
            {
                Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
            }
        }
        public AppVersionInfo()
        {
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            AppVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
