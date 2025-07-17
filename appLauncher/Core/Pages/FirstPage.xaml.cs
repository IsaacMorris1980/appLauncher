using appLauncher.Core.CustomEvent;
using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls; // Ensure you have an older compatible version installed
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace appLauncher.Core.Pages
{
    public sealed partial class FirstPage : Page
    {
        internal static Action<PageChangedEventArgs> pageChanged;

       public FirstPageViewModel ViewModel { get; }
       public FirstPage()
        {
            this.InitializeComponent();
            ViewModel = new FirstPageViewModel();
            this.DataContext = ViewModel;

            // Subscribe to ViewModel's events for UI-specific actions
            ViewModel.NavigationRequested += ViewModel_NavigationRequested;
            ViewModel.NotificationRequested += ViewModel_NotificationRequested;

            NavFrame.Navigated += NavFrame_Navigated;

            // Initial navigation
            NavFrame.Navigate(typeof(AppLoading));
        }
        private void ViewModel_NavigationRequested(object sender, FirstPageViewModel.NavigationRequestedEventArgs e)
        {
            switch (e.Type)
            {
                case FirstPageViewModel.NavigationType.NavigateToPage:
                    if (e.TargetPageType != null)
                    {
                        NavFrame.Navigate(e.TargetPageType, e.Parameter);
                    }
                    break;
                case FirstPageViewModel.NavigationType.GoBack:
                    if (NavFrame.CanGoBack)
                    {
                        NavFrame.GoBack();
                    }
                    break;
                case FirstPageViewModel.NavigationType.ChangePage:
                    // This assumes PackageHelper.Apps.PageChanged handles the actual page display
                    // and the UI (MainPage.xaml) binds to the data updated by that.
                    // If NavFrame needs to re-navigate to MainPage with a parameter indicating page,
                    // you'd do that here.
                    NavFrame.Navigate(typeof(MainPage)); // Re-navigate to refresh MainPage content
                    break;
            }
        }
        private void ViewModel_NotificationRequested(object sender, FirstPageViewModel.NotificationRequestedEventArgs e)
        {
            Inapp.Show(e.Message, e.Duration);
        }
        private void NavFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update ViewModel property based on current page
            ViewModel.CurrentPageName = e.SourcePageType == typeof(MainPage) ? "mainpage" : string.Empty;
        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavFrame.Height = MainNavigation.ActualHeight;
            NavFrame.Width = MainNavigation.ActualWidth - 50;
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NavFrame.Height = MainNavigation.ActualHeight;
            NavFrame.Width = MainNavigation.ActualWidth - 50;
        }

        private void Searching_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SearchTextChangedCommand.Execute(((TextBox)sender).Text);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItem is Windows.UI.Xaml.Shapes.Ellipse selectedEllipse && selectedEllipse.Tag is int pageIndex)
            {
                ViewModel.PageIndicatorSelectedCommand.Execute(pageIndex);
            }
        }

        private void MainNavigation_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            MainNavigation.IsPaneOpen = true;
        }

        private void MainNavigation_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            MainNavigation.IsPaneOpen = false;
        }
    }
}