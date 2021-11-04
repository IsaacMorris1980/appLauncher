using appLauncher.Core.Brushes;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class AppTile
    {
        [JsonIgnore]
        public AppListEntry AppListentry { get; set; }
        [JsonIgnore]
        public Package Pack { get; set; }
        public string AppName => this.Pack.DisplayName;
        public string AppFullName => this.Pack.Id.FullName;
        public string AppDeveloper => this.Pack.PublisherDisplayName;
        public DateTimeOffset AppInstalled => this.Pack.InstalledDate;
        public Color AppTileForegroundcolor { get; set; } = Colors.Red;
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
            return new MaskedBrush(this.appLogo, this.AppTileForegroundcolor);

        }
        public SolidColorBrush BackgroundColorBrush()
        {
            return new SolidColorBrush(this.AppTileBackgroundcolor);
        }

        public SolidColorBrush AppTileForegroundBrush()
        {
            return new SolidColorBrush(this.AppTileForegroundcolor);
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