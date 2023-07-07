﻿using appLauncher.Core.Helpers;

using GoogleAnalyticsv4SDK;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private string appversion = $"App Version: {Helpers.SettingsHelper.totalAppSettings.AppVersion}";
        public AboutPage()
        {
            this.InitializeComponent();
            this.Loaded += AboutPage_Loaded;
        }

        private async void AboutPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var screenview = new ScreenViewEventCalls(SettingsHelper.totalAppSettings.MeasurementID, SettingsHelper.totalAppSettings.APISecret, SettingsHelper.totalAppSettings.ClientID);
            await screenview.CollectScreenViews("About Screen");
        }

        private void Home_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void SettingsPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

    }
}
