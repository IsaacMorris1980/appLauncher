using Newtonsoft.Json;

using System.Collections.Generic;

using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class GlobalAppSettings : ModelBase
    {
        private string _appForegroundColor;
        private string _appForegroundOpacity;
        private string _appBackgroundColor;
        private string _appBackgroundOpacity;

        public GlobalAppSettings() { }
        public bool disableCrashReporting { get; set; } = true;
        public bool disableAnalytics { get; set; } = true;
        [JsonIgnore]
        public List<string> AppColors { get; set; } = new List<string>();
        [JsonIgnore]
        public List<string> AppOpacity { get; set; } = new List<string>();
        public string appForgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appForegroundColor))
                {
                    return "Blue";
                }
                return _appForegroundColor;
            }
            set
            {
                SetProperty(ref _appForegroundColor, value, "AppForegroundColor");
            }
        }
        public string appForegroundOpacity
        {
            get
            {
                if (string.IsNullOrEmpty(_appForegroundOpacity))
                {
                    return "255";
                }
                return _appForegroundOpacity;
            }
            set
            {
                SetProperty(ref _appForegroundOpacity, value, "AppForegroundColor");
            }
        }
        public string appBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appBackgroundColor))
                {
                    return "Transparent";
                }
                return _appBackgroundColor;
            }
            set
            {
                SetProperty(ref _appBackgroundColor, value, "AppBackgroundColor");
            }
        }
        public string appBackgroundOpacity
        {
            get
            {
                if (string.IsNullOrEmpty(_appBackgroundOpacity))
                {
                    return "255";
                }
                return _appBackgroundOpacity;
            }
            set
            {
                SetProperty(ref _appBackgroundOpacity, value, "AppBackgroundColor");
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppForegroundColor

        {
            get
            {
                Windows.UI.Color color = new Windows.UI.Color();
                System.Drawing.Color backcolor = System.Drawing.Color.FromArgb(int.Parse(appForegroundOpacity), System.Drawing.Color.FromName(appForgroundColor));
                color.A = backcolor.A;
                color.R = backcolor.R;
                color.G = backcolor.G;
                color.B = backcolor.B;
                return new SolidColorBrush(color);



            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColor
        {
            get
            {
                Windows.UI.Color color = new Windows.UI.Color();
                System.Drawing.Color backcolor = System.Drawing.Color.FromArgb(int.Parse(appBackgroundOpacity), System.Drawing.Color.FromName(appBackgroundColor));
                color.A = backcolor.A;
                color.R = backcolor.R;
                color.G = backcolor.G;
                color.B = backcolor.B;
                return new SolidColorBrush(color);

            }
        }


    }
}
