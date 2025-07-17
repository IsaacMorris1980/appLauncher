using appLauncher.Core.Model; // Assuming ColorComboItem is in Model namespace

using Microsoft.Toolkit.Uwp.Helpers; // For ToColor() extension

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class AppDisplaySettings : ModelBase 
    {
        private string _appForegroundColor = "Orange";
        private string _appBackgroundColor = "Green";
        private TimeSpan _imageRotationTime = TimeSpan.FromSeconds(15);
        private List<ColorComboItem> _appColors = new List<ColorComboItem>();

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
        public List<ColorComboItem> AppColors
        {
            get => _appColors;
            set => SetProperty(ref _appColors, value);
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