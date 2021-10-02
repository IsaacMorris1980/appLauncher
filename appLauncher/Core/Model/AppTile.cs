using applauncher.mobile.Core.Brushes;

using appLauncher.mobile.Core.Helpers;

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace applauncher.mobile.Core.Model
{
    public class AppTile
    {
        public string AppName { get; set; }
        public string AppFullName { get; set; }
        public string AppDeveloper { get; set; }
        public DateTimeOffset AppInstalled { get; set; }
        public Color AppTileForgroundcolor { get; set; }
        public Color AppTileBackgroundcolor { get; set; }
        public double AppTileOpacity { get; set; } = 1;
        public byte[] appLogo { get; set; }
        public async Task<bool> LaunchAsync()
        {
            var packages = await packageHelper.pkgManager.FindPackage(this.AppFullName).GetAppListEntriesAsync();
            return await packages[0].LaunchAsync();
        }
        public MaskedBrush ForegroundLabel()
        {
            return new MaskedBrush()
            {
                logo = this.appLogo.AsBuffer().AsStream().AsRandomAccessStream(),
                overlaycolor = this.AppTileForgroundcolor,
                Opacity = this.AppTileOpacity
            };

        }
        public SolidColorBrush BackgroundColorBrush()
        {
            return new SolidColorBrush()
            {
                Color = this.AppTileBackgroundcolor,
                Opacity = this.AppTileOpacity
            };
        }

        public SolidColorBrush AppTileForgroundBrush()
        {
            return new SolidColorBrush()
            {
                Color = this.AppTileForgroundcolor,
                Opacity = 1
            };
        }
    }
}