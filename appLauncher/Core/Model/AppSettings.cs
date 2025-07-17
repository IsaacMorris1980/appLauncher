// AppSettings.cs
namespace appLauncher.Core.Model
{
    public class AppSettings : ModelBase
    {
        public AppDisplaySettings DisplaySettings { get; set; } = new AppDisplaySettings();
        public PageManagementSettings PageSettings { get; set; } = new PageManagementSettings();
        public AppVersionInfo VersionInfo { get; set; } = new AppVersionInfo();
        public NetworkSettings NetworkSettings { get; set; } = new NetworkSettings();

        public AppSettings()
        {
            
        }
    }
}