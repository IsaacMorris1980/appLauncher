using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class GlobalAppSettings : ModelBase
    {
        private string _appForegroundColor = "Orange";
        private string _appForegroundOpacity = "150";
        private string _appBackgroundColor = "Black";
        private string _appBackgroundOpacity = "255";
        private bool _disableCrashReporting = true;
        private bool _disableAnalytics = true;
        private string _appBorderColor = "Silver";
        private bool _bgimagesavailable = false;
        private bool _imagesloaded = false;
        private TimeSpan _imagerotationtime = TimeSpan.FromSeconds(15);


        public GlobalAppSettings() { }
        public TimeSpan ImageRotationTime
        {
            get { return _imagerotationtime; }
            set { SetProperty(ref _imagerotationtime, value); }
        }
        public bool ImagesLoaded
        {
            get { return _imagesloaded; }
            set { SetProperty(ref _imagesloaded, value); }
        }
        public bool BgImagesAvailable
        {
            get { return _bgimagesavailable; }
            set { SetProperty(ref _bgimagesavailable, value); }
        }
        public bool disableCrashReporting
        {
            get
            {
                return _disableCrashReporting;
            }
            set
            {
                SetProperty(ref _disableCrashReporting, value);
            }
        }
        public bool disableAnalytics
        {
            get
            {
                return _disableAnalytics;
            }
            set
            {
                SetProperty(ref _disableAnalytics, value);
            }
        }
        [JsonIgnore]
        public List<string> AppColors { get; set; } = new List<string>();
        [JsonIgnore]
        public List<string> AppOpacity { get; set; } = new List<string>();
        public string appBorderColor
        {
            get
            {
                if (string.IsNullOrEmpty(_appBorderColor))
                {
                    return "Blue";
                }
                return _appBorderColor;
            }
            set
            {
                SetProperty(ref _appBorderColor, value, "AppBorderColorBrush");
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
                SetProperty(ref _appBackgroundColor, value, "AppBackgroundColorBrush");
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
                SetProperty(ref _appForegroundColor, value, "AppForegroundColorBrush");
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppForegroundColorBrush

        {
            get
            {
                Windows.UI.Color color = appForgroundColor.ToColor();
                color.A = Convert.ToByte(int.Parse(appForegroundOpacity));

                return new SolidColorBrush(color);



            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColorBrush
        {
            get
            {
                Windows.UI.Color color = appBackgroundColor.ToColor();
                color.A = Convert.ToByte(int.Parse(appBackgroundOpacity));
                return new SolidColorBrush(color);

            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBorderColorBrush

        {
            get
            {
                Windows.UI.Color color = appBorderColor.ToColor();
                color.A = Convert.ToByte(int.Parse("255"));

                return new SolidColorBrush(color);



            }
        }
    }
}
