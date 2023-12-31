using appLauncher2.Core.Views;

using System;

namespace appLauncher2.Core.Models
{
    public class HMenuItem : ModelBase
    {
        private string _name;
        private Type _pageToNavTo;
        private string _tip;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return "Menu";
                }
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public Type NavPage
        {
            get
            {
                if (_pageToNavTo == null)
                {
                    return typeof(AppsPage);
                }
                return _pageToNavTo;
            }
            set
            {
                SetProperty(ref _pageToNavTo, value);
            }

        }
        public string Tip
        {
            get
            {
                if (string.IsNullOrEmpty(_tip))
                {
                    return "Menu item's tool tip not set";
                }
                return _tip;
            }
            set
            {
                SetProperty(ref _tip, value);
            }
        }
    }
}
