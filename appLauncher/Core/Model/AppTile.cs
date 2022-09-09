using appLauncher.Core.Brushes;

using Newtonsoft.Json;

using System;
using System.Drawing;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class AppTile : ModelBase
    {
        public AppTile(Package pack, AppListEntry entry)
        {
            SetProperty(ref _package, pack);
            SetProperty(ref _applistentry, entry);
        }
        public AppTile() { }
        protected void SetPackage(Package pack)
        {
            SetProperty(ref _package, pack);
        }
        protected void SetAppListEntry(AppListEntry entry)
        {
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

        public string AppName()
        {
            return this._package.Id.Name;
        }
        public string AppDeveloper()
        {
            return this._package.Id.Publisher;
        }
        public DateTimeOffset AppInstalledDate()
        {
            return this._package.InstalledDate;
        }
        public async Task<MaskedBrush> AppLogoAsync()
        {
            try
            {
                RandomAccessStreamReference logoStream = this.applistentry.DisplayInfo.GetLogo(new Windows.Foundation.Size(50, 50));
                IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                byte[] temp = new byte[whatIWant.Size];
                using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                {
                    await read.LoadAsync((uint)whatIWant.Size);
                    read.ReadBytes(temp);
                }
                Color frontcolor = Color.FromArgb(int.Parse(this.forgroundopacity), Color.FromName(this.foregroundcolor));
                Windows.UI.Color uicolor = new Windows.UI.Color();
                uicolor.A = frontcolor.A;
                uicolor.R = frontcolor.R;
                uicolor.G = frontcolor.G;
                uicolor.B = frontcolor.B;
                MaskedBrush brush = new MaskedBrush(temp, uicolor);
                return brush;

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw e;
            }
        }
        public SolidColorBrush AppBackgroundColor()
        {
            Windows.UI.Color color = new Windows.UI.Color();
            Color backcolor = Color.FromArgb(int.Parse(backgroundopacity), Color.FromName(this.backgroundcolor));
            color.A = backcolor.A;
            color.R = backcolor.R;
            color.G = backcolor.G;
            color.B = backcolor.B;
            return new SolidColorBrush(color);
        }
        public SolidColorBrush AppTextColor()
        {
            Windows.UI.Color color = new Windows.UI.Color();
            Color backcolor = Color.FromArgb(int.Parse(this.textopacity), Color.FromName(this.textcolor));
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
