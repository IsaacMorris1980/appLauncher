using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;

using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FolderView : Page
    {
        private AppFolder displayfolder;
        private ThreadPoolTimer threadPoolTimer;

        public FolderView()
        {
            try
            {

                this.InitializeComponent();

            }
            catch (Exception es)
            {

            }
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FinalTiles selecteditem = (FinalTiles)AppsinFolders.SelectedItem;
            await PackageHelper.LaunchApp(selecteditem.FullName);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            displayfolder = (AppFolder)e.Parameter;
        }

        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
            Frame.Navigate(typeof(MainPage));
        }
        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
            Frame.Navigate(typeof(AboutPage));

        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsHelper.totalAppSettings.Images)
            {
                await ImageHelper.LoadBackgroundImages();
            }
            threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
             {
                 //
                 // Update the UI thread by using the UI core dispatcher.
                 //
                 await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                     agileCallback: () =>
                     {
                         Background = ImageHelper.GetBackbrush;

                         GC.Collect();
                     });
             }
                      , SettingsHelper.totalAppSettings.ImageRotationTime);

        }




    }
}

