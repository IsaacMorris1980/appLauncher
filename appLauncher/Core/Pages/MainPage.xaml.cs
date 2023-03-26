// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.UI.Animations;

using System;
using System.Linq;

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
        public int Maxicons { get; private set; }

        public CoreDispatcher coredispatcher;
        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // milliseconds;
        TimeSpan timeSpan = TimeSpan.FromSeconds(15);
        DispatcherTimer dispatching = new DispatcherTimer();
        private Button oldAnimatedButton;
        private Button buttontoanimate;
        private Ellipse ellipseToAnimate;

        private int selectedIndex;
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

            if (listView.SelectedItem == null)
            {
                return;
            }

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
            screensContainerFlipView.SelectedIndex = listView.SelectedIndex;

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
                        packageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
                        packageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < GlobalVariables.numOfPages - 1;

                        for (int i = 0; i < additionalPagesToMake; i++)
                        {
                            screensContainerFlipView.Items.Add(i);
                        }
                    }
                    SearchField.ItemsSource = packageHelper.searchApps.ToList();

                    GlobalVariables.SetPageNumber(SettingsHelper.totalAppSettings.LastPageNumber);
                    previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
                    //this.InvalidateArrange();

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





        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.Scale(2f, 2f, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 0).StartAsync();
            await this.Scale(1, 1, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 300).StartAsync();
        }
        private void SetupPageIndicators(int e)
        {

            for (int i = 0; i < e; i++)
            {
                Button btn = new Button();
                btn.Tag = i;
                btn.Background = new SolidColorBrush(Colors.Transparent);
                Ellipse el = new Ellipse
                {
                    Height = 8,
                    Width = 8,
                    Margin = new Thickness(12),
                    Fill = new SolidColorBrush(Colors.Gray)
                };
                btn.Tapped += Btn_Tapped;
                btn.Content = el;
                listView.Items.Add(btn);
            }

            //flipViewIndicatorStackPanel.Children.Clear();

            //for (int i = 0; i < e; i++)
            //{
            //    Button btn = new Button
            //    {
            //        Margin = new Thickness { Bottom = 0, Top = 0, Left = 12, Right = 12 },
            //        Width = 30,
            //        Height = 30,
            //        Tag = i,
            //        Content = new Ellipse
            //        {
            //            Width = 8,
            //            Height = 8,
            //            Fill = new SolidColorBrush(Colors.Gray)
            //        },
            //    };

            //    btn.Tapped += Btn_Tapped; ;
            //    flipViewIndicatorStackPanel.Children.Add(btn);
            //}
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

            await ImageHelper.LoadBackgroundImages();
            this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) ? GlobalVariables.pagenum : 0;
            maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)screensContainerFlipView.ActualHeight);
            maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)screensContainerFlipView.ActualWidth);
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
                SetupPageIndicators(additionalPagesToMake);
                GlobalVariables.SetNumOfPages(additionalPagesToMake);
                packageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
                packageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < GlobalVariables.numOfPages - 1;
                for (int i = 0; i < additionalPagesToMake; i++)
                {
                    screensContainerFlipView.Items.Add(i);

                }



                screensContainerFlipView.SelectionChanged += FlipViewMain_SelectionChanged;



            }
            previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
            GlobalVariables.SetPageNumber(SettingsHelper.totalAppSettings.LastPageNumber);
            screensContainerFlipView.SelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;

            SearchField.ItemsSource = packageHelper.searchApps.ToList();
        }

        private void Items_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            var collection = sender;
            int count = collection.Count;

            //flipViewIndicatorStackPanel.Children.Clear();

            //for (int i = 0; i < count; i++)
            //{
            //    flipViewIndicatorStackPanel.Children.Add(new Ellipse
            //    {
            //        Width = 8,
            //        Height = 8,
            //        Fill = new SolidColorBrush(Colors.Gray),
            //        Margin = new Thickness(4, 0, 4, 0)
            //    });

            //};
            //flipViewIndicatorStackPanel.Children.Clear();

            //for (int i = 0; i < count; i++)
            //{
            //    Button btn = new Button
            //    {
            //        Margin = new Thickness { Bottom = 0, Top = 0, Left = 12, Right = 12 },
            //        Width = 30,
            //        Height = 30,
            //        Tag = i,
            //        Content = new Ellipse
            //        {
            //            Width = 8,
            //            Height = 8,
            //            Fill = new SolidColorBrush(Colors.Gray)
            //        },
            //    };

            //    btn.Tapped += Btn_Tapped; ;
            //    flipViewIndicatorStackPanel.Children.Add(btn);
            // }
            //AdjustIndicatorStackPanel(GlobalVariables.pagenum);
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
                        ellipseToAnimate = (Ellipse)buttontoanimate.Content;
                        ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                        olderellipse.RenderTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1 };
                        ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                        olderellipse.Fill = new SolidColorBrush(Colors.Gray);
                        oldAnimatedButton = buttontoanimate;
                    }
                    else
                    {
                        buttontoanimate = (Button)listView.Items[selectedIndex];
                        ellipseToAnimate = (Ellipse)buttontoanimate.Content; //FindFirstElementInVisualTree<Ellipse>(buttontoanimate);
                        ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                        ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                        oldAnimatedButton = buttontoanimate;

                    }
                    listView.SelectedIndex = selectedIndex;
                }
                //if (!firstrun)
                //{


                //    StackPanel indicator = flipViewIndicatorStackPanel;

                //    Ellipse ellipseToAnimate = new Ellipse();

                //    Button buttontoanimate = new Button();
                //    if (oldAnimatedButton != null)
                //    {
                //        Button oldbutton = null;

                //        oldbutton = oldAnimatedButton;
                //        Ellipse olderellipse = (Ellipse)oldbutton.Content;
                //        buttontoanimate = (Button)indicator.Children[selectedIndex];
                //        var c = indicator.Children;
                //        var d = c[0];
                //        ellipseToAnimate = (Ellipse)buttontoanimate.Content;
                //        ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                //        olderellipse.RenderTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1 };
                //        ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                //        olderellipse.Fill = new SolidColorBrush(Colors.Gray);
                //        oldAnimatedButton = buttontoanimate;
                //    }
                //    else
                //    {
                //        buttontoanimate = (Button)flipViewIndicatorStackPanel.Children[selectedIndex];
                //        ellipseToAnimate = (Ellipse)buttontoanimate.Content; //FindFirstElementInVisualTree<Ellipse>(buttontoanimate);
                //        ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                //        ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                //        oldAnimatedButton = buttontoanimate;

                //    }
                //}
            }
            catch (Exception e)
            {
                Analytics.TrackEvent("Crashed during updating selected page");
                Crashes.TrackError(e);
            }

            //var indicator = flipViewIndicatorStackPanel;
            //Ellipse ellipseToAnimate = new Ellipse();
            //for (int i = 0; i < indicator.Children.Count; i++)
            //{
            //    if (i == selectedIndex)
            //    {
            //        var ellipse = (Ellipse)indicator.Children[i];
            //        ellipseToAnimate = ellipse;
            //        ellipse.Fill = new SolidColorBrush(Colors.Orange);

            //    }
            //    else
            //    {
            //        var ellipse = (Ellipse)indicator.Children[i];
            //        ellipse.Fill = (SolidColorBrush)App.Current.Resources["DefaultTextForegroundThemeBrush"];

            //    }
            //}
            //float centerX = (float)ellipseToAnimate.ActualWidth / 2;
            //float centerY = (float)ellipseToAnimate.ActualHeight / 2;
            //float animationScale = 1.7f;

            //double duration = 300;
            //if (IndicatorAnimation.oldAnimatedEllipse != null)
            //{
            //    await Task.WhenAll(ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration, easingType: EasingType.Back).StartAsync(),
            //        IndicatorAnimation.oldAnimatedEllipse.Scale(1, 1, centerX, centerY, duration, easingType: EasingType.Back).StartAsync());

            //}
            //else
            //{
            //    await ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration, easingType: EasingType.Bounce).StartAsync();
            //}

            //IndicatorAnimation.oldAnimatedEllipse = ellipseToAnimate;
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

        //private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    string selected = ((ComboBoxItem)Filterby.SelectedItem).Content.ToString();
        //    List<Apps> orderlist;
        //    ObservableCollection<Apps> newordercollection = new ObservableCollection<Apps>();
        //    switch (selected)
        //    {
        //        case "AtoZ":
        //            orderlist = packageHelper.Apps.OrderBy(y => y.Name).ToList();
        //            for (int i = 0; i < orderlist.Count - 1; i++)
        //            {
        //                var a = orderlist[i];
        //                var c = packageHelper.Apps.IndexOf(a);
        //                packageHelper.Apps.Move(c, i);
        //            }
        //            break;
        //        case "Developer":
        //            orderlist = packageHelper.Apps.OrderBy(x => x.Developer).ToList();
        //            for (int i = 0; i < orderlist.Count - 1; i++)
        //            {
        //                var a = orderlist[i];
        //                var c = packageHelper.Apps.IndexOf(a);
        //                packageHelper.Apps.Move(c, i);
        //            }
        //            break;

        //        case "Installed":
        //            orderlist = packageHelper.Apps.OrderBy(x => x.InstalledDate).ToList();
        //            for (int i = 0; i < orderlist.Count - 1; i++)
        //            {
        //                var a = orderlist[i];
        //                var c = packageHelper.Apps.IndexOf(a);
        //                packageHelper.Apps.Move(c, i);
        //            }
        //            break;

        //        default:
        //            return;

        //    }
        //    this.Frame.Navigate(typeof(MainPage));
        //}
        private void FlipViewMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //if (e.AddedItems.Count > 0)
            //{
            //    var flipViewItem = screensContainerFlipView.ContainerFromIndex(screensContainerFlipView.SelectedIndex);
            //    appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
            //    userControl.SwitchedToThisPage();
            //}
            //if (e.RemovedItems.Count > 0)
            //{
            //    var flipViewItem = screensContainerFlipView.ContainerFromItem(e.RemovedItems[0]);
            //    appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
            //    userControl.SwitchedFromThisPage();
            //}
            GlobalVariables.SetPageNumber(((FlipView)sender).SelectedIndex);
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

        private void RefreshApps_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void ReScan_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _ = packageHelper.RescanForNewApplications().ConfigureAwait(true);
        }
    }
}

