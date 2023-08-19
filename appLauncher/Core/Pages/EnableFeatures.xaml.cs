using appLauncher.Core.Helpers;

using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    public sealed partial class EnableFeatures : ContentDialog
    {
        public EnableFeatures()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SettingsHelper.totalAppSettings.Search = EnableSearch.IsChecked == true ? true : false;
            SettingsHelper.totalAppSettings.Filter = EnableFilter.IsChecked == true ? true : false;
            SettingsHelper.totalAppSettings.Images = EnableImages.IsChecked == true ? true : false;
            SettingsHelper.totalAppSettings.AppSettings = EnableAppSettings.IsChecked == true ? true : false;
            SettingsHelper.totalAppSettings.Tiles = EnableTiles.IsChecked == true ? true : false;
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private void ContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            EnableFilter.IsChecked = SettingsHelper.totalAppSettings.Filter;
            EnableSearch.IsChecked = SettingsHelper.totalAppSettings.Search;
            EnableAppSettings.IsChecked = SettingsHelper.totalAppSettings.AppSettings;
            EnableTiles.IsChecked = SettingsHelper.totalAppSettings.Tiles;
            EnableImages.IsChecked = SettingsHelper.totalAppSettings.Images;
        }
    }
}
