using System;

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
        public FirstPage()
        {
            this.InitializeComponent();
        }

        private void MenuButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainNavigation.IsPaneOpen = MainNavigation.IsPaneOpen != true;
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
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
    }
}
