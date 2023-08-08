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
        private TimeSpan _imageRotationTime = TimeSpan.FromSeconds(15);
        private int _appsPerScreen = 0;
        private int _lastPageNum = 0;
        private string _appVersion = string.Empty;
        private bool _showApps = false;
        private List<ColorComboItem> _appColors = new List<ColorComboItem>();
        private bool _search = false;
        private bool _filter = false;
        private bool _images = false;
        private bool _tiles = false;
        private bool _appSettings = false;
        public readonly string MeasurementID = "G-WV43RHFPXN";
        public readonly string APISecret = "iVAKVkeZQ1CNQi4ONEOo9Q";
        private string _client_id;
        private bool reporting = false;

        public bool Reporting
        {
            get
            {
                return reporting;
            }
            set
            {
                reporting = value;
            }
        }
        public string ClientID
        {
            get
            {
                if (string.IsNullOrEmpty(_client_id))
                {
                    return Guid.NewGuid().ToString();
                }
                return _client_id;
            }
            set
            {
                _client_id = value;
            }
        }


        public GlobalAppSettings()
        {
            GlobalVariables.NumofApps += SetPageSize;
            GlobalVariables.PageNumChanged += SetPageNumber;
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            _appVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

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
                return _appColors;
            }
            set
            {
                _appColors = value;
            }
        }
        public bool ShowApps
        {
            get
            {
                return _showApps;
            }
            set
            {
                SetProperty(ref _showApps, value);
            }
        }
        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
        }
        public void SetPageSize(AppPageSizeChangedEventArgs e)
        {
            _appsPerScreen = e.AppPageSize;
        }
        public void SetPageNumber(PageChangedEventArgs e)
        {
            _lastPageNum = e.PageIndex;
        }
        public int LastPageNumber
        {
            get
            {
                return _lastPageNum;
            }
            set
            {
                _lastPageNum = value;
            }
        }
        public int AppsPerPage
        {
            get
            {
                return _appsPerScreen;
            }
            set
            {
                _appsPerScreen = value;
            }
        }
        public TimeSpan ImageRotationTime
        {
            get { return _imageRotationTime; }
            set { SetProperty(ref _imageRotationTime, value); }
        }
        public Color AppBackgroundColor
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
        public Color AppForgroundColor
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
                return new SolidColorBrush(AppForgroundColor);



            }
        }
        [JsonIgnore]
        public SolidColorBrush AppBackgroundColorBrush
        {
            get
            {

                return new SolidColorBrush(AppBackgroundColor);

            }
        }

    }
}
