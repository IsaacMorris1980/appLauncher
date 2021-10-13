using applauncher.Core.Brushes;

using appLauncher.Core.Helpers;

using Newtonsoft.Json;

using System;
using Windows.Foundation;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace applauncher.Core.Models
{
    public class AppTile
    {   [JsonIgnore]
        public AppListEntry AppListentry { get; set; }
        [JsonIgnore]
        public Package Pack { get; set; }
        public string AppName => this.Pack.DisplayName;
        public string AppFullName => this.Pack.Id.FullName;
        public string AppDeveloper => this.Pack.PublisherDisplayName;
        public DateTimeOffset AppInstalled => this.Pack.InstalledDate;
        public Color AppTileForgroundcolor { get; set; } = Colors.Red;
        public Color AppTileBackgroundcolor { get; set; } = Colors.Black;
        public double AppTileForegroundOpacity { get; set; } = .50;
        public double AppTileBackgroundOpacity { get; set; } = .75;

        public byte[] appLogo { get; set; }
        public async Task<bool> LaunchAsync()
        {
            return await this.AppListentry.LaunchAsync();
        }
        public MaskedBrush ForegroundLabel()
        {
            return new MaskedBrush(this.appLogo,this.AppTileForgroundcolor);

        }
        public SolidColorBrush BackgroundColorBrush()
        {
            return new SolidColorBrush(this.AppTileBackgroundcolor);
        }

        public SolidColorBrush AppTileForgroundBrush()
        {
            return new SolidColorBrush(this.AppTileForgroundcolor);
        }
        public async Task SetLogo()
        {
            var logoStream = this.AppListentry.DisplayInfo.GetLogo(new Size(50, 50));
            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
            byte[] temp = new byte[whatIWant.Size];
            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
            {
                await read.LoadAsync((uint)whatIWant.Size);
                read.ReadBytes(temp);
            }
            this.appLogo = temp;
        }
        
    }
}