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
        private TimeSpan _imagerotationtime = TimeSpan.FromSeconds(15);
        private int _appsperscreen = 0;
        private int _lastpagenum = 0;
        private string _appversion = string.Empty;
        private bool _showapps = false;
        private List<ColorComboItem> _appcolors = new List<ColorComboItem>();
        private bool _search = false;
        private bool _filter = false;
        private bool _images = false;
        private bool _tiles = false;
        private bool _appSettings = false;


        public GlobalAppSettings()
        {
            GlobalVariables.NumofApps += SetPageSize;
            GlobalVariables.PageNumChanged += SetPageNumber;
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            _appversion = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

        }
        public bool Search
        {
            get
            {
                return _search;
            }
            set
            {
                SetProperty(ref _search, value);
            }
        }
        public bool Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                SetProperty(ref _filter, value);
            }
        }
        public bool Images
        {
            get
            {
                return _images;
            }
            set
            {
                SetProperty(ref _images, value);
            }
        }
        public bool Tiles
        {
            get
            {
                return _tiles;
            }
            set
            {
                SetProperty(ref _tiles, value);
            }
        }
        public bool AppSettings
        {
            get
            {
                return _appSettings;
            }
            set
            {
                SetProperty(ref _appSettings, value);
            }
        }
        [JsonIgnore]
        public List<ColorComboItem> AppColors
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
                return _showapps;
            }
            set
            {
                SetProperty(ref _showapps, value);
            }
        }
        public string AppVersion
        {
            get
            {
                return _appversion;
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
