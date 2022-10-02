using appLauncher.Core.Brushes;

using Newtonsoft.Json;

using System;
using System.Drawing;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class AppTile : ModelBase
    {
        public AppTile(Package pack, AppListEntry entry)
        {
            SetProperty(ref _package, pack);
            SetProperty(ref _applistentry, entry);
            appfullname = _package.Id.FullName;
        }
        public AppTile() { }

        public void SetuptAppTileAsync(Package pack, AppListEntry entry)
        {
            SetProperty(ref _package, pack);
            SetProperty(ref _applistentry, entry);


        }
        [JsonIgnore]
        internal Package _package;
        [JsonIgnore]
        internal AppListEntry _applistentry;
        public string appfullname { get; set; }
        public string foregroundcolor { get; set; } = "Blue";
        public string forgroundopacity { get; set; } = "255";
        public string backgroundcolor { get; set; } = "Black";
        public string backgroundopacity { get; set; } = "125";
        public string textcolor { get; set; } = "Red";
        public string textopacity { get; set; } = "255";
        [JsonIgnore]
        public byte[] applogo { get; private set; }
        [JsonIgnore]
        public string appname => this._applistentry.DisplayInfo.DisplayName;
        [JsonIgnore]
        public string appdeveloper => this._package.Id.Publisher;
        [JsonIgnore]
        public DateTimeOffset appinstalleddate => this._package.InstalledDate;
        public byte[] Setlogo
        {
            set
            {
                applogo = value;
            }
            //Console.Write(e.ToString());
            //Crashes.TrackError(e);
            //applogo = new byte[1];
            ////   throw e;
            //return;


        }
        public MaskedBrush AppLogo()
        {
            Color frontcolor = Color.FromArgb(int.Parse("255"), Color.FromName(this.foregroundcolor));
            Windows.UI.Color uicolor = new Windows.UI.Color();
            uicolor.A = frontcolor.A;
            uicolor.R = frontcolor.R;
            uicolor.G = frontcolor.G;
            uicolor.B = frontcolor.B;
            MaskedBrush brush = new MaskedBrush(applogo, uicolor);
            return brush;


        }
        public SolidColorBrush AppBackgroundColor()
        {
            Windows.UI.Color color = new Windows.UI.Color();
            Color backcolor = Color.FromArgb(int.Parse("255"), Color.FromName(this.backgroundcolor));
            color.A = backcolor.A;
            color.R = backcolor.R;
            color.G = backcolor.G;
            color.B = backcolor.B;
            return new SolidColorBrush(color);
        }
        public SolidColorBrush AppTextColor()
        {
            Windows.UI.Color color = new Windows.UI.Color();
            Color backcolor = Color.FromArgb(int.Parse("255"), Color.FromName(this.textcolor));
            color.A = backcolor.A;
            color.R = backcolor.R;
            color.G = backcolor.G;
            color.B = backcolor.B;
            return new SolidColorBrush(color);
        }
        public async Task<bool> LaunchAsync()
        {
            return await this._applistentry.LaunchAsync();
        }
    }
}
