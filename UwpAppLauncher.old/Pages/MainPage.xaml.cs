using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.UI.Animations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UwpAppLauncher.Animation;
using UwpAppLauncher.Interfaces;
using UwpAppLauncher.Model;

using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpAppLauncher.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private App app = (App)Application.Current;
        private int maxRows;
        private int maxColumns;
        private int appsperpage;
        private int numofpages;
        private int selectedpage;
        public static FolderAppDialog folderAppDialog;
        bool firstrun { get; set; } = true;
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        TimeSpan timeSpan = TimeSpan.FromSeconds(15);
        const int updateTimerLength = 100;
        private bool Nextpagedisplay
        {
            get
            {
                bool shown = false;
                if (selectedpage >= numofpages)
                {
                    shown = false;
                }
                if (selectedpage >= 0 && selectedpage < numofpages)
                {
                    shown = true;
                }
                return shown;
            }
        }

        private DraggedItem draggedItem = new DraggedItem();
        public MainPage()
        {
            this.InitializeComponent();
            sizeChangeTimer.Tick += SizeChangeTimer_Tick;
        }
        private bool Previouspagedisplay
        {
            get
            {
                bool shown = false;
                if (selectedpage <= 0)
                {
                    shown = false;
                }
                if (selectedpage < numofpages - 1 && selectedpage > 0)
                {
                    shown = true;
                }
                return shown;
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            maxRows = NumofRoworColumn(12, 12, (int)GridViewMain.ActualHeight);
            maxColumns = NumofRoworColumn(12, 12, (int)GridViewMain.ActualWidth);
            appsperpage = maxColumns * maxRows;
            numofpages = calculateExtraPages(appsperpage) - 1;
            int appsleft = app.packageHelper.appTiles.Count() - (numofpages * appsperpage);
            if (appsleft > 0)
            {
                numofpages += 1;
            }
            flipViewIndicatorStackPanel.Children.Clear();

            for (int i = 0; i < numofpages; i++)
            {
                flipViewIndicatorStackPanel.Children.Add(new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(app.settingsHelper._appSettings.ForgroundColor),
                    Margin = new Thickness(4, 0, 4, 0)
                });

            };
            GridViewMain.ItemsSource = app.packageHelper.appTiles.Skip(selectedpage * numofpages).Take(appsperpage).ToList();
        }
        private int calculateExtraPages(int appsPerScreen)
        {
            double appsPerScreenAsDouble = appsPerScreen;
            double numberOfApps = app.packageHelper.appTiles.Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }


        public int NumofRoworColumn(int padding, int objectsize, int sizetofit)
        {
            int amount = 0;
            int intsize = objectsize + (padding + padding);
            int size = intsize;
            while (size <= sizetofit)
            {
                amount += 1;
                size += intsize;
            }
            return amount;

        }
        private async void SizeChangeTimer_Tick(object sender, object e)
        {
            try
            {
                if (currentTimeLeft == 0)
                {
                    currentTimeLeft = 0;
                    sizeChangeTimer.Stop();
                    maxRows = NumofRoworColumn(12, 12, (int)GridViewMain.ActualHeight);
                    maxColumns = NumofRoworColumn(12, 12, (int)GridViewMain.ActualWidth);
                    appsperpage = maxColumns * maxRows;
                    numofpages = calculateExtraPages(appsperpage) - 1;
                    int appsleft = app.packageHelper.appTiles.Count() - (numofpages * appsperpage);
                    if (appsleft > 0)
                    {
                        numofpages += 1;
                    }
                    flipViewIndicatorStackPanel.Children.Clear();

                    for (int i = 0; i < numofpages; i++)
                    {
                        flipViewIndicatorStackPanel.Children.Add(new Ellipse
                        {
                            Width = 8,
                            Height = 8,
                            Fill = new SolidColorBrush(app.settingsHelper._appSettings.ForgroundColor),
                            Margin = new Thickness(4, 0, 4, 0)
                        });

                    };
                    GridViewMain.ItemsSource = app.packageHelper.appTiles.Skip(selectedpage * numofpages).Take(appsperpage).ToList();

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
            ContentDialog c = new ContentDialog();
            c.Width = this.ActualWidth - 100;
            c.Height = this.ActualHeight - 100;
            await c.ShowAsync();



        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
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

        private void SettingsPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            app.settingsHelper._appSettings.LastPage = selectedpage;
            Analytics.TrackEvent("Navigated to Settings Page");
            Frame.Navigate(typeof(SettingsPage));
        }

        private void SearchField_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = app.packageHelper.searchApps.Where(p => p.Name.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            AppTile ap = (AppTile)args.SelectedItem;

            ap.LaunchAsync().ConfigureAwait(false);
            sender.Text = String.Empty;
            sender.ItemsSource = app.packageHelper.searchApps;
        }

        private void Filterby_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            MenuFlyoutItem menuitem = (MenuFlyoutItem)sender;

            List<AppTile> orderlist;
            List<IApporFolder> folders = app.packageHelper.appTiles.ToList();
            switch (menuitem.Text)
            {
                case "A-Z":
                    orderlist = app.packageHelper.appTiles.OfType<AppTile>().OrderBy(y => y.Name).ToList();
                    int orderloc = 0;
                    foreach (AppTile tile in orderlist)
                    {
                        int c = app.packageHelper.appTiles.IndexOf(tile);
                        IApporFolder apped = folders[orderloc];
                        if (apped is FolderTile)
                        {
                            orderloc++;
                        }
                        app.packageHelper.appTiles.Move(c, orderloc);
                        orderloc++;
                    }
                    break;
                case "Developer A-Z":
                    orderlist = app.packageHelper.appTiles.OfType<AppTile>().OrderBy(x => x.AppDeveloper).ToList();
                    int listloc = 0;
                    foreach (AppTile tile in orderlist)
                    {
                        int c = app.packageHelper.appTiles.IndexOf(tile);
                        IApporFolder apped = folders[listloc];
                        if (apped is FolderTile)
                        {
                            listloc++;
                        }
                        app.packageHelper.appTiles.Move(c, listloc);
                        listloc++;
                    }
                    break;

                case "Installed Date":
                    orderlist = app.packageHelper.appTiles.OfType<AppTile>().OrderBy(x => x.AppInstalledDate).ToList();
                    int loclist = 0;
                    foreach (AppTile tile in orderlist)
                    {
                        int c = app.packageHelper.appTiles.IndexOf(tile);
                        IApporFolder apped = folders[loclist];
                        if (apped is FolderTile)
                        {
                            loclist++;
                        }
                        app.packageHelper.appTiles.Move(c, loclist);
                        loclist++;
                    }
                    break;

                default:
                    return;

            }
            this.Frame.Navigate(typeof(MainPage));
        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            Point startpoint = e.GetPosition(d);


            GeneralTransform a = this.TransformToVisual(d);
            Point b = a.TransformPoint(new Point(0, 0));
            if (startpoint.X < (b.X + 100))
            {
                if (selectedpage > 0)
                {
                    selectedpage -= 1;

                }

            }
            else if (startpoint.X > (b.X + d.ActualWidth - 100))
            {
                if (selectedpage < numofpages)
                {
                    selectedpage += 1;

                }

            }
            GridViewMain.ItemsSource = app.packageHelper.appTiles.Skip(selectedpage * appsperpage).Take(appsperpage).ToList();
            AdjustIndicatorStackPanel(selectedpage).ConfigureAwait(true);
        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            object item = e.Items.First();
            var source = sender;
            draggedItem.itemdragged = (IApporFolder)item;

        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            GridView view = sender as GridView;

            // Get your data

            //   var item = e.Data.Properties.Where(p => p.Key == "item").Single();

            //Find the position where item will be dropped in the gridview
            Point pos = e.GetPosition(view.ItemsPanelRoot);

            //Get the size of one of the list items
            GridViewItem gvi = (GridViewItem)view.ContainerFromIndex(0);
            double itemHeight = gvi.ActualHeight + gvi.Margin.Top + gvi.Margin.Bottom;
            double itemwidth = gvi.ActualHeight + gvi.Margin.Left + gvi.Margin.Right;

            //Determine the index of the item from the item position (assumed all items are the same size)
            int index = Math.Min(view.Items.Count - 1, (int)(pos.Y / itemHeight));
            int indexy = Math.Min(view.Items.Count - 1, (int)(pos.X / itemwidth));
            List<AppTile> t = (List<AppTile>)view.ItemsSource;
            int te = ((index * maxColumns) + (indexy));
            if (te >= t.Count())
            {
                te = t.Count() - 1;
            }
            else if (te < 0)
            {
                te = 0;
            }
            int its = (selectedpage * appsperpage) + te;
            if (its >= app.packageHelper.appTiles.Count)
            {
                app.packageHelper.appTiles.Move(draggedItem.initialindex, app.packageHelper.appTiles.Count - 1);
            }
            else if (its >= 0 && its < app.packageHelper.appTiles.Count)
            {
                IApporFolder items = app.packageHelper.appTiles[app.packageHelper.appTiles.Count - 1];
                if (items is AppTile)
                {
                    FolderTile ft = new FolderTile();
                    ft.Listlocation = items.Listlocation;
                    ft.FolderApps.Add((AppTile)draggedItem.itemdragged);
                    ft.FolderApps.Add((AppTile)items);
                    app.packageHelper.appTiles.Remove(items);
                    foreach (IApporFolder item in ft.FolderApps)
                    {
                        item.Listlocation = -1;
                    }
                    app.packageHelper.appTiles.Insert(ft.Listlocation, ft);
                }
                if (items is FolderTile)
                {
                    ((FolderTile)items).FolderApps.Add((AppTile)draggedItem.itemdragged);
                }




            }



        }



        private void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            IApporFolder apps = (IApporFolder)e.ClickedItem;
            if (apps is FolderTile)
            {
                Analytics.TrackEvent("Expanding app folder");
                Frame.Navigate(typeof(FolderTile), apps);
            }
        }

        private void PrevioiusPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            selectedpage += 1;
            GridViewMain.ItemsSource = app.packageHelper.appTiles.Skip(selectedpage * appsperpage).Take(appsperpage).ToList();
        }

        private void NextPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            selectedpage -= 1;
            GridViewMain.ItemsSource = app.packageHelper.appTiles.Skip(selectedpage * appsperpage).Take(appsperpage).ToList();
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
    }
}
