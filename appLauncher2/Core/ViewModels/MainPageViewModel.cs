using appLauncher2.Core.Models;
using appLauncher2.Core.Views;

using System.Collections.Generic;

namespace appLauncher2.Core.ViewModels
{
    public class MainPageViewModel
    {
        public List<HMenuItem> MenuItems { get; set; }
        private HMenuItem _selectedItem;
        public MainPageViewModel()
        {
            MenuItems = new List<HMenuItem>
            {
                new HMenuItem
                {
                    Name="Menu",
                    NavPage = null,
                    Tip = "Main Menu"
                },
                new HMenuItem
                {
                    Name = "Back",
                    NavPage = null,
                    Tip = "Navigate to the previous page"
                },
                new HMenuItem
                {
                    Name = "Forward",
                    NavPage = null,
                    Tip = "Navigate forward to next page"
                },
                new HMenuItem
                {
                    Name = "All Apps",
                    NavPage = typeof(AppsPage),
                    Tip = "Navigate to the all Apps Page"
                },
                new HMenuItem
                {
                    Name = "Settings",
                    NavPage = typeof(SettingsPage),
                    Tip = "Navigate to the Settings Page"
                },
                new HMenuItem
                {
                    Name = "About",
                    NavPage = typeof(AboutPage),
                    Tip = "Navigate to the About Page"
                }
            };
        }
    }
}
