using appLauncher.Control;
using appLauncher.Helpers;
using appLauncher.Model;
using appLauncher.Pages;
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
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
                            Margin = new Thickness(0,10,0,0)

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
                    Stretch = Stretch.UniformToFill
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

            catch(Exception e)
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
                if (screensContainerFlipView.SelectedIndex == 0)
                {                
                    //Swipe Right for Cortana!
                    await Launcher.LaunchUriAsync(new Uri("ms-cortana://"));
                    screensContainerFlipView.SelectedIndex = 1;
                }
            }
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
    }
}
