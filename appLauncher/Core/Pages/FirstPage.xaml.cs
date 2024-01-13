using System;
using System.Diagnostics;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstPage : Page
    {
        public static Frame navFrame { get; set; }
        public FirstPage()
        {
            this.InitializeComponent();
            navFrame = NavFrame;
            NavFrame.Navigate(typeof(AppLoading));
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("In back tapped");
            if (NavFrame.CanGoBack)
            {
                NavFrame.GoBack();
            }
        }

        private void ForwardButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (NavFrame.CanGoForward)
            {
                NavFrame.GoForward();
                NavFrame.BackStack.RemoveAt(NavFrame.BackStackDepth - 1);
                GC.Collect();
            }
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(MainPage));
        }

        private void SettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(SettingsPage));
        }

        private void AboutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(AboutPage));
        }

        private void FilterApps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt(((FontIcon)sender));
        }

        private void AlphaAZ_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Search_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
