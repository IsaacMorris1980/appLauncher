// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.UI.Animations;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
        public int Maxicons { get; private set; }


        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // milliseconds;


        private Button oldAnimatedButton;
        private Button buttontoanimate;
        private Ellipse ellipseToAnimate;

        private int previousSelectedIndex = 0;

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
                this.listView.SelectionChanged += ListView_SelectionChanged;

            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during creation of main page");
                Crashes.TrackError(es);
            }

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"Previous Selected: {previousSelectedIndex}");
            Debug.WriteLine($"Current Selected: {listView.SelectedIndex}");
            if (listView.SelectedItem == null)
            {
                return;
            }
            if (listView.SelectedIndex > -1)
            {
                var item = listView.SelectedItem;
                // Calculations relative to screen or ListView
                FrameworkElement listViewItem = (FrameworkElement)listView.ContainerFromItem(item);
                if (listViewItem == null)
                {
                    listView.ScrollIntoView(item);
                }
                int changeint = listView.SelectedIndex;
                if (previousSelectedIndex < changeint)
                {
                    if (changeint >= (Maxicons - 1))
                    {
                        changeint -= 1;
                    }
                    else
                    {
                        changeint += 1;
                    }
                }
                if (previousSelectedIndex > changeint)
                {
                    if (changeint <= 0)
                    {
                        changeint += 1;
                    }
                    else
                    {
                        changeint -= 1;
                    }
                }
                listView.ScrollIntoView(listView.Items[changeint]);
                previousSelectedIndex = listView.SelectedIndex;

            }

        }

        public void UpdateIndicator(PageChangedEventArgs e)
        {
            packageHelper.pageVariables.IsPrevious = e.PageIndex > 0;
            packageHelper.pageVariables.IsNext = e.PageIndex < GlobalVariables.numOfPages - 1;
            AdjustIndicatorStackPanel(e.PageIndex);

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
                    GlobalVariables.columns = maxColumns;
                    GlobalVariables.SetPageSize(maxColumns * maxRows);
                    int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
                    int fullPages = additionalPagesToMake;
                    int appsLeftToAdd = packageHelper.Apps.GetOriginalCollection().Count - (fullPages * GlobalVariables.appsperscreen);
                    if (appsLeftToAdd > 0)
                    {
                        additionalPagesToMake += 1;
                    }
                    if (additionalPagesToMake > 0)
                    {
                        Maxicons = additionalPagesToMake;
                        GlobalVariables.SetNumOfPages(additionalPagesToMake);
                        SetupPageIndicators(additionalPagesToMake);
                        if (SettingsHelper.totalAppSettings.LastPageNumber > (additionalPagesToMake - 1))
                        {
                            SettingsHelper.totalAppSettings.LastPageNumber = (additionalPagesToMake - 1);
                        }
                        packageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
                        packageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < GlobalVariables.numOfPages - 1;

                    }
                    AdjustIndicatorStackPanel(SettingsHelper.totalAppSettings.LastPageNumber);
                    SearchField.ItemsSource = packageHelper.searchApps.ToList();
                    GlobalVariables.SetPageNumber(SettingsHelper.totalAppSettings.LastPageNumber);
                    previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
                }
                else
                {
                    currentTimeLeft -= (int)sizeChangeTimer.Interval.TotalMilliseconds;
                }
                AdjustIndicatorStackPanel(SettingsHelper.totalAppSettings.LastPageNumber);
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during window resize event");
                Crashes.TrackError(es);
            }

        }

        //internal object getFlipview()
        //{
        //    return screensContainerFlipView;
        //}

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

        public void DelayDragOver(int timetopause)
        {
            Thread.Sleep(timetopause);
        }



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.Scale(2f, 2f, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 0).StartAsync();
            await this.Scale(1, 1, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 300).StartAsync();
        }
        private void SetupPageIndicators(int e)
        {
            listView.Items.Clear();
            for (int i = 0; i < e; i++)
            {
                Button btn = new Button();
                btn.Tag = i;
                btn.Background = new SolidColorBrush(Colors.Transparent);
                Ellipse el = new Ellipse
                {
                    Tag = i,
                    Height = 8,
                    Width = 8,
                    Margin = new Thickness(12),
                    Fill = new SolidColorBrush(Colors.Gray),

                };
                btn.Tapped += Btn_Tapped;
                btn.Content = el;
                ToolTipService.SetToolTip(btn, $"Page {i + 1}");
                listView.Items.Add(btn);
            }
        }

        private void Btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int a = (int)btn.Tag;
            GlobalVariables.SetPageNumber(a);
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
            GridViewMain.ItemsSource = packageHelper.Apps;
            await ImageHelper.LoadBackgroundImages();
            maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)GridViewMain.ActualHeight);
            maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)GridViewMain.ActualWidth);
            GlobalVariables.columns = maxColumns;
            GlobalVariables.SetPageSize(maxColumns * maxRows);
            int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
            int fullPages = additionalPagesToMake;
            int appsLeftToAdd = packageHelper.Apps.GetOriginalCollection().Count - (fullPages * GlobalVariables.appsperscreen);
            if (appsLeftToAdd > 0)
            {
                additionalPagesToMake += 1;
            }

            if (additionalPagesToMake > 0)
            {
                if (SettingsHelper.totalAppSettings.LastPageNumber > (additionalPagesToMake - 1))
                {
                    SettingsHelper.totalAppSettings.LastPageNumber = (additionalPagesToMake - 1);
                }
                Maxicons = additionalPagesToMake;
                SetupPageIndicators(additionalPagesToMake);
                GlobalVariables.SetNumOfPages(additionalPagesToMake);
                packageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
                packageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < GlobalVariables.numOfPages - 1;
            }

            AdjustIndicatorStackPanel(SettingsHelper.totalAppSettings.LastPageNumber);
            previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
            GlobalVariables.SetPageNumber(SettingsHelper.totalAppSettings.LastPageNumber);
            SearchField.ItemsSource = packageHelper.searchApps.ToList();
        }

        private void Items_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            var collection = sender;
            int count = collection.Count;


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

            catch (Exception e)
            {
                Analytics.TrackEvent("Crashed during disabling scrollviewer on gridviews");
                Crashes.TrackError(e);
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
            double numberOfApps = packageHelper.Apps.GetOriginalCollection().Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }

        private void AdjustIndicatorStackPanel(int selectedIndex)
        {
            try
            {
                if (!firstrun)
                {
                    if (oldAnimatedButton != null)
                    {
                        Button oldbutton = null;

                        oldbutton = oldAnimatedButton;
                        Ellipse olderellipse = (Ellipse)oldbutton.Content;
                        buttontoanimate = (Button)listView.Items[selectedIndex];
                        if (oldAnimatedButton != buttontoanimate && buttontoanimate != null)
                        {


                            ellipseToAnimate = (Ellipse)buttontoanimate.Content;
                            ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                            olderellipse.RenderTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1 };
                            ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                            olderellipse.Fill = new SolidColorBrush(Colors.Gray);
                            oldAnimatedButton = buttontoanimate;
                        }
                    }
                    else
                    {
                        buttontoanimate = (Button)listView.Items[selectedIndex];
                        ellipseToAnimate = (Ellipse)buttontoanimate.Content;
                        ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                        ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                        oldAnimatedButton = buttontoanimate;

                    }
                    listView.SelectedIndex = selectedIndex;
                }

            }
            catch (Exception e)
            {
                Analytics.TrackEvent("Crashed during updating selected page");
                Crashes.TrackError(e);
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
                sender.ItemsSource = packageHelper.searchApps.Where(p => p.Name.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }

        }

        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Apps ap = (Apps)args.SelectedItem;

            packageHelper.LaunchApp(ap.FullName).ConfigureAwait(false);
            sender.Text = String.Empty;
            sender.ItemsSource = packageHelper.searchApps;
        }

        private void PreviousPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (packageHelper.pageVariables.IsPrevious)
            {
                GlobalVariables.SetPageNumber(GlobalVariables.pagenum - 1);
            }

        }

        private void NextPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (packageHelper.pageVariables.IsNext)
            {
                GlobalVariables.SetPageNumber(GlobalVariables.pagenum + 1);
            }

        }

        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((AppBarButton)sender).ContextFlyout.ShowAt((AppBarButton)sender);
        }

        private void AlphaAZ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            packageHelper.Apps.GetFilteredApps("AppAZ");
        }

        private void AlphaZA_Tapped(object sender, TappedRoutedEventArgs e)
        {
            packageHelper.Apps.GetFilteredApps("AppZA");
        }

        private void DevAZ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            packageHelper.Apps.GetFilteredApps("DevAZ");
        }

        private void DevZA_Tapped(object sender, TappedRoutedEventArgs e)
        {
            packageHelper.Apps.GetFilteredApps("DevZA");
        }

        private void InstalledNewest_Tapped(object sender, TappedRoutedEventArgs e)
        {
            packageHelper.Apps.GetFilteredApps("InstalledNewest");
        }

        private void InstalledOldest_Tapped(object sender, TappedRoutedEventArgs e)
        {
            packageHelper.Apps.GetFilteredApps("InstalledOldest");
        }



        private void ReScan_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _ = packageHelper.RescanForNewApplications().ConfigureAwait(true);
        }

        private void GridViewMain_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int a = e.GetCurrentPoint((UIElement)sender).Properties.MouseWheelDelta;
            if (a > 0)
            {
                if (packageHelper.pageVariables.IsNext)
                {
                    GlobalVariables.SetPageNumber(GlobalVariables.pagenum + 1);
                }

            }
            else
            {
                if (packageHelper.pageVariables.IsPrevious)
                {
                    GlobalVariables.SetPageNumber(GlobalVariables.pagenum - 1);
                }
            }
        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            Point startpoint = e.GetPosition(GridViewMain);

            GeneralTransform a = GridViewMain.TransformToVisual(GridViewMain);
            Point b = a.TransformPoint(new Point(0, 0));
            if (startpoint.X < (b.X + 15))
            {
                if (packageHelper.pageVariables.IsPrevious)
                {
                    GlobalVariables.SetPageNumber(GlobalVariables.pagenum - 1);
                }
            }
            else if (startpoint.X > (b.X + d.ActualWidth - 70))
            {
                if (packageHelper.pageVariables.IsNext)
                {
                    GlobalVariables.SetPageNumber(GlobalVariables.pagenum + 1);
                }
            }
            DelayDragOver(2000);


        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            GlobalVariables.isdragging = true;
            GlobalVariables.itemdragged.initialindex = packageHelper.Apps.IndexOf((Apps)e.Items[0]);

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
            int indexx = Math.Min(view.Items.Count - 1, (int)(pos.Y / itemHeight));
            int indexy = Math.Min(view.Items.Count - 1, (int)(pos.X / itemwidth));
            AppPaginationObservableCollection t = (AppPaginationObservableCollection)view.ItemsSource;
            int listindex = (indexx * (GlobalVariables.columns)) + (indexy);
            int moveto = 0;
            if (listindex >= t.Count() - 1)
            {
                moveto = (GlobalVariables.pagenum * GlobalVariables.appsperscreen) + listindex;
                if (moveto >= packageHelper.Apps.GetOriginalCollection().Count() - 1)
                {
                    moveto = packageHelper.Apps.GetOriginalCollection().Count() - 1;
                }
            }
            if (listindex <= t.Count() - 1)
            {
                moveto = (GlobalVariables.pagenum * GlobalVariables.appsperscreen) + listindex;
            }
            packageHelper.Apps.MoveApp(GlobalVariables.itemdragged.initialindex, moveto);

        }

        private async void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            Apps fi = (Apps)e.ClickedItem;
            await packageHelper.LaunchApp(fi.FullName);
        }

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            disableScrollViewer(GridViewMain);
        }

        private void About_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}