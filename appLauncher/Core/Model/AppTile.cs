using appLauncher.Core.Brushes;

using Newtonsoft.Json;

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
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
            SetProperty(ref _appTilePackage, pack);
            SetProperty(ref _appTileListEntry, entry);
            appTileFullName = _appTilePackage.Id.FullName;
        }
        public AppTile() { }

        public void SetuptAppTileAsync(Package pack, AppListEntry entry)
        {
            SetProperty(ref _appTilePackage, pack);
            SetProperty(ref _appTileListEntry, entry);
        }
        private Package _appTilePackage;
        private AppListEntry _appTileListEntry;
        private string _appTileLogoColor;
        private string _appTileLogoOpacity;
        private string _appTileBackgroundColor;
        private string _appTileForegroundColor;
        private string _appTileBackgroundOpacity;
        private string _appTileForegroundOpacity;


        public string appTileFullName { get; set; }
        [JsonIgnore]
        public string appTileTooltipString => $"App Name: {appTileName} {Environment.NewLine}App Description: {appTileDescription}";
        [JsonIgnore]
        public string appTileDescription => this._appTileListEntry.DisplayInfo.Description;
        [JsonIgnore]
        public byte[] appTilelogo { get; private set; }
        [JsonIgnore]
        public string appTileName => this._appTileListEntry.DisplayInfo.DisplayName;
        [JsonIgnore]
        public string appTileDeveloper => this._appTilePackage.Id.Publisher;
        [JsonIgnore]
        public DateTimeOffset appTileInstalledDate => this._appTilePackage.InstalledDate;

        public string appTileLogoColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appTileLogoColor))
                {
                    return "Blue";
                }
                return _appTileLogoColor;
            }
            set
            {
                SetProperty(ref _appTileLogoColor, value, "AppTileLogo");

            }
        }
        public string appTileLogoOpacity
        {
            get
            {
                if (string.IsNullOrEmpty(_appTileLogoOpacity))
                {
                    return "255";
                }
                return _appTileLogoOpacity;
            }
            set
            {
                SetProperty(ref _appTileLogoOpacity, value, "AppTileLogo");
            }
        }
        public string appTileBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appTileBackgroundColor))
                {
                    return "Black";
                }
                return _appTileBackgroundColor;
            }
            set
            {
                SetProperty(ref _appTileBackgroundColor, value, "AppTileBackgroundColor");
            }
        }
        public string appTileBackgroundOpacity
        {
            get
            {
                if (string.IsNullOrEmpty(_appTileBackgroundOpacity))
                {
                    return "255";
                }
                return _appTileBackgroundOpacity;
            }
            set
            {
                SetProperty(ref _appTileBackgroundOpacity, value, "AppTileBackgroundColor");
            }
        }
        public string appTileTextColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appTileForegroundColor))
                {
                    return "Red";
                }
                return _appTileForegroundColor;
            }
            set
            {
                SetProperty(ref _appTileForegroundColor, value, "AppTileTextColor");
            }
        }
        public string appTileTextOpacity
        {
            get
            {
                if (string.IsNullOrEmpty(_appTileForegroundOpacity))
                {
                    return "255";
                }
                return _appTileForegroundOpacity;
            }
            set
            {
                SetProperty(ref _appTileForegroundOpacity, value, "AppTileTextColor");
            }
        }

        [JsonIgnore]
        public byte[] appTileSetlogo
        {
            set
            {
                appTilelogo = value;
            }
            //Console.Write(e.ToString());
            //Crashes.TrackError(e);
            //applogo = new byte[1];
            ////   throw e;
            //return;


        }
        [JsonIgnore]
        public MaskedBrush AppTileLogoBrush
        {
            get
            {
                Color frontcolor = Color.FromArgb(int.Parse(appTileLogoOpacity), Color.FromName(appTileLogoColor));
                Windows.UI.Color uicolor = new Windows.UI.Color();
                uicolor.A = frontcolor.A;
                uicolor.R = frontcolor.R;
                uicolor.G = frontcolor.G;
                uicolor.B = frontcolor.B;
                MaskedBrush brush = new MaskedBrush(appTilelogo.AsBuffer().AsStream().AsRandomAccessStream(), uicolor);
                return brush;
            }

        }
        [JsonIgnore]
        public SolidColorBrush AppTileBackgroundColorBrush
        {
            get
            {
                Windows.UI.Color color = new Windows.UI.Color();
                Color backcolor = Color.FromArgb(int.Parse(appTileBackgroundOpacity), Color.FromName(appTileBackgroundColor));
                color.A = backcolor.A;
                color.R = backcolor.R;
                color.G = backcolor.G;
                color.B = backcolor.B;
                return new SolidColorBrush(color);
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppTileTextColorBrush
        {
            get
            {
                Windows.UI.Color color = new Windows.UI.Color();
                Color backcolor = Color.FromArgb(int.Parse(appTileTextOpacity), Color.FromName(this.appTileTextColor));
                color.A = backcolor.A;
                color.R = backcolor.R;
                color.G = backcolor.G;
                color.B = backcolor.B;
                return new SolidColorBrush(color);
            }
        }

        public async Task<bool> appTileLaunchAsync()
        {
            return await this._appTileListEntry.LaunchAsync();
        }
    }
}
