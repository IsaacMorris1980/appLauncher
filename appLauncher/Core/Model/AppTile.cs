//using appLauncher.Core.Brushes;

//using Microsoft.Toolkit.Uwp.Helpers;

//using Newtonsoft.Json;

//using System;
//using System.IO;
//using System.Runtime.InteropServices.WindowsRuntime;
//using System.Threading.Tasks;

//using Windows.ApplicationModel;
//using Windows.ApplicationModel.Core;
//using Windows.UI.Xaml.Media;

//namespace appLauncher.Core.Model
//{
//    public class Apps : ModelBase
//    {
//        public Apps(Package pack, AppListEntry entry)
//        {
//            SetProperty(ref _AppsPackage, pack);
//            SetProperty(ref _AppsListEntry, entry);
//            AppsFullName = _AppsPackage.Id.FullName;
//        }
//        public Apps() { }

//        public void SetuptAppsAsync(Package pack, AppListEntry entry)
//        {
//            SetProperty(ref _AppsPackage, pack);
//            SetProperty(ref _AppsListEntry, entry);
//        }
//        private Package _AppsPackage;
//        private AppListEntry _AppsListEntry;
//        private string _AppsLogoColor;
//        private string _AppsLogoOpacity;
//        private string _AppsBackgroundColor;
//        private string _AppsForegroundColor;
//        private string _AppsBackgroundOpacity;
//        private string _AppsForegroundOpacity;


//        public string AppsFullName { get; set; }
//        [JsonIgnore]
//        public string AppsTooltipString => $"App Name: {AppsName} {Environment.NewLine}App Description: {AppsDescription}";
//        [JsonIgnore]
//        public string AppsDescription => this._AppsListEntry.DisplayInfo.Description;
//        [JsonIgnore]
//        public byte[] Appslogo { get; private set; }
//        [JsonIgnore]
//        public string AppsName => this._AppsListEntry.DisplayInfo.DisplayName;
//        [JsonIgnore]
//        public string AppsDeveloper => this._AppsPackage.Id.Publisher;
//        [JsonIgnore]
//        public DateTimeOffset AppsInstalledDate => this._AppsPackage.InstalledDate;

//        public string AppsLogoColor
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_AppsLogoColor))
//                {
//                    return "Blue";
//                }
//                return _AppsLogoColor;
//            }
//            set
//            {
//                SetProperty(ref _AppsLogoColor, value, "AppsLogo");

//            }
//        }
//        public string AppsLogoOpacity
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_AppsLogoOpacity))
//                {
//                    return "255";
//                }
//                return _AppsLogoOpacity;
//            }
//            set
//            {
//                SetProperty(ref _AppsLogoOpacity, value, "AppsLogo");
//            }
//        }
//        public string AppsBackgroundColor
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_AppsBackgroundColor))
//                {
//                    return "Black";
//                }
//                return _AppsBackgroundColor;
//            }
//            set
//            {
//                SetProperty(ref _AppsBackgroundColor, value, "AppsBackgroundColor");
//            }
//        }
//        public string AppsBackgroundOpacity
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_AppsBackgroundOpacity))
//                {
//                    return "255";
//                }
//                return _AppsBackgroundOpacity;
//            }
//            set
//            {
//                SetProperty(ref _AppsBackgroundOpacity, value, "AppsBackgroundColor");
//            }
//        }
//        public string AppsTextColor
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_AppsForegroundColor))
//                {
//                    return "Red";
//                }
//                return _AppsForegroundColor;
//            }
//            set
//            {
//                SetProperty(ref _AppsForegroundColor, value, "AppsTextColor");
//            }
//        }
//        public string AppsTextOpacity
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_AppsForegroundOpacity))
//                {
//                    return "255";
//                }
//                return _AppsForegroundOpacity;
//            }
//            set
//            {
//                SetProperty(ref _AppsForegroundOpacity, value, "AppsTextColor");
//            }
//        }

//        [JsonIgnore]
//        public byte[] AppTilesetlogo
//        {
//            set
//            {
//                Appslogo = value;
//            }
//            //Console.Write(e.ToString());
//            //Crashes.TrackError(e);
//            //applogo = new byte[1];
//            ////   throw e;
//            //return;


//        }
//        [JsonIgnore]
//        public MaskedBrush AppsLogoBrush
//        {
//            get
//            {
//                Windows.UI.Color uicolor = AppsLogoColor.ToColor();
//                uicolor.A = Convert.ToByte(int.Parse(AppsLogoOpacity));
//                MaskedBrush brush = new MaskedBrush(Appslogo.AsBuffer().AsStream().AsRandomAccessStream(), uicolor);
//                return brush;
//            }

//        }
//        [JsonIgnore]
//        public SolidColorBrush AppsBackgroundColorBrush
//        {
//            get
//            {
//                Windows.UI.Color color = AppsBackgroundColor.ToColor();
//                color.A = Convert.ToByte(int.Parse(AppsBackgroundOpacity));
//                return new SolidColorBrush(color);
//            }
//        }
//        [JsonIgnore]
//        public SolidColorBrush AppsTextColorBrush
//        {
//            get
//            {
//                Windows.UI.Color color = AppsTextColor.ToColor();
//                color.A = Convert.ToByte(int.Parse(AppsTextOpacity));
//                return new SolidColorBrush(color);
//            }
//        }

//        public async Task<bool> AppsLaunchAsync()
//        {
//            return await this._AppsListEntry.LaunchAsync();
//        }
//    }
//}
