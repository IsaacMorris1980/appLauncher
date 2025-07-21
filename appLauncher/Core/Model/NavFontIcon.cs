using appLauncher.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.Model
{
    public class NavFontIcon : FontIcon
    {
        private IApporFolder apporFolder = null;
        private Type _navlocation = null;
        private string _displayName = string.Empty;
        private string _tip = string.Empty;
        private string _glyph = string.Empty;
        private string _name = string.Empty;
        public NavFontIcon() { }
        public IApporFolder AppOrFolder
        {
            get { return apporFolder; }
            set { apporFolder = value; }
        }
        public Type NavLocation
        {
            get { return _navlocation; }
            set
            {
                _navlocation = value;
            }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                this.Glyph = value;
            }
        }
        public string Tip
        {
            get { return _tip; }
            set
            {
                _tip = value;
            }
        }

    }
}