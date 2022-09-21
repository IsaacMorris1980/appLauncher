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
            appfullname = _package.Id.FullName;
            Task t = Setlogo();
            t.Wait();
        }
        public AppTile() { }

        protected void SeuptAppTileAsync(Package pack, AppListEntry entry)
        {
            SetProperty(ref _package, pack);
            SetProperty(ref _applistentry, entry);
            Task t = Setlogo();

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

        public string appname => this._applistentry.DisplayInfo.DisplayName;
        public string appdeveloper => this._package.Id.Publisher;
        public DateTimeOffset appinstalleddate => this._package.InstalledDate;
        public async Task Setlogo()
        {


            try
            {
                RandomAccessStreamReference logoStream = this._applistentry.DisplayInfo.GetLogo(new Windows.Foundation.Size(50, 50));
                IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                byte[] temp = new byte[whatIWant.Size];
                using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                {
                    await read.LoadAsync((uint)whatIWant.Size);
                    read.ReadBytes(temp);
                }
                applogo = temp;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                applogo = new byte[1];
                //   throw e;
            }

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
