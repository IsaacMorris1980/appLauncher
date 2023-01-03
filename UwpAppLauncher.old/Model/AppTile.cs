using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using UwpAppLauncher.Brushes;
using UwpAppLauncher.Interfaces;

using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UwpAppLauncher.Model
{
    public sealed class AppTile : ModelBase, IApporFolder
    {
        public AppTile()
        {

        }
        /// <summary>
        /// These need to be serialized
        /// </summary>
        private int _listlocation;
        private bool _inFolder = false;
        private string _partOfFolderName = string.Empty;
        private string _appFullName;
        private string _appTip;
        private string _appForegroundColor = "Blue";
        private string _appForegroundOpacity = "255";
        private string _appTextColor = "Red";
        private string _appTextOpacity = "255";
        private string _appBackgroundColor = "Black";
        private string _appBackgroundOpacity = "255";




        /// <summary>
        /// Do not need to be serialized
        /// </summary>
        private string _notfound = "{0} not Retrieved";
        private AppListEntry _appEntry;
        private string _appName;
        private string _appDescription;
        private string _appFullSizeLogo;
        private string _appFolderSizeLogo;
        private string _appDeveloper;
        private long _appInstalledDate;


        public int Listlocation
        {
            get
            {
                if (_listlocation > -1)
                {
                    return _listlocation;
                }
                return -1;
            }
            set
            {
                SetProperty(ref _listlocation, value);
            }
        }
        public bool InFolder
        {
            get
            {
                return _inFolder;
            }
            set
            {
                SetProperty(ref _inFolder, value);
            }
        }
        public string FolderName
        {
            get
            {
                return _partOfFolderName;
            }
            set
            {
                SetProperty(ref _partOfFolderName, value);
            }
        }
        public string AppFullName
        {
            get
            {
                if (string.IsNullOrEmpty(_appFullName))
                {
                    return string.Format(_notfound, "App Full Name");
                }
                return _appFullName;
            }
            set
            {
                _appFullName = value;
            }
        }
        public string AppTip
        {
            get
            {
                if (string.IsNullOrEmpty(_appTip))
                {
                    return $"App Name: {Name}{Environment.NewLine} App Description: {AppDescription}";
                }
                return _appTip;
            }
            set
            {
                _appTip = value;
            }
        }
        public Color ForegroundColor
        {
            get
            {
                return _appForegroundColor.ToColor();
            }
            set
            {
                SetProperty(ref _appForegroundColor, value.ToString());
            }
        }
        public string ForegroundOpacity
        {
            get
            {
                return _appForegroundOpacity;
            }
            set
            {
                SetProperty(ref _appForegroundOpacity, value);
            }
        }
        public Color BackgroundColor
        {
            get
            {
                return _appBackgroundColor.ToColor();
            }
            set
            {
                SetProperty(ref _appBackgroundColor, value.ToString());
            }
        }
        public string BackgroundOpacity
        {
            get
            {
                return _appBackgroundOpacity;
            }
            set
            {
                SetProperty(ref _appBackgroundOpacity, value);
            }
        }
        public Color TextColor
        {
            get
            {
                return _appTextColor.ToColor();
            }
            set
            {
                SetProperty(ref _appTextColor, value.ToString());
            }
        }
        public string TextOpacity
        {
            get
            {
                return _appTextOpacity;
            }
            set
            {
                SetProperty(ref _appTextOpacity, value);
            }
        }






        [JsonIgnore]
        public AppListEntry AppEntry
        {
            get
            {
                return _appEntry;
            }
            set
            {
                _appEntry = value;
            }
        }
        [JsonIgnore]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_appName))
                {
                    return string.Format(_notfound, "App Name");
                }
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }
        [JsonIgnore]
        public string AppDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_appDescription))
                {
                    return string.Format(_notfound, "App Description");
                }
                return _appDescription;
            }
            set
            { _appDescription = value; }
        }
        [JsonIgnore]
        public string AppDeveloper
        {
            get
            {
                if (string.IsNullOrEmpty(_appDeveloper))
                {
                    return string.Format(_notfound, "App Developer");
                }
                return _appDeveloper;
            }
            set
            {
                _appDeveloper = value;
            }
        }
        [JsonIgnore]
        public DateTimeOffset AppInstalledDate
        {
            get
            {
                if (_appInstalledDate <= 0)
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(0);
                }
                return DateTimeOffset.FromUnixTimeMilliseconds(_appInstalledDate);
            }
            set
            {
                _appInstalledDate = value.ToUnixTimeMilliseconds();
            }
        }
        [JsonIgnore]
        public byte[] AppFullSizeLogo
        {
            get
            {
                if (string.IsNullOrEmpty(_appFullSizeLogo))
                {
                    return new byte[1];
                }
                return Convert.FromBase64String(_appFullSizeLogo);
            }
            set
            {
                _appFullSizeLogo = Convert.ToBase64String(value);
            }
        }
        [JsonIgnore]
        public byte[] FolderSizeLogo
        {
            get
            {
                if (string.IsNullOrEmpty(_appFolderSizeLogo))
                {
                    return new byte[1];
                }
                return Convert.FromBase64String(_appFolderSizeLogo);
            }
            set
            {
                _appFolderSizeLogo = Convert.ToBase64String(value);
            }
        }
        [JsonIgnore]
        public SolidColorBrush TextBrush
        {
            get
            {
                return new SolidColorBrush(TextColor);
            }
        }
        [JsonIgnore]
        public MaskedBrush LogoForegroundBrush
        {
            get
            {

                return new MaskedBrush(FolderSizeLogo.AsBuffer().AsStream().AsRandomAccessStream(), ForegroundColor);
            }
        }
        [JsonIgnore]
        public MaskedBrush FolderTileForegroundBrush
        {
            get
            {
                return new MaskedBrush(FolderSizeLogo.AsBuffer().AsStream().AsRandomAccessStream(), ForegroundColor);
            }
        }
        [JsonIgnore]
        public SolidColorBrush BackgroundBrush
        {
            get
            {
                return new SolidColorBrush(BackgroundColor);
            }
        }
        public async Task<bool> LaunchAsync()
        {
            return await this.AppEntry.LaunchAsync();
        }

    }
}
