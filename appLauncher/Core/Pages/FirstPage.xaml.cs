using appLauncher.Core.CustomEvent;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;
using appLauncher.Core.ViewModels;

using Microsoft.Toolkit.Uwp.UI.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstPage : Page
    {
        public FirstPageViewModel ViewModel { get; }

        public FirstPage()
        {
            this.InitializeComponent();

            // Resolve ViewModel via DI container
            ViewModel = App.Services.GetRequiredService<FirstPageViewModel>();

            // Initialize the navigation service with our inner frame
            if (App.Services.GetService<INavigationService>() is FrameNavigationService navService)
            {
                
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Default load into MainPage on startup if frame is empty
            if (NavFrame.Content == null)
            {
                NavFrame.Navigate(typeof(MainPage));
            }

            await ViewModel.InitializeAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Page loaded logic
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Size change handling if required
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (NavFrame.CanGoBack) NavFrame.GoBack();
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(MainPage));
        }

        private void SettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(AppSettings));
        }

        private async void Rescan_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ViewModel.RescanAsync();
        }

        // Additional event handlers as needed...
        private void FilterApps_Tapped(object sender, TappedRoutedEventArgs e) { }
        private void Search_Tapped(object sender, TappedRoutedEventArgs e) { }
        private void CreateSpecialFolder_Tapped(object sender, TappedRoutedEventArgs e) { }
        private void InstallorRemoveApp_Tapped(object sender, TappedRoutedEventArgs e) { }
        private void AboutButton_Tapped(object sender, TappedRoutedEventArgs e) { }
    }
}

