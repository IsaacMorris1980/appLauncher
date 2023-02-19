using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Windows.UI;
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
        private int _appsperscreen = 1;
        private int _lastpagenum = 0;
        private List<string> _supportedImageTypes = new List<string>();

        public List<string> SupportedImageTypes
        {
            get
            {
                if (_supportedImageTypes.Count <= 0)
                {
                    List<string> types = new List<string>
                    {
                        ".jpg",
                        ".jpeg",
                        ".jpe",
                        ".png",
                        ".svg",
                        ".tif",
                        ".tiff",
                        ".bmp",
                        ".jif",
                        ".jfif",
                        ".gif",
                        ".gifv"
                    };
                    return types;
                }
                return _supportedImageTypes;
            }
            set
            {
                _supportedImageTypes = value;
            }
        }
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
            set
            {
                GlobalVariables.SetPageNumber(value);
            }
        }
        public int AppsPerPage
        {
            get
            {
                return _appsperscreen;
            }
            set
            {
                GlobalVariables.SetPageSize(value);
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



        public Color BackgroundColor
        {
            get
            {
                return backgroundColor.ToColor();
            }
            set
            {
                SetProperty(ref backgroundColor, value.ToHex().ToString(), "AppBackgroundColorBrush");
            }
        }

        public Color ForgroundColor
        {
            get
            {
                return foregroundColor.ToColor();
            }
            set
            {
                SetProperty(ref foregroundColor, value.ToHex().ToString(), "AppForegroundColorBrush");
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppForegroundColorBrush

        {
            get
            {
                return new SolidColorBrush(ForgroundColor);
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColorBrush
        {
            get
            {
                return new SolidColorBrush(BackgroundColor);
            }
        }


    }
}
