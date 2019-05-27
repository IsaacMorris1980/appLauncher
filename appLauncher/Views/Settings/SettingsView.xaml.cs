using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Views.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public ObservableCollection<SettingsItem> Items;
        public SettingsView()
        {
            this.InitializeComponent();
        }

        private void GenerateItems()
        {
            List<SettingsItem> personalisationSettings = new List<SettingsItem>
            {
                new SettingsItem("Background", "Customise background type", "\uEB9F",null),
                new SettingsItem("Font", "Change font size", "\uE8D2", null)
            };



            List<SettingsItem> itemsToAdd = new List<SettingsItem>
            {
                new SettingsItem("Personalisation", "Customise background, Font Size", "\uE771", personalisationSettings),
                new SettingsItem("About", "Support Info, Open Source Licenses", "\uE897", null)
            };


            Items = new ObservableCollection<SettingsItem>(itemsToAdd);
        }

        private void SettingsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (SettingsItem)e.ClickedItem;
            SettingsItemArgs childrenArgs = clickedItem.NavigateToChild();
            if (childrenArgs.SettingsItems != null)
            {
                Frame.Navigate(typeof(SettingsView), childrenArgs.SettingsItems);
            }
            else
            {
                Frame.Navigate(childrenArgs.ViewType);
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is List<SettingsItem> itemsToUse)
            {
                Items = new ObservableCollection<SettingsItem>(itemsToUse);
            }
            else
            {
                GenerateItems();
            }
        }


    }
}
