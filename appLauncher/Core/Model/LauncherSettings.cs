using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;

namespace appLauncher.Core.Model
{
    public class LauncherSettings: ModelBase
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
    }
}
