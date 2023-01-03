using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UwpAppLauncher.Model
{
    public sealed class GlobalAppSettings : ModelBase
    {
        private string _appForegroundColor;
        private string _appForegroundOpacity = "1.0";
        private string _appBackgroundOpacity = "1.0";
        private string _appBackgroundColor;
        private bool _appCrashReporting = true;
        private bool _appAnalytics = true;
        private string _appVersion;
        public string _lastpageon = "0";

        public GlobalAppSettings() { }
        public bool CrashReporting
        {
            get
            {
                return _appCrashReporting;
            }
            set
            {
                SetProperty(ref _appCrashReporting, value);
            }
        }
        public bool AnalyticsReporting
        {
            get
            {
                return _appAnalytics;
            }
            set
            {
                SetProperty(ref _appAnalytics, value);
            }
        }
        public double AppForegroundOpacity
        {
            get
            {
                return Convert.ToDouble(AppForegroundOpacity);
            }
            set
            {
                SetProperty(ref _appForegroundOpacity, value.ToString());
            }
        }
        public double AppBackgroundOpacity
        {
            get
            {
                return Convert.ToDouble(AppBackgroundOpacity);
            }
            set
            {
                SetProperty(ref _appBackgroundOpacity, value.ToString());
            }
        }
        [JsonIgnore]
        public List<string> AppColors { get; set; } = new List<string>();
        [JsonIgnore]
        public List<string> AppOpacity { get; set; } = new List<string>();
        public Color ForgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appForegroundColor))
                {
                    return "Blue".ToColor();
                }
                return _appForegroundColor.ToColor();
            }
            set
            {
                SetProperty(ref _appForegroundColor, value.ToString(), "AppForegroundColor");
            }
        }
        public Color BackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appBackgroundColor))
                {
                    return "Transparent".ToColor();
                }
                return _appBackgroundColor.ToColor();
            }
            set
            {
                SetProperty(ref _appBackgroundColor, value.ToString(), "AppBackgroundColor");
            }
        }
        public string AppVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_appVersion))
                {
                    return "2.0.11.0";
                }
                return _appVersion;
            }
            set
            {
                if (!string.Equals(value, _appVersion))
                {
                    _appVersion = value;
                }
                return;

            }
        }

        public int LastPage
        {
            get
            {
                return int.Parse(_lastpageon);
            }
            set
            {
                SetProperty(ref _lastpageon, value.ToString());
            }
        }
        [JsonIgnore]
        public SolidColorBrush ForegroundColorBrush

        {
            get
            {
                return new SolidColorBrush(ForgroundColor);



            }
        }
        [JsonIgnore]
        public SolidColorBrush BackgroundColorBrush
        {
            get
            {
                return new SolidColorBrush(BackgroundColor);
            }
        }

    }
}
