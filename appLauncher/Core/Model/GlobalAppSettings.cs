using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class GlobalAppSettings : ModelBase
    {
        private string _appForegroundColor = "Orange";
        private string _appBackgroundColor = "Black";
        private bool _disableCrashReporting = true;
        private bool _disableAnalytics = true;
        private bool _bgimagesavailable = false;
        private bool _imagesloaded = false;
        private TimeSpan _imagerotationtime = TimeSpan.FromSeconds(15);
        private int _appsperscreen;
        private int _lastpagenum;
        private string appversion;
        private bool showapps;
        private List<string> _appcolors = new List<string>();



        public GlobalAppSettings()
        {
            GlobalVariables.NumofApps += SetPageSize;
            GlobalVariables.PageNumChanged += SetPageNumber;
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            appversion = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

        }
        [JsonIgnore]
        public List<string> AppColors
        {
            get
            {
                return _appcolors;
            }
            set
            {
                _appcolors = value;
            }
        }
        public bool ShowApps
        {
            get
            {
                return showapps;
            }
            set
            {
                SetProperty(ref showapps, value);
            }
        }
        public string AppVersion
        {
            get
            {
                return appversion;
            }
        }
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
                _lastpagenum = value;
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
                _appsperscreen = value;
            }
        }
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



        public Color appBackgroundColor
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
                SetProperty(ref _appBackgroundColor, value.ToString(), "AppBackgroundColorBrush");
            }
        }

        public Color appForgroundColor
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
                SetProperty(ref _appForegroundColor, value.ToString(), "AppForegroundColorBrush");
            }
        }
        [JsonIgnore]
        public SolidColorBrush AppForegroundColorBrush

        {
            get
            {
                return new SolidColorBrush(appForgroundColor);



            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColorBrush
        {
            get
            {

                return new SolidColorBrush(appBackgroundColor);

            }
        }

    }
}
