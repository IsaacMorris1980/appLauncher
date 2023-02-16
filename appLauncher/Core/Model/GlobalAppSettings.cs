using appLauncher.Core.CustomEvent;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class GlobalAppSettings : ModelBase
    {
        private string foregroundColor = "#FFFF0000";
        private string backgroundColor = "#FF000000";
        private bool _disableCrashReporting = true;
        private bool _disableAnalytics = true;
        private bool _bgimagesavailable = false;
        private bool _imagesloaded = false;
        private long imageRotationTime = TimeSpan.FromSeconds(15).Ticks;
        private int _appsperscreen;
        private int _lastpagenum;


        public GlobalAppSettings() { }
        public void SetPageSize(AppPageSizeChangedEventArgs e)
        {
            _appsperscreen = e.AppPageSize;
        }
        public void SetPageNumber(PageChangedEventArgs e)
        {
            _lastpagenum = e.PageIndex;
        }
        public int LastPageNumber
        {
            get
            {
                return _lastpagenum;
            }
        }
        public int AppsPerPage
        {
            get
            {
                return _appsperscreen;
            }
        }
        public TimeSpan ImageRotationTime
        {
            get { return TimeSpan.FromTicks(imageRotationTime); }
            set { SetProperty(ref imageRotationTime, value.Ticks); }
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


        public string appBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(backgroundColor))
                {
                    return "Transparent";
                }
                return backgroundColor;
            }
            set
            {
                SetProperty(ref backgroundColor, value, "AppBackgroundColorBrush");
            }
        }

        public string appForgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(foregroundColor))
                {
                    return "#";
                }
                return foregroundColor;
            }
            set
            {
                SetProperty(ref foregroundColor, value, "AppForegroundColorBrush");
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppForegroundColorBrush

        {
            get
            {
                Windows.UI.Color color = appForgroundColor.ToColor();


                return new SolidColorBrush(color);



            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColorBrush
        {
            get
            {
                Windows.UI.Color color = appBackgroundColor.ToColor();
                return new SolidColorBrush(color);

            }
        }


    }
}
