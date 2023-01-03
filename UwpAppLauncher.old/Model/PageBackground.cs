using Newtonsoft.Json;

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

using UwpAppLauncher.Brushes;

using Windows.UI;

namespace UwpAppLauncher.Model
{
    public sealed class PageBackground : ModelBase
    {
        public PageBackground() { }
        private string _pageBackgroundName;
        private string _pageBackgroundImage;
        private double _pageBackgroundOpacity = 1.0;
        private string _pageBackgroundPath;
        public string PageBackgroundName
        {
            get
            {
                if (string.IsNullOrEmpty(_pageBackgroundName))
                {
                    return "Image Name not Retrieve";
                }
                return _pageBackgroundName;
            }
            set
            {
                SetProperty(ref _pageBackgroundName, value);
            }
        }

        public string PageBackgroundPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_pageBackgroundPath))
                {
                    return _pageBackgroundPath;
                }
                return string.Empty;
            }
            set
            {
                _pageBackgroundPath = value;
            }
        }
        [JsonIgnore]
        public byte[] PageBackgroundImage
        {
            get
            {
                if (string.IsNullOrEmpty(_pageBackgroundImage))
                {
                    return new byte[1];
                }
                return Convert.FromBase64String(_pageBackgroundImage);
            }
            set
            {
                SetProperty(ref _pageBackgroundImage, Convert.ToBase64String(value));
            }
        }
        public double PageBackgroundOpacity
        {
            get
            {
                if (_pageBackgroundOpacity < 0)
                {
                    return 0.0;
                }
                if (_pageBackgroundOpacity > 1)
                {
                    return 1.0;
                }
                return _pageBackgroundOpacity;
            }
            set
            {
                if (value < 0)
                {
                    _pageBackgroundOpacity = 0.0;
                }
                else if (value > 1)
                {
                    _pageBackgroundOpacity = 1.0;
                }
                else
                {
                    _pageBackgroundOpacity = value;
                }
            }
        }
        [JsonIgnore]
        public MaskedBrush Backimage
        {
            get
            {
                MaskedBrush mb = new MaskedBrush(PageBackgroundImage.AsBuffer().AsStream().AsRandomAccessStream(), Colors.Transparent);
                return mb;
            }
        }

    }
}
