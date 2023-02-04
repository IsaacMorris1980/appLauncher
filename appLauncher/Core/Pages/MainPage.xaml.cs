// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.Animations;
using appLauncher.Core.Control;
using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.UI.Animations;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace appLauncher.Core.Pages
{

    /// <summary>
    /// The page where the apps are displayed. Most of the user interactions with the app launcher will be here.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int maxRows;
        private int maxColumns;
        bool firstrun { get; set; } = true;
        public CoreDispatcher coredispatcher;
        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // milliseconds;
        TimeSpan timeSpan = TimeSpan.FromSeconds(15);
        DispatcherTimer dispatching = new DispatcherTimer();



        /// <summary>
        /// Runs when a new instance of MainPage is created
        /// </summary>
        public MainPage()
        {
            try
            {
                GlobalVariables.PageNumChanged += new PageChangedDelegate(UpdateIndicator);
                this.InitializeComponent();
                this.SizeChanged += MainPage_SizeChanged;
                sizeChangeTimer.Tick += SizeChangeTimer_Tick;
                screensContainerFlipView.Items.VectorChanged += Items_VectorChanged;
                backimage.RotationDelay = SettingsHelper.totalAppSettings.ImageRotationTime;
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during creation of main page");
                Crashes.TrackError(es);
            }

        }



        private void Dispatching_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }

        internal async void UpdateIndicator(PageChangedEventArgs e)
        {
            await AdjustIndicatorStackPanel(e.PageIndex);

        }

        // Updates grid of apps only when a bit of time has passed after changing the size of the window.
        // Better than doing this inside the the flip view item template since you don't have a timer that's always running anymore.
        private void SizeChangeTimer_Tick(object sender, object e)
        {
            try
            {
                this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) ? GlobalVariables.pagenum : 0;
                if (currentTimeLeft == 0)
                {
                    currentTimeLeft = 0;
                    sizeChangeTimer.Stop();
                    maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)screensContainerFlipView.ActualHeight);
                    maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)screensContainerFlipView.ActualWidth);
                    GlobalVariables.columns = maxColumns;
                    GlobalVariables.SetPageSize(maxColumns * maxRows);
                    int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
                    int fullPages = additionalPagesToMake;
                    int appsLeftToAdd = packageHelper.appTiles.GetOriginalCollection().Count - (fullPages * GlobalVariables.appsperscreen);
                    if (appsLeftToAdd > 0)
                    {
                        additionalPagesToMake += 1;
                    }
                    if (additionalPagesToMake > 0)
                    {

                        screensContainerFlipView.Items.Clear();
                        for (int i = 0; i < additionalPagesToMake; i++)
                        {
                            screensContainerFlipView.Items.Add(i);
                        }
                    }
                    SearchField.ItemsSource = packageHelper.searchApps.ToList();
                    this.InvalidateArrange();

                }
                else
                {
                    currentTimeLeft -= (int)sizeChangeTimer.Interval.TotalMilliseconds;
                }
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during window resize event");
                Crashes.TrackError(es);
            }

        }

        internal object getFlipview()
        {
            return screensContainerFlipView;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (firstrun)
            {
                firstrun = false;
                return;
            }
            else
            {
                if (!sizeChangeTimer.IsEnabled)
                {
                    sizeChangeTimer.Interval = TimeSpan.FromMilliseconds(updateTimerLength / 10);
                    sizeChangeTimer.Start();
                }
                currentTimeLeft = updateTimerLength;
                firstrun = false;
            }
        }

        private void Items_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            var collection = sender;
            int count = collection.Count;

            flipViewIndicatorStackPanel.Children.Clear();

            for (int i = 0; i < count; i++)
            {
                flipViewIndicatorStackPanel.Children.Add(new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(4, 0, 4, 0)
                });

            };

        }



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.Scale(2f, 2f, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 0).StartAsync();
            await this.Scale(1, 1, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 300).StartAsync();
        }



        /// <summary>
        /// Runs when the page has loaded
        /// <para> Sorts all of the apps into pages based on how
        /// based on the size of the app window/device's screen resolution.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ImageHelper.LoadBackgroundImages();
            this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) ? GlobalVariables.pagenum : 0;
            maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)screensContainerFlipView.ActualHeight);
            maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)screensContainerFlipView.ActualWidth);
            GlobalVariables.columns = maxColumns;
            GlobalVariables.SetPageSize(maxColumns * maxRows);
            int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
            int fullPages = additionalPagesToMake;
            int appsLeftToAdd = packageHelper.appTiles.Count - (fullPages * GlobalVariables.appsperscreen);
            if (appsLeftToAdd > 0)
            {
                additionalPagesToMake += 1;
            }

            if (additionalPagesToMake > 0)
            {
                //ControlTemplate template = new appControl().Template;

                for (int i = 0; i < additionalPagesToMake; i++)
                {

                    screensContainerFlipView.Items.Add(i);
                }



                screensContainerFlipView.SelectionChanged += FlipViewMain_SelectionChanged;



            }
            SearchField.ItemsSource = packageHelper.searchApps.ToList();
        }

        private void disableScrollViewer(GridView gridView)
        {
            try
            {
                var border = (Border)VisualTreeHelper.GetChild(gridView, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.IsVerticalRailEnabled = false;
                scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }

            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Returns result of calculation of extra pages needed to be added.
        /// </summary>
        /// <param name="appsPerScreen"></param>
        /// <returns></returns>
        private int calculateExtraPages(int appsPerScreen)
        {
            double appsPerScreenAsDouble = appsPerScreen;
            double numberOfApps = packageHelper.appTiles.Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }

        private async Task AdjustIndicatorStackPanel(int selectedIndex)
        {
            var indicator = flipViewIndicatorStackPanel;
            Ellipse ellipseToAnimate = new Ellipse();
            for (int i = 0; i < indicator.Children.Count; i++)
            {
                if (i == selectedIndex)
                {
                    var ellipse = (Ellipse)indicator.Children[i];
                    ellipseToAnimate = ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.Orange);

                }
                else
                {
                    var ellipse = (Ellipse)indicator.Children[i];
                    ellipse.Fill = (SolidColorBrush)App.Current.Resources["DefaultTextForegroundThemeBrush"];

                }
            }
            float centerX = (float)ellipseToAnimate.ActualWidth / 2;
            float centerY = (float)ellipseToAnimate.ActualHeight / 2;
            float animationScale = 1.7f;

            double duration = 300;
            if (IndicatorAnimation.oldAnimatedEllipse != null)
            {
                await Task.WhenAll(ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration, easingType: EasingType.Back).StartAsync(),
                    IndicatorAnimation.oldAnimatedEllipse.Scale(1, 1, centerX, centerY, duration, easingType: EasingType.Back).StartAsync());

            }
            else
            {
                await ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration, easingType: EasingType.Bounce).StartAsync();
            }

            IndicatorAnimation.oldAnimatedEllipse = ellipseToAnimate;
        }

        /// <summary>
        /// Ensures expected behaviour when using the launcher with a touch screen input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            for (int i = 1; i < screensContainerFlipView.Items.Count; i++)
            {
                var flipViewItem = this.screensContainerFlipView.ContainerFromIndex(i);
                if (flipViewItem != null)
                {
                    // We are looking for the first flipviewitem element in the visual tree of the flipviewitem. 
                    var scroller = FindFirstElementInVisualTree<GridView>(flipViewItem);
                    disableScrollViewer(scroller);
                }


                //GridView gridOfApps = (GridView)screen.Content;
                //disableScrollViewer(gridOfApps);
            }
        }

        private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = ((ComboBoxItem)Filterby.SelectedItem).Content.ToString();
            List<AppTile> orderlist;
            ObservableCollection<AppTile> newordercollection = new ObservableCollection<AppTile>();
            switch (selected)
            {
                case "AtoZ":
                    orderlist = packageHelper.appTiles.OrderBy(y => y.appTileName).ToList();
                    for (int i = 0; i < orderlist.Count - 1; i++)
                    {
                        var a = orderlist[i];
                        var c = packageHelper.appTiles.IndexOf(a);
                        packageHelper.appTiles.Move(c, i);
                    }
                    break;
                case "Developer":
                    orderlist = packageHelper.appTiles.OrderBy(x => x.appTileDeveloper).ToList();
                    for (int i = 0; i < orderlist.Count - 1; i++)
                    {
                        var a = orderlist[i];
                        var c = packageHelper.appTiles.IndexOf(a);
                        packageHelper.appTiles.Move(c, i);
                    }
                    break;

                case "Installed":
                    orderlist = packageHelper.appTiles.OrderBy(x => x.appTileInstalledDate).ToList();
                    for (int i = 0; i < orderlist.Count - 1; i++)
                    {
                        var a = orderlist[i];
                        var c = packageHelper.appTiles.IndexOf(a);
                        packageHelper.appTiles.Move(c, i);
                    }
                    break;

                default:
                    return;

            }
            this.Frame.Navigate(typeof(MainPage));
        }
        private void FlipViewMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalVariables.SetPageNumber(((FlipView)sender).SelectedIndex);
            if (e.AddedItems.Count > 0)
            {
                var flipViewItem = screensContainerFlipView.ContainerFromIndex(screensContainerFlipView.SelectedIndex);
                appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
                userControl.SwitchedToThisPage();
            }
            if (e.RemovedItems.Count > 0)
            {
                var flipViewItem = screensContainerFlipView.ContainerFromItem(e.RemovedItems[0]);
                appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
                userControl.SwitchedFromThisPage();
            }

        }

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
            {
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private void SettingsPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void SearchField_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = packageHelper.searchApps.Where(p => p.appTileName.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }

        }

        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            AppTile ap = (AppTile)args.SelectedItem;

            ap.appTileLaunchAsync().ConfigureAwait(false);
            sender.Text = String.Empty;
            sender.ItemsSource = packageHelper.searchApps;
        }


    }
}

