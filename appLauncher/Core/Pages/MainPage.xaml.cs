// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.Animations;
using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Foundation;
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
        DispatcherTimer pageChangeTimer = new DispatcherTimer();

        public static event PageNumChangedDelegate PageNumChanged;

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
                pageChangeTimer.Tick += PageChangeTimer_Tick;

            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during creation of main page");
                Crashes.TrackError(es);
            }

        }

        private void PageChangeTimer_Tick(object sender, object e)
        {
            // throw new NotImplementedException();
            var a = Colors.Blue;
            var b = a.ToHex().ToString();
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
                if (currentTimeLeft == 0)
                {
                    currentTimeLeft = 0;
                    sizeChangeTimer.Stop();
                    maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)GridViewMain.ActualHeight);
                    maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)GridViewMain.ActualWidth);
                    GlobalVariables.SetPageSize(maxColumns * maxRows);
                    int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
                    int appsLeftToAdd = packageHelper.appTiles.GetOriginalCollection().Count - (additionalPagesToMake * GlobalVariables.appsperscreen);
                    if (appsLeftToAdd > 0)
                    {
                        additionalPagesToMake += 1;
                    }
                    if (additionalPagesToMake > 0)
                    {
                        SetupPageIndicators(additionalPagesToMake);
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

        private void SetupPageIndicators(int pagesCount)
        {


            flipViewIndicatorStackPanel.Children.Clear();

            for (int i = 0; i < pagesCount; i++)
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
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)GridViewMain.ActualHeight);
            maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)GridViewMain.ActualWidth);
            GlobalVariables.SetPageSize(maxColumns * maxRows);
            int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
            int appsLeftToAdd = packageHelper.appTiles.Count - (additionalPagesToMake * GlobalVariables.appsperscreen);
            if (appsLeftToAdd > 0)
            {
                additionalPagesToMake += 1;
            }

            if (additionalPagesToMake > 0)
            {

                SetupPageIndicators(additionalPagesToMake);

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
            StackPanel indicator = flipViewIndicatorStackPanel;
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
            disableScrollViewer(GridViewMain);
        }

        private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = ((ComboBoxItem)Filterby.SelectedItem).Content.ToString();
            List<Apps> orderlist;
            switch (selected)
            {
                case "AtoZ":
                    orderlist = packageHelper.appTiles.GetOriginalCollection().OrderBy(y => y.Name).ToList();
                    packageHelper.appTiles = new PaginationObservableCollection(orderlist);
                    break;
                case "Developer":
                    orderlist = packageHelper.appTiles.OrderBy(x => x.Developer).ToList();
                    packageHelper.appTiles = new PaginationObservableCollection(orderlist);
                    break;

                case "Installed":
                    orderlist = packageHelper.appTiles.OrderBy(x => x.InstalledDate).ToList();
                    packageHelper.appTiles = new PaginationObservableCollection(orderlist);
                    break;

                default:
                    return;

            }
            this.Frame.Navigate(typeof(MainPage));
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
                sender.ItemsSource = packageHelper.searchApps.Where(p => p.Name.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }

        }

        private async void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            await ((Apps)args.SelectedItem).Launch();
            sender.Text = String.Empty;
            sender.ItemsSource = packageHelper.searchApps;
        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {

            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            Point startpoint = e.GetPosition(this);
            if (GlobalVariables.startingpoint.X == 0)
            {
                GlobalVariables.startingpoint = startpoint;

            }
            else
            {

                GeneralTransform a = this.TransformToVisual(d);
                Point b = a.TransformPoint(new Point(0, 0));
                if (GlobalVariables.startingpoint.X > startpoint.X && startpoint.X < (b.X + 100))
                {
                    if (GlobalVariables.pagenum > 0)
                    {
                        int pages = GlobalVariables.pagenum;
                        GlobalVariables.SetPageNumber(pages - 1);
                        GlobalVariables.startingpoint = startpoint;
                    }

                }
                else if (GlobalVariables.startingpoint.X < startpoint.X && startpoint.X > (b.X + d.ActualWidth - 100))
                {
                    if (GlobalVariables.pagenum < GlobalVariables.NumofPages - 1)
                    {
                        int pages = GlobalVariables.pagenum;
                        GlobalVariables.SetPageNumber(pages + 1);
                        GlobalVariables.startingpoint = startpoint;
                    }

                }
            }


        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            GlobalVariables.isdragging = true;
            GlobalVariables.itemdragged.initialindex = packageHelper.appTiles.GetIndexApp((Apps)e.Items[0]);
            // packageHelper.appTiles.RemoveAt(packageHelper.appTiles.GetIndexof((AppTile)(e.Items[0])));
        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            GridView view = sender as GridView;


            //Find the position where item will be dropped in the gridview
            Point pos = e.GetPosition(view.ItemsPanelRoot);

            //Get the size of one of the list items
            GridViewItem gvi = (GridViewItem)view.ContainerFromIndex(0);
            double itemHeight = gvi.ActualHeight + gvi.Margin.Top + gvi.Margin.Bottom;
            double itemwidth = gvi.ActualHeight + gvi.Margin.Left + gvi.Margin.Right;

            //Determine the index of the item from the item position (assumed all items are the same size)
            int index = Math.Min(view.Items.Count - 1, (int)(pos.Y / itemHeight));
            int indexy = Math.Min(view.Items.Count - 1, (int)(pos.X / itemwidth));
            List<Apps> t = (List<Apps>)view.ItemsSource;
            int listindex = ((index * GlobalVariables.columns) + (indexy));

            int moveto = 0;
            if (listindex >= t.Count())
            {
                moveto = (GlobalVariables.pagenum * GlobalVariables.appsperscreen) + listindex;
                if (moveto >= packageHelper.appTiles.Count())
                {
                    moveto = (packageHelper.appTiles.Count() - 1);
                }

                //          GlobalVariables.itemdragged.indexonnewpage = t.Count();

            }
            if (listindex <= t.Count() - 1)
            {
                moveto = (GlobalVariables.pagenum * GlobalVariables.appsperscreen) + listindex;
                //GlobalVariables.itemdragged.newpage = pagenum;
                //GlobalVariables.itemdragged.indexonnewpage = ((index * GlobalVariables.columns) + (indexy));

            }


            //  packageHelper.appTiles.Moved(GlobalVariables.oldindex, appnewindex, GlobalVariables.itemdragged);
            //   AllApps.listOfApps.Move(GlobalVariables.oldindex,GlobalVariables.newindex);

            packageHelper.appTiles.Move(GlobalVariables.itemdragged.initialindex, moveto);

        }

        private void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}

