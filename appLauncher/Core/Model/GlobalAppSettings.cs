using appLauncher.Core.CustomEvent;
using appLauncher.Core.Pages;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Net;

using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class GlobalAppSettings : ModelBase
    {
        private string _appForegroundColor = "Orange";
        private string _appBackgroundColor = "Green";
        private TimeSpan _imageRotationTime = TimeSpan.FromSeconds(15);
        private int _appsPerScreen = 0;
        private int _lastPageNum = 0;
        private string _appVersion = string.Empty;
        private List<ColorComboItem> _appColors = new List<ColorComboItem>();
        private IPEndPoint _remoteIP = null;
        private bool _sync = false;
        public bool CanEnablePreLaunch
        {
            get
            {
                return Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");
            }
        }
        public bool Sync
        {
            get
            {
                return _sync;
            }
            set
            {
                _sync = value;
            }
        }
        [JsonIgnore]
        public IPEndPoint RemoteIP
        {
            get
            {
                return _remoteIP;
            }
            set
            {
                _remoteIP = value;
            }
        }

        public GlobalAppSettings()
        {
            MainPage.pageSizeChanged += SetPageSize;
            MainPage.pageChanged += SetPageNumber;
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            _appVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

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
        [JsonIgnore]
        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
        }
        public void SetPageSize(PageSizeEventArgs e)
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
