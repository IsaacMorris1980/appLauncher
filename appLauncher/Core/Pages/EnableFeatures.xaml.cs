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
            SettingsHelper.totalAppSettings.Search = (bool)EnableSearch.IsChecked;
            SettingsHelper.totalAppSettings.Filter = (bool)EnableFilter.IsChecked;
            SettingsHelper.totalAppSettings.Images = (bool)EnableImages.IsChecked;
            SettingsHelper.totalAppSettings.AppSettings = (bool)EnableAppSettings.IsChecked;
            SettingsHelper.totalAppSettings.Tiles = (bool)EnableTiles.IsChecked;
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }


    }
}
