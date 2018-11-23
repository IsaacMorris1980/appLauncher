using appLauncher.Animations;
using appLauncher.Control;
using appLauncher.Core;
using appLauncher.Helpers;
using appLauncher.Model;
using appLauncher.Pages;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace appLauncher
{

    /// <summary>
    /// The page where the apps are displayed. Most of the user interactions with the app launcher will be here.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int maxRows;
        public ObservableCollection<finalAppItem> finalApps;
        public ObservableCollection<finalAppItem> queriedApps = new ObservableCollection<finalAppItem>();
        public static FlipViewItem flipViewTemplate;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        bool pageIsLoaded = false;

        /// <summary>
        /// Runs when a new instance of MainPage is created
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            finalApps = finalAppItem.listOfApps;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            foreach (var item in finalApps)
            {
                queriedApps.Add(item);
            }
            screensContainerFlipView.Items.VectorChanged += Items_VectorChanged;

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

        private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AllAppsGrid.Visibility == Visibility.Visible)
            {
                DesktopBackButton.HideBackButton();
                e.Handled = true;
                await Task.WhenAll(
                AllAppsGrid.Fade(0).StartAsync(),
                AppListViewGrid.Blur(0).StartAsync());
                AllAppsGrid.Visibility = Visibility.Collapsed;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.Scale(2f, 2f, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 0).StartAsync();
            await this.Scale(1, 1, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 300).StartAsync();
        }

        /// <summary>
        /// When an app is selected, the launcher will attempt to launch the selected app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void appGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedApp = (finalAppItem)e.ClickedItem;
            bool isLaunched = await clickedApp.appEntry.LaunchAsync();
            if (isLaunched == false)
            {
                Debug.WriteLine("Error: App not launched!");
            }
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

            maxRows = (int)(appGridView.ActualHeight / 124);
            int appsPerScreen = 4 * maxRows;
            int additionalPagesToMake = calculateExtraPages(appsPerScreen) - 1;
            int fullPages = additionalPagesToMake;
            int appsLeftToAdd = finalApps.Count() - (fullPages * appsPerScreen);

            //NOTE: I wasn't able to create an ItemTemplate from C# so I made a GridView
            //in the XAML view with the desired values and used its 
            //item template to create identical GridViews.

            //If you know how to create ItemTemplates in C#, please make a pull request which
            //with a new solution for this issue or contanct me directly. It would make things way easier for everyone!
            DataTemplate theTemplate = appGridView.ItemTemplate;


            //Following code creates any extra app pages then adds apps to each page.
            if (additionalPagesToMake > 0)
            {
                ControlTemplate template = new appControl().Template;

                for (int i = 0; i < additionalPagesToMake; i++)
                {
                    screensContainerFlipView.Items.Add(new FlipViewItem()
                    {
                        Content = new GridView()
                        {
                            ItemTemplate = theTemplate,
                            ItemsPanel = appGridView.ItemsPanel,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            IsItemClickEnabled = true,
                            Margin = new Thickness(0, 10, 0, 0),
                            SelectionMode = ListViewSelectionMode.None

                        }


                    });


                    int j = i + 2;
                    {
                        var flipViewItem = (FlipViewItem)screensContainerFlipView.Items[j];
                        var gridView = (GridView)flipViewItem.Content;
                        gridView.ItemClick += appGridView_ItemClick;
                    }

                }
                int start = 0;
                int end = appsPerScreen;

                for (int j = 1; j < fullPages + 1; j++)
                {

                    FlipViewItem screen = (FlipViewItem)screensContainerFlipView.Items[j];
                    GridView gridOfApps = (GridView)screen.Content;
                    addItemsToGridViews(gridOfApps, start, end);
                    if (j == 1)
                    {
                        start = appsPerScreen + 1;
                        end += appsPerScreen + 1;
                    }
                    else
                    {
                        start += appsPerScreen;
                        end += appsPerScreen;
                    }
                }


                int startOfLastAppsToAdd = finalApps.Count() - appsLeftToAdd;


                FlipViewItem finalScreen = (FlipViewItem)screensContainerFlipView.Items[additionalPagesToMake + 1];
                GridView finalGridOfApps = (GridView)finalScreen.Content;
                addItemsToGridViews(finalGridOfApps, startOfLastAppsToAdd, finalApps.Count());
                screensContainerFlipView.SelectedItem = screensContainerFlipView.Items[1];
                AdjustIndicatorStackPanel(1);
            }
            else
            {
                for (int i = 0; i < finalApps.Count() - 1; i++)
                {
                    appGridView.Items.Add(finalApps[i]);
                }
            }
            loadSettings();
            pageIsLoaded = true;
            screensContainerFlipView.SelectionChanged += screensContainerFlipView_SelectionChanged;




        }

        /// <summary>
        /// Loads local settings e.g. loads background image if it's available.
        /// </summary>
        private async void loadSettings()
        {
            if ((string)App.localSettings.Values["bgImageAvailable"] == "1")
            {
                //load background image
                var bgImageFolder = await localFolder.GetFolderAsync("backgroundImage");
                var filesInBgImageFolder = await bgImageFolder.GetFilesAsync();
                var bgImageFile = filesInBgImageFolder.First();
                rootGrid.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(bgImageFile.Path)),
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.7
                };

            }




        }

        /// <summary>
        /// Attempts to disable vertical scrolling.
        /// </summary>
        /// <param name="gridView"></param>
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
            double numberOfApps = finalApps.Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }

        /// <summary>
        /// Adds apps to an app page
        /// </summary>
        /// <param name="gridOfApps"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void addItemsToGridViews(GridView gridOfApps, int start, int end)
        {
            for (int k = start; k < end; k++)
            {
                gridOfApps.Items.Add(finalApps[k]);
            }
        }


        /// <summary>
        /// (Not implemented yet) Will attempt to launch an app in the launcher dock.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO: try cast object as appItem then launch the app

        }

        /// <summary>
        /// Runs when launcher settings is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("You clicked on the settings icon");
            Frame.Navigate(typeof(settings));
        }

        /// <summary>
        /// Runs whenever app the the selected FlipViewItem has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void screensContainerFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pageIsLoaded)
            {
                int SelectedIndex = screensContainerFlipView.SelectedIndex;
                if (SelectedIndex == 0)
                {
                    //Swipe Right for Cortana!
                    await Launcher.LaunchUriAsync(new Uri("ms-cortana://"));
                    screensContainerFlipView.SelectedIndex = 1;
                   await AdjustIndicatorStackPanel(screensContainerFlipView.SelectedIndex);
                }
                else
                {
                    await AdjustIndicatorStackPanel(SelectedIndex);

                }

            }
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
                    ellipse.Fill = new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
                    
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
            await Task.WhenAll(ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY,duration, easingType: EasingType.Back).StartAsync(),
                IndicatorAnimation.oldAnimatedEllipse.Scale(1,1,centerX,centerY, duration, easingType: EasingType.Back).StartAsync());

            }
            else
            {
                await ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration,easingType: EasingType.Bounce).StartAsync();
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

                FlipViewItem screen = (FlipViewItem)screensContainerFlipView.Items[i];
                GridView gridOfApps = (GridView)screen.Content;
                disableScrollViewer(gridOfApps);
            }
        }

        private async void allAppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DesktopBackButton.ShowBackButton();
            AllAppsGrid.Visibility = Visibility.Visible;
            await Task.WhenAll(
            AllAppsGrid.Fade(1).StartAsync(),
            AppListViewGrid.Blur(20).StartAsync());
            useMeTextBox.Focus(FocusState.Pointer);

        }

        private void useMeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = useMeTextBox.Text.ToLower();
            if (!String.IsNullOrEmpty(query))
            {
                List<finalAppItem> queryList = finalApps.Where(p => p.appEntry.DisplayInfo.DisplayName.ToLower().Contains(query)).ToList();
                int count = queryList.Count;
                if (count > 0)
                {
                    queriedApps.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        queriedApps.Add(queryList[i]);

                    }
                }
            }
            else
            {
                queriedApps.Clear();
                List<finalAppItem> fullList = new List<finalAppItem>();
                int count = finalApps.Count;
                for (int i = 0; i < count; i++)
                {
                    queriedApps.Add(finalApps[i]);
                }
            }
        }


        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            await ((finalAppItem)e.ClickedItem).appEntry.LaunchAsync();


        }

		private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		
		 string selected = ((ComboBoxItem)Filterby.SelectedItem).Content.ToString();
			switch (selected)
			{
				case "AtoZ":
					{
						var te = finalAppItem.Allpackages.OrderBy(x => x.Key.DisplayInfo.DisplayName);
						ObservableCollection<finalAppItem> items = new ObservableCollection<finalAppItem>();
						foreach (var item in te)
						{
							items.Add(new finalAppItem { appEntry = item.Key,
								appLogo = finalAppItem.listOfApps.First(x=>x.appEntry==item.Key).appLogo});
						}
						finalAppItem.listOfApps = items;
					}
					
					break;
				case "Developer":
					{
						
						var te = finalAppItem.Allpackages.OrderBy(x => x.Value.Id.Publisher);
						ObservableCollection<finalAppItem> items = new ObservableCollection<finalAppItem>();
						foreach (var item in te)
						{
							items.Add(new finalAppItem
							{
								appEntry = item.Key,
								appLogo = finalAppItem.listOfApps.First(x=>x.appEntry==item.Key).appLogo
							});
						}
						finalAppItem.listOfApps = items;
					}
					break;
				case "Installed":
					{
						var te = finalAppItem.Allpackages.OrderBy(x => x.Value.InstalledDate);
						ObservableCollection<finalAppItem> items = new ObservableCollection<finalAppItem>();
						foreach (var item in te)
						{
							items.Add(new finalAppItem
							{
								appEntry = item.Key,
								appLogo = finalAppItem.listOfApps.First(x=>x.appEntry==item.Key).appLogo
							});
						}
						finalAppItem.listOfApps = items;
					}
					break;
				default:
					break;
			}
			this.Frame.Navigate(typeof(appLauncher.MainPage));
		}
	}
}
