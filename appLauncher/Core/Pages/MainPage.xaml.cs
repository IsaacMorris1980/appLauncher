// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;
using Windows.System.Threading;
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

        public static bool firstrun
        { get; set; } = true;
        private string _appfullname;
        public int Maxicons { get; set; }
        public bool buttonssetup = false;
        private ObservableCollection<IApporFolder> _folders;

        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // seconds;
        double imageTimeLeft = 0;
        TimeSpan updateImageTimerLength = SettingsHelper.totalAppSettings.ImageRotationTime;
        public byte[] backImages = Encoding.ASCII.GetBytes(Colors.Orange.ToString());
        private Ellipse oldAnimatedEllipse;
        private Ellipse ellipseToAnimate;
        DispatcherTimer imageTimer;
        private int previousSelectedIndex = 0;

        private bool isPageLoaded = false;
        ThreadPoolTimer threadPoolTimer;
        private bool isnetworkstatuschangedregistered;

        public static int _appsPerScreen { get; set; }
        public DraggedItem _Itemdragged { get; set; } = new DraggedItem();
        private int _columns { get; set; }

        private int _rows { get; set; }

        public int _pageNum { get; set; }
        public int _numOfPages { get; set; }
        public bool _isDragging { get; set; }
        public Point _startingPoint { get; set; }
        public static event PageChangedDelegate pageChanged;
        public static event PageNumChangedDelegate numofPagesChanged;
        public static event PageSizeChangedDelegate pageSizeChanged;
        public static GridView gridView { get; set; }




        public static async Task LoggingCrashesAsync(Exception crashToStore)
        {
            Debug.WriteLine(crashToStore.ToString());
            StorageFile errorFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("errors.json", CreationCollisionOption.OpenIfExists);
            string errorStr = crashToStore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.AppendTextAsync(errorFile, errorStr);
        } 
        public static InAppNotification messageNotification = new InAppNotification();
        public MainPage()
        {
            try
            {
                // pageChanged += new PageChangedDelegate(UpdateIndicator);
                //     numofPagesChanged += new PageNumChangedDelegate(SetupPageIndicators);
                pageSizeChanged += MainPage_pageSizeChanged;
                this.InitializeComponent();
                this.SizeChanged += MainPage_SizeChanged;
              //  sizeChangeTimer.Tick += SizeChangeTimer_Tick;
                sizeChangeTimer.Interval = new TimeSpan(0, 0, 1);

                //   this.listView.SelectionChanged += ListView_SelectionChanged;
                firstrun = true;
            }
            catch (Exception es)
            {
                LoggingCrashesAsync(es).ConfigureAwait(false);
            }
        }

        private void MainPage_pageSizeChanged(PageSizeEventArgs e)
        {
            if (e.AppPageSize != SettingsHelper.totalAppSettings.AppsPerPage)
            {
                SettingsHelper.totalAppSettings.AppsPerPage = e.AppPageSize;
            }
        }



        //private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Debug.WriteLine($"Previous Selected: {previousSelectedIndex}");
        //    Debug.WriteLine($"Current Selected: {listView.SelectedIndex}");
        //    if (listView.SelectedItem == null || listView.SelectedIndex == -1)
        //    {
        //        return;
        //    }
        //    var item = listView.SelectedItem;
        //    Calculations relative to screen or ListView
        //    FrameworkElement listViewItem = (FrameworkElement)listView.ContainerFromItem(item);
        //    if (listViewItem == null)
        //    {
        //        listView.ScrollIntoView(item);
        //    }
        //    listView.ScrollIntoView(listView.Items[listView.SelectedIndex]);
        //    previousSelectedIndex = listView.SelectedIndex;
        //    pageChanged?.Invoke(new PageChangedEventArgs(listView.SelectedIndex));
        //}

        public void UpdateIndicator(PageChangedEventArgs e)
        {
            PackageHelper.pageVariables.IsPrevious = e.PageIndex > 0;
            PackageHelper.pageVariables.IsNext = e.PageIndex < _numOfPages - 1;
            _pageNum = e.PageIndex;
            Debug.WriteLine(e.PageIndex);
            Debug.WriteLine(_pageNum);
            //AdjustIndicatorStackPanel(e.PageIndex);

        }
        public void Search(string textToFind)
        {
            List<IApporFolder> searchresults = PackageHelper.Apps.GetOriginalCollection().Where(x => x.Name.ToLower().Contains(textToFind.ToLower())).ToList();
        }

        // Updates grid of apps only when a bit of time has passed after changing the size of the window.
        // Better than doing this inside the the flip view item template since you don't have a timer that's always running anymore.
        //private void SizeChangeTimer_Tick(object sender, object e)
        //{

        //    try
        //    {

        //        sizeChangeTimer.Stop();
        //        GridViewMain.Width = this.ActualWidth;
        //        GridViewMain.Height = this.ActualHeight;

        //        _columns = NumofRoworColumn(94, (int)GridViewMain.Width);
        //        _rows = NumofRoworColumn(108, (int)GridViewMain.Height);
        //        Debug.WriteLine($"Columns: {_columns}");
        //        Debug.WriteLine($"Rows: {_rows}");
        //        _appsPerScreen = (NumofRoworColumn(108, (int)GridViewMain.Height) * NumofRoworColumn(94, (int)GridViewMain.Width));
        //        Debug.WriteLine(_appsPerScreen);
        //        int additionalPagesToMake = calculateExtraPages(_appsPerScreen) - 1;
        //        additionalPagesToMake += PackageHelper.Apps.GetOriginalCollection().Count - (additionalPagesToMake * _appsPerScreen) > 0 ? 1 : 0;
        //        if (additionalPagesToMake > 0)
        //        {
        //            SettingsHelper.totalAppSettings.LastPageNumber = (SettingsHelper.totalAppSettings.LastPageNumber > (additionalPagesToMake - 1)) ? (additionalPagesToMake - 1) : SettingsHelper.totalAppSettings.LastPageNumber;
        //            //SetupPageIndicators(new PageNumChangedArgs(additionalPagesToMake));

        //            numofPagesChanged?.Invoke(new PageNumChangedArgs(additionalPagesToMake));
        //            pageSizeChanged?.Invoke(new PageSizeEventArgs(_appsPerScreen));

        //        }



        //        //    AdjustIndicatorStackPanel(SettingsHelper.totalAppSettings.LastPageNumber);
        //        previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
        //        _pageNum = SettingsHelper.totalAppSettings.LastPageNumber;


        //        threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
        //             {
        //                 //
        //                 // Update the UI thread by using the UI core dispatcher.
        //                 //
        //                 await Dispatcher.RunAsync(CoreDispatcherPriority.High,
        //                     agileCallback: () =>
        //                     {
        //                         this.Background = ImageHelper.GetBackbrush;

        //                         GC.WaitForPendingFinalizers();

        //                     });

        //             }
        //                 , SettingsHelper.totalAppSettings.ImageRotationTime);
        //        //GlobalVariables._pageNum = (SettingsHelper.totalAppSettings.LastPageNumber);
        //    }
        //    catch (Exception es)
        //    {
        //        LoggingCrashesAsync(es).ConfigureAwait(false);
        //    }

        //}
        public static int NumofRoworColumn(int objectSize, int sizeToFit)
        {
            int amount = 0;
            int size = objectSize;
            while (size <= sizeToFit)
            {
                amount += 1;
                size += objectSize;
            }
            return amount;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //sizeChangeTimer.Stop();
            //sizeChangeTimer.Start();


        }

        public async void DelayDragOver(int timetopause)
        {
            await Task.Delay(timetopause);
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


        }
        private void disableScrollViewer(GridView gridView)
        {
            //try
            //{
            //    var border = (Border)VisualTreeHelper.GetChild(gridView, 0);
            //    var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            //    scrollViewer.IsVerticalRailEnabled = false;
            //    scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            //    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            //}
            //catch (Exception es)
            //{
            //    LoggingCrashesAsync(es).ConfigureAwait(false);
            //}
        }

        /// <summary>
        /// Returns result of calculation of extra pages needed to be added.
        /// </summary>
        /// <param name="appsPerScreen"></param>
        /// <returns></returns>
        private int calculateExtraPages(int appsPerScreen)
        {
            double appsPerScreenAsDouble = appsPerScreen;
            double numberOfApps = PackageHelper.Apps.GetOriginalCollection().Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
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
                if (child != null && child is T t)
                {
                    return t;
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
            Frame.Navigate(typeof(FirstPage));

        }
        private void SearchField_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                if (PackageHelper.Search.Count > 0)
                {
                    sender.ItemsSource = PackageHelper.Search.Where(p => p.Name.ToLower().Contains(((AutoSuggestBox)sender).Text.ToLower())).ToList();

                }
            }
        }

        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            FinalTiles ap = (FinalTiles)args.SelectedItem;
            PackageHelper.LaunchApp(ap.FullName).ConfigureAwait(false);

            sender.ItemsSource = PackageHelper.Search;
            sender.Text = String.Empty;
        }

        private void PreviousPage_Tapped(object sender, TappedRoutedEventArgs e)

        {
            Debug.WriteLine(_pageNum);
            if (PackageHelper.pageVariables.IsPrevious)
            {
                pageChanged?.Invoke(new PageChangedEventArgs(_pageNum - 1));
            }
        }

        private void NextPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine(_pageNum);
            if (PackageHelper.pageVariables.IsNext)
            {
                pageChanged?.Invoke(new PageChangedEventArgs(_pageNum + 1));
            }
        }

        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((AppBarButton)sender).ContextFlyout.ShowAt((AppBarButton)sender);
        }

        private void AlphaAZ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps("AppAZ");
        }

        private void AlphaZA_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps("AppZA");
        }

        private void DevAZ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps("DevAZ");
        }

        private void DevZA_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps("DevZA");
        }

        private void InstalledNewest_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps("InstalledNewest");
        }

        private void InstalledOldest_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps("InstalledOldest");
        }



        private void ReScan_Tapped(object sender, TappedRoutedEventArgs e)
        {
           PackageHelper.RescanForNewApplications();
        }

        private void GridViewMain_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int a = e.GetCurrentPoint((UIElement)sender).Properties.MouseWheelDelta;
            if (a > 0)
            {
                if (PackageHelper.pageVariables.IsNext)
                {
                    pageChanged?.Invoke(new PageChangedEventArgs(_pageNum + 1));
                }
            }
            else
            {
                if (PackageHelper.pageVariables.IsPrevious)
                {
                    pageChanged?.Invoke(new PageChangedEventArgs(_pageNum - 1));
                }
            }
        }

        private async void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            //await Task.Delay(3000);
            //GridView d = (GridView)sender;
            //e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            //Point startpoint = e.GetPosition(GridViewMain);
            //GeneralTransform a = GridViewMain.TransformToVisual(GridViewMain);
            //Point b = a.TransformPoint(new Point(0, 0));
            //if (startpoint.X < (b.X + 15))
            //{
            //    if (PackageHelper.pageVariables.IsPrevious)
            //    {
            //        pageChanged?.Invoke(new PageChangedEventArgs(_pageNum - 1));
            //    }
            //}
            //else if (startpoint.X > (b.X + d.ActualWidth - 70))
            //{
            //    if (PackageHelper.pageVariables.IsNext)
            //    {
            //        pageChanged?.Invoke(new PageChangedEventArgs(_pageNum + 1));
            //        e.Handled = true;
            //        await Task.Delay(5000);
            //    }
            //}
            //DelayDragOver(5000);
            await Task.Delay(1500);
        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _isDragging = true;
            _Itemdragged.InitialIndex = PackageHelper.Apps.IndexOf((IApporFolder)e.Items[0]);
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
            int listindex = (indexx * (_columns)) + (indexy);
            int moveto = 0;
            if (listindex >= t.Count() - 1)
            {
                moveto = (_pageNum * _appsPerScreen) + listindex;
                if (moveto >= PackageHelper.Apps.Count() - 1)
                {
                    //      moveto = PackageHelper.Apps.Count() - 1;
                }
            }
            if (listindex <= t.Count() - 1)
            {
                moveto = (_pageNum * _appsPerScreen) + listindex;
            }
            //  PackageHelper.Apps.MoveApp(_Itemdragged.InitialIndex, moveto);
        }

        private async void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            IApporFolder selecteditem = (IApporFolder)e.ClickedItem;
            if (selecteditem.GetType() == typeof(AppFolder))
            {
                Frame.Navigate(typeof(FolderView), selecteditem);
            }
            else
            {
                ((FinalTiles)selecteditem).LaunchedCount += 1;
                await PackageHelper.LaunchApp(((FinalTiles)selecteditem).FullName);
            }
        }

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
         //   disableScrollViewer(GridViewMain);
        }






        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var a = e.ClickedItem as Ellipse;

        }




        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StackPanel st = new StackPanel()
            {
                Or
            }

        }

        private void rightclickmenuitem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AppInformation), _appfullname);
        }



        private void Favorites_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<FinalTiles> favorites = PackageHelper.Apps.OfType<FinalTiles>().Where(x => x.Favorite == true).ToList();
            if (favorites.Count() <= 0)
            {
                return;
            }
            var appfolder = new AppFolder();
            appfolder.Name = "Favorites";
            appfolder.Description = "All Favorites";
            foreach (var item in favorites)
            {
                appfolder.FolderApps.Add(item);
            }
            ObservableCollection<IApporFolder> allitems = PackageHelper.Apps.GetOriginalCollection();
            PackageHelper.Apps.Insert(0, appfolder);
            Frame.Navigate(typeof(MainPage));
        }



        private void GridViewMain_Loaded(object sender, RoutedEventArgs e)
        {
            //GridViewMain.Width = this.ActualWidth;
            //GridViewMain.Height = this.Height;
            //Debug.WriteLine($"amount of tiles {PackageHelper.Apps.GetOriginalCollection().Count()}");

            //_columns = NumofRoworColumn(94, (int)GridViewMain.ActualWidth);
            //_rows = NumofRoworColumn(108, (int)GridViewMain.ActualHeight);
            //Debug.WriteLine($"Columns: {_columns}");
            //Debug.WriteLine($"Rows: {_rows}");
            //var a = this.ActualHeight;
            //var b = this.ActualWidth;
            //_appsPerScreen = _columns * _rows;
            //Debug.WriteLine(_appsPerScreen);
            //int additionalPagesToMake = calculateExtraPages(_appsPerScreen) - 1;
            //additionalPagesToMake += PackageHelper.Apps.GetOriginalCollection().Count - (additionalPagesToMake * _appsPerScreen) > 0 ? 1 : 0;
            //if (additionalPagesToMake > 0)
            //{
            //    SettingsHelper.totalAppSettings.LastPageNumber = (SettingsHelper.totalAppSettings.LastPageNumber > (additionalPagesToMake)) ? (additionalPagesToMake - 1) : SettingsHelper.totalAppSettings.LastPageNumber;
            //    numofPagesChanged?.Invoke(new PageNumChangedArgs(additionalPagesToMake));
            //    pageSizeChanged?.Invoke(new PageSizeEventArgs(_appsPerScreen));
            //    PackageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
            //    PackageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < _numOfPages - 1;
            //}
            //previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
            //_pageNum = SettingsHelper.totalAppSettings.LastPageNumber;
            //this.Background = ImageHelper.GetBackbrush;
            //threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            //{
            //    await Dispatcher.RunAsync(CoreDispatcherPriority.High,
            //       agileCallback: () =>
            //       {
            //           this.Background = ImageHelper.GetBackbrush;

            //           GC.WaitForPendingFinalizers();
            //       });
            //}
            //         , SettingsHelper.totalAppSettings.ImageRotationTime);
            //GC.WaitForPendingFinalizers();
        }

        private void Edit_Tapped(object sender, TappedRoutedEventArgs e)
        {


            object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item != null)
            {
                if (item.GetType() == typeof(FinalTiles))
                {
                    Frame.Navigate(typeof(EditApp), ((FinalTiles)item));
                    return;
                }
                if (item.GetType() == typeof(AppFolder))
                {
                    Frame.Navigate(typeof(EditFolder), ((AppFolder)item));
                    return;
                }


            }
        }

        private void Info_Tapped(object sender, TappedRoutedEventArgs e)
        {


            object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item != null)
            {

                if (item.GetType() == typeof(FinalTiles))
                {
                    Frame.Navigate(typeof(AppInformation), ((FinalTiles)item));
                    return;

                }
                if (item.GetType() == typeof(AppFolder))
                {
                    Frame.Navigate(typeof(FolderInfo), ((AppFolder)item));
                    return;
                }


            }
        }
    }
}