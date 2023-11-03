// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.UI.Animations;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
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
        private int maxRows;
        private int maxColumns;
        private NetworkStatusChangedEventHandler networkstatuschangedhandler;
        bool firstrun { get; set; } = true;
        public int Maxicons { get; set; }
        public bool buttonssetup = false;
        public BackgroundWorker UDPServer;
        public BackgroundWorker TCPServer;
        public BackgroundWorker UDPClient;
        public BackgroundWorker TCPClient;

        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // seconds;
        double imageTimeLeft = 0;
        TimeSpan updateImageTimerLength = SettingsHelper.totalAppSettings.ImageRotationTime;

        private Ellipse oldAnimatedEllipse;
        private Ellipse ellipseToAnimate;
        DispatcherTimer imageTimer;
        private int previousSelectedIndex = 0;

        private bool isPageLoaded = false;
        ThreadPoolTimer threadPoolTimer;
        private bool isnetworkstatuschangedregistered;

        public int _appsPerScreen { get; set; }
        public DraggedItem _Itemdragged { get; set; } = new DraggedItem();
        public int _columns { get; set; }
        public int _pageNum { get; set; }
        public int _numOfPages { get; set; }
        public bool _isDragging { get; set; }
        public Point _startingPoint { get; set; }
        public static event PageChangedDelegate pageChanged;
        public static event PageNumChangedDelegate numofPagesChanged;
        public static event PageSizeChangedDelegate pageSizeChanged;

        public static async Task LoggingCrashesAsync(Exception crashToStore)
        {
            Debug.WriteLine(crashToStore.ToString());
            StorageFile errorFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("errors.json", CreationCollisionOption.OpenIfExists);
            string errorStr = crashToStore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.AppendTextAsync(errorFile, errorStr);
        }

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
        /// <summary>
        /// Runs when a new instance of MainPage is created
        /// </summary>
        public MainPage()
        {
            try
            {
                pageChanged += new PageChangedDelegate(UpdateIndicator);
                numofPagesChanged += new PageNumChangedDelegate(SetupPageIndicators);
                pageSizeChanged += MainPage_pageSizeChanged;
                this.InitializeComponent();
                this.SizeChanged += MainPage_SizeChanged;
                sizeChangeTimer.Tick += SizeChangeTimer_Tick;
                this.listView.SelectionChanged += ListView_SelectionChanged;
                firstrun = true;
                PackageHelper.AppsRetreived += PackageHelper_AppsRetreived1;
            }
            catch (Exception es)
            {

            }

        }

        private void MainPage_pageSizeChanged(PageSizeEventArgs e)
        {
            _appsPerScreen = e.AppPageSize;
        }

        private void PackageHelper_AppsRetreived1(object sender, EventArgs e)
        {
            RingofProgress.IsActive = false;
            this.FindName("GridViewMain");
            GridViewMain.ItemsSource = PackageHelper.Apps;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"Previous Selected: {previousSelectedIndex}");
            Debug.WriteLine($"Current Selected: {listView.SelectedIndex}");
            if (listView.SelectedItem == null || listView.SelectedIndex == -1)
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

            listView.ScrollIntoView(listView.Items[listView.SelectedIndex]);
            previousSelectedIndex = listView.SelectedIndex;
            pageChanged?.Invoke(new PageChangedEventArgs(listView.SelectedIndex));

        }

        public void UpdateIndicator(PageChangedEventArgs e)
        {
            PackageHelper.pageVariables.IsPrevious = e.PageIndex > 0;
            PackageHelper.pageVariables.IsNext = e.PageIndex < _numOfPages - 1;
            _pageNum = e.PageIndex;
            Debug.WriteLine(e.PageIndex);
            Debug.WriteLine(_pageNum);
            AdjustIndicatorStackPanel(e.PageIndex);
        }

        // Updates grid of apps only when a bit of time has passed after changing the size of the window.
        // Better than doing this inside the the flip view item template since you don't have a timer that's always running anymore.
        private async void SizeChangeTimer_Tick(object sender, object e)
        {
            try
            {
                if (currentTimeLeft == 0)
                {
                    currentTimeLeft = 0;
                    sizeChangeTimer.Stop();

                    await ImageHelper.LoadBackgroundImages();


                    PackageHelper.SearchApps = (await PackageHelper.GetApps()).OrderBy(x => x.Name).ToList();

                    SearchField.ItemsSource = PackageHelper.SearchApps;

                    _columns = NumofRoworColumn(84, (int)GridViewMain.ActualWidth);
                    pageSizeChanged?.Invoke(new PageSizeEventArgs(NumofRoworColumn(108, (int)GridViewMain.ActualHeight) * NumofRoworColumn(84, (int)GridViewMain.ActualWidth)));
                    int additionalPagesToMake = calculateExtraPages(_appsPerScreen) - 1;
                    additionalPagesToMake += (PackageHelper.Apps.GetOriginalCollection().Count - (additionalPagesToMake * _appsPerScreen)) > 0 ? 1 : 0;
                    if (additionalPagesToMake > 0)
                    {
                        SettingsHelper.totalAppSettings.LastPageNumber = (SettingsHelper.totalAppSettings.LastPageNumber > (additionalPagesToMake - 1)) ? (additionalPagesToMake - 1) : SettingsHelper.totalAppSettings.LastPageNumber;
                        Maxicons = additionalPagesToMake;
                        // SetupPageIndicators(new PageNumChangedArgs(additionalPagesToMake));

                        numofPagesChanged?.Invoke(new PageNumChangedArgs(additionalPagesToMake));
                        PackageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
                        PackageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < _numOfPages - 1;
                    }



                    //    AdjustIndicatorStackPanel(SettingsHelper.totalAppSettings.LastPageNumber);
                    previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
                    _pageNum = SettingsHelper.totalAppSettings.LastPageNumber;
                }
                else
                {
                    currentTimeLeft -= (int)sizeChangeTimer.Interval.TotalMilliseconds;
                }
                threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                     {
                         //
                         // Update the UI thread by using the UI core dispatcher.
                         //
                         await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                             agileCallback: () =>
                             {
                                 AppPage.Background = ImageHelper.GetBackbrush;

                                 GC.Collect();
                             });
                     }
                         , SettingsHelper.totalAppSettings.ImageRotationTime);
                //GlobalVariables._pageNum = (SettingsHelper.totalAppSettings.LastPageNumber);
            }
            catch (Exception es)
            {

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

            }
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
        private void SetupPageIndicators(PageNumChangedArgs e)
        {
            _numOfPages = e.numofpages;
            listView.Items.Clear();
            for (int i = 0; i < e.numofpages; i++)
            {
                Ellipse el = new Ellipse
                {
                    Tag = i,
                    Height = 8,
                    Width = 8,
                    Margin = new Thickness(12),
                    Fill = new SolidColorBrush(Colors.Gray),

                };

                ToolTipService.SetToolTip(el, $"Page {i + 1}");
                listView.Items.Add(el);
            }
            buttonssetup = true;
        }

        private void Btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int a = (int)btn.Tag;
            pageChanged?.Invoke(new PageChangedEventArgs(a));
        }
        public void SetPageSize(int number)
        {
            pageSizeChanged?.Invoke(new PageSizeEventArgs(number));
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
            await PackageHelper.LoadCollectionAsync();



            await ImageHelper.LoadBackgroundImages();


            PackageHelper.SearchApps = (await PackageHelper.GetApps()).OrderBy(x => x.Name).ToList();

            UDPServer = new BackgroundWorker();
            UDPClient = new BackgroundWorker();
            TCPServer = new BackgroundWorker();
            TCPClient = new BackgroundWorker();
            OnNetworkStatusChange(new object());
            NetworkStatusChange();
            _columns = NumofRoworColumn(84, (int)GridViewMain.ActualWidth);
            _appsPerScreen = (NumofRoworColumn(108, (int)GridViewMain.ActualHeight) *
             NumofRoworColumn(84, (int)GridViewMain.ActualWidth));
            int additionalPagesToMake = calculateExtraPages(_appsPerScreen) - 1;
            additionalPagesToMake += (PackageHelper.Apps.GetOriginalCollection().Count - (additionalPagesToMake * _appsPerScreen)) > 0 ? 1 : 0;
            if (additionalPagesToMake > 0)
            {
                SettingsHelper.totalAppSettings.LastPageNumber = (SettingsHelper.totalAppSettings.LastPageNumber > (additionalPagesToMake - 1)) ? (additionalPagesToMake - 1) : SettingsHelper.totalAppSettings.LastPageNumber;
                Maxicons = additionalPagesToMake;
                numofPagesChanged?.Invoke(new PageNumChangedArgs(additionalPagesToMake));
                PackageHelper.pageVariables.IsPrevious = SettingsHelper.totalAppSettings.LastPageNumber > 0;
                PackageHelper.pageVariables.IsNext = SettingsHelper.totalAppSettings.LastPageNumber < _numOfPages - 1;
            }

            previousSelectedIndex = SettingsHelper.totalAppSettings.LastPageNumber;
            pageChanged?.Invoke(new PageChangedEventArgs(SettingsHelper.totalAppSettings.LastPageNumber));
            AdjustIndicatorStackPanel(SettingsHelper.totalAppSettings.LastPageNumber);
            threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                 {
                     //
                     // Update the UI thread by using the UI core dispatcher.
                     //
                     await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                         agileCallback: () =>
                         {
                             AppPage.Background = ImageHelper.GetBackbrush;

                             GC.Collect();
                         });
                 }
                     , SettingsHelper.totalAppSettings.ImageRotationTime);

        }
        private async void disableScrollViewer(GridView gridView)
        {
            try
            {
                var border = (Border)VisualTreeHelper.GetChild(gridView, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.IsVerticalRailEnabled = false;
                scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            catch (Exception es)
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
            double numberOfApps = PackageHelper.Apps.GetOriginalCollection().Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }

        private void AdjustIndicatorStackPanel(int selectedIndex)
        {
            try
            {
                if (!buttonssetup)
                {
                    return;
                }
                if (!firstrun)
                {
                    if (oldAnimatedEllipse != null)
                    {
                        Ellipse oldellipse = null;
                        Ellipse es = new Ellipse();
                        es.Fill = new SolidColorBrush(Colors.Gray);
                        es.Tag = "test";
                        es.Margin = new Thickness(20);
                        es.Tapped += OnTapped;

                        oldellipse = oldAnimatedEllipse;

                        ellipseToAnimate = (Ellipse)listView.Items[selectedIndex];
                        if (oldAnimatedEllipse != ellipseToAnimate && ellipseToAnimate != null)
                        {

                            ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                            oldellipse.RenderTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1 };
                            ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                            oldellipse.Fill = new SolidColorBrush(Colors.Gray);
                            oldAnimatedEllipse = ellipseToAnimate;
                        }
                    }
                    else
                    {
                        var a = listView.Items[selectedIndex];
                        ellipseToAnimate = (Ellipse)listView.Items[selectedIndex];
                        ellipseToAnimate.RenderTransform = new CompositeTransform() { ScaleX = 1.7f, ScaleY = 1.7f };
                        ellipseToAnimate.Fill = new SolidColorBrush(Colors.Orange);
                        oldAnimatedEllipse = ellipseToAnimate;
                    }
                    listView.SelectedIndex = selectedIndex;
                }
            }
            catch (Exception e)
            {

            }
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var es = (Ellipse)sender;
            //es.
            //throw new NotImplementedException();
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
            threadPoolTimer.Cancel();
        }
        private void SearchField_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                if (PackageHelper.SearchApps.Count > 0)
                {
                    sender.ItemsSource = PackageHelper.SearchApps.Where(p => p.Name.ToLower().Contains(((AutoSuggestBox)sender).Text.ToLower())).ToList();

                }
            }
        }

        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            FinalTiles ap = (FinalTiles)args.SelectedItem;
            PackageHelper.LaunchApp(ap.FullName).ConfigureAwait(false);

            sender.ItemsSource = PackageHelper.SearchApps;
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
            _ = PackageHelper.RescanForNewApplications().ConfigureAwait(true);
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
            await Task.Delay(3000);
            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            Point startpoint = e.GetPosition(GridViewMain);
            GeneralTransform a = GridViewMain.TransformToVisual(GridViewMain);
            Point b = a.TransformPoint(new Point(0, 0));
            if (startpoint.X < (b.X + 15))
            {
                if (PackageHelper.pageVariables.IsPrevious)
                {
                    pageChanged?.Invoke(new PageChangedEventArgs(_pageNum - 1));
                }
            }
            else if (startpoint.X > (b.X + d.ActualWidth - 70))
            {
                if (PackageHelper.pageVariables.IsNext)
                {
                    pageChanged?.Invoke(new PageChangedEventArgs(_pageNum + 1));
                    e.Handled = true;
                    await Task.Delay(5000);
                }
            }
            DelayDragOver(5000);
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
                if (moveto >= PackageHelper.Apps.GetOriginalCollection().Count() - 1)
                {
                    moveto = PackageHelper.Apps.GetOriginalCollection().Count() - 1;
                }
            }
            if (listindex <= t.Count() - 1)
            {
                moveto = (_pageNum * _appsPerScreen) + listindex;
            }
            PackageHelper.Apps.MoveApp(_Itemdragged.InitialIndex, moveto);
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
                await PackageHelper.LaunchApp(((FinalTiles)selecteditem).FullName);
            }
        }

        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            disableScrollViewer(GridViewMain);
        }

        private void About_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
            threadPoolTimer.Cancel();
        }



        private void InstallApps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(InstallApps));
            threadPoolTimer.Cancel();
        }

        private void AddFolders_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Folders));
            threadPoolTimer.Cancel();
        }
        public void OnNetworkStatusChange(object sender)
        {
            SettingsHelper.totalAppSettings.Sync = false;
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
            {


                if (!profile.IsWlanConnectionProfile)
                {

                    SettingsHelper.totalAppSettings.Sync = false;
                    return;
                }
                //              SettingsHelper.totalAppSettings.Sync = true;
                if (!UDPServer.IsBusy)
                {
                    UDPServer.RunWorkerAsync();
                }
                if (!UDPClient.IsBusy)
                {
                    UDPClient.RunWorkerAsync();
                }
                if (!TCPServer.IsBusy)
                {
                    TCPServer.RunWorkerAsync();
                }

            }
        }

        private void NetworkStatusChange()
        {
            networkstatuschangedhandler = new NetworkStatusChangedEventHandler(OnNetworkStatusChange);
            if (isnetworkstatuschangedregistered == false)
            {
                NetworkInformation.NetworkStatusChanged += networkstatuschangedhandler;
                isnetworkstatuschangedregistered = true;
            }

        }
        private void TCPClient_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //           throw new NotImplementedException();
        }

        private void TCPClient_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SettingsHelper.totalAppSettings.RemoteIP != null)
            {
                TcpClient client = new TcpClient();
                client.ConnectAsync(SettingsHelper.totalAppSettings.RemoteIP.Address, 13000);
                using (var stream = client.GetStream())
                {
                    string content = JsonConvert.SerializeObject(SettingsHelper.totalAppSettings);
                    byte[] sendbody = Encoding.UTF8.GetBytes(content);
                    stream.WriteAsync(sendbody, 0, sendbody.Length);
                }
            }
        }

        private void TCPServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void TCPServer_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 13000);
            Task.Factory.StartNew(async () =>
            {
                listener.Start();
                while (true)
                {

                    TcpClient a = await listener.AcceptTcpClientAsync();
                    NetworkStream b = a.GetStream();
                    byte[] message = new byte[1024];
                    string test = string.Empty;
                    using (var stream = a.GetStream())
                    {
                        int ab = b.Read(message, 0, message.Length);
                        if (ab == 0)
                        {
                            listener.Stop();
                            return;
                        }
                        test += Encoding.UTF8.GetString(message, 0, ab);
                        SettingsHelper.totalAppSettings = JsonConvert.DeserializeObject<GlobalAppSettings>(test);
                    }
                    listener.Stop();
                }
            });
        }

        private void UDPClient_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void UDPClient_DoWork(object sender, DoWorkEventArgs e)
        {
            UdpClient client = new UdpClient();
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Broadcast, 32123);
                    IReadOnlyList<HostName> hosts = NetworkInformation.GetHostNames();
                    byte[] data = Encoding.UTF8.GetBytes(hosts.ToString());
                    var a = await client.SendAsync(data, data.Length, ipep);
                    if (a > -1)
                    {
                        break;
                    }
                }
            });

        }

        private void UDPServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void UDPServer_DoWork(object sender, DoWorkEventArgs e)
        {
            UdpClient server = new UdpClient();

            //start listening for messages and copy the messages back to the client
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 32123);
                    UdpReceiveResult data = await server.ReceiveAsync();
                    var hosts = NetworkInformation.GetHostNames();
                    var es = Encoding.UTF8.GetString(data.Buffer);
                    if (es != hosts.ToString())
                    {
                        SettingsHelper.totalAppSettings.RemoteIP = data.RemoteEndPoint;
                        Console.WriteLine(data.RemoteEndPoint.Address);
                        Console.WriteLine(es);
                        break;
                    }


                }
            });

        }



        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var a = e.ClickedItem as Ellipse;

        }
    }
}