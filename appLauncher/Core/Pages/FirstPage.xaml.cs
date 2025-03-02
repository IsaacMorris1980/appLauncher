using appLauncher.Core.CustomEvent;
using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.UI.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstPage : Page
    {
        private string onpage = "";
        private bool isOnMainPage
        {
            get
            {
                return onpage == "mainpage";
            }
        }
        public static List<FinalTiles> tiles = new List<FinalTiles>();
        public static List<AppFolder> appFolders = new List<AppFolder>();
        public static int numOfPages = 1;
        DispatcherTimer searchDelay = new DispatcherTimer();
        public string searchText = string.Empty;
        public string previousSearchText = string.Empty;
        public int currentTimeLeft = 0;
        public int updateTimeer = 1500;
        public static ObservableCollection<PageIndicators> pages = new ObservableCollection<PageIndicators>();
        public static Frame navFrame { get; set; }
        public static InAppNotification showMessage { get; set; }
        private int _currentPage = 0;
        private int _numofPages = 0;
        private bool buttonssetup;
        private int _numOfPages;
        public static event PageChangedDelegate pageChanged;
        public static event PageNumChangedDelegate pageNumChanged;
        public FirstPage()
        {
            this.InitializeComponent();
            navFrame = NavFrame;
            searchDelay.Tick += SearchDelay_Tick;
            searchDelay.Interval = TimeSpan.FromMilliseconds(100);
            NavFrame.Navigate(typeof(AppLoading));
            MainPage.numofPagesChanged += SetupPageIndicators;
            PackageHelper.pageVariables = new PageChangingVariables();
            pageChanged += FirstPage_pageChanged;
            navFrame.Navigating += NavFrame_Navigating;
        }
        private void NavFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            onpage = (sender.GetType() == typeof(MainPage)) ? "mainpage" : string.Empty;
            Debug.WriteLine(onpage);
        }
        private void FirstPage_pageChanged(PageChangedEventArgs e)
        {
            _currentPage = e.PageIndex;
            PackageHelper.pageVariables.IsPrevious = e.PageIndex > 0;
            PackageHelper.pageVariables.IsNext = e.PageIndex < _numofPages - 1;
        }
        private void SearchDelay_Tick(object sender, object e)
        {
            if (currentTimeLeft == 0)
            {
                if (searchText == string.Empty)
                {
                    PackageHelper.Apps.Search(searchText);
                    searchDelay.Stop();
                    return;
                }
                if (searchText.Equals(previousSearchText))
                {

                    PackageHelper.Apps.Search(searchText);
                    searchDelay.Stop();
                }
            }
            else
            {
                currentTimeLeft -= (int)searchDelay.Interval.TotalMilliseconds;
                previousSearchText = searchText;
            }
        }
        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("In back tapped");
            if (NavFrame.CanGoBack)
            {
                NavFrame.GoBack();
            }
        }
        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(MainPage));
        }
        private void SettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt(((FontIcon)sender));
        }
        private void AboutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            navFrame.Navigate(typeof(AboutPage));
        }
        private void FilterApps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt(((FontIcon)sender));
        }
        private void Search_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt((FontIcon)sender);
        }
        private void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = (((TextBox)sender).Text);
            PackageHelper.Apps.Search(searchText);
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
                    Height = 13,
                    Width = 13,
                    Margin = new Thickness(12),
                    Fill = (i == SettingsHelper.totalAppSettings.LastPageNumber) ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Gray),
                };
                ToolTipService.SetToolTip(el, $"Page {i + 1}");
                listView.Items.Add(el);
            }
            pageChanged?.Invoke(new PageChangedEventArgs(SettingsHelper.totalAppSettings.LastPageNumber));
        }
        private void CreateSpecialFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt((FontIcon)sender);
        }
        private void InstallorRemoveApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt((FontIcon)sender);
        }
        private void InstallorRemvoe(object sender, TappedRoutedEventArgs e)
        {
            switch (((MenuFlyoutItem)sender).Tag)
            {
                case "Install":
                    NavFrame.Navigate(typeof(InstallApps));
                    break;
                case "Remove":
                    navFrame.Navigate(typeof(RemoveApps));
                    break;
                default:
                    showMessage.Show("Did not select install or remove and app", 1500);
                    break;
            }
        }
        private void Rescan_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.RescanForNewApplications();
        }
        private void FilterAppsAndFolders(object sender, TappedRoutedEventArgs e)
        {
            PackageHelper.Apps.GetFilteredApps(((MenuFlyoutItem)sender).Tag.ToString());
        }
        private async void CreateRemoveFolders(object sender, TappedRoutedEventArgs e)
        {
            switch (((MenuFlyoutItem)sender).Tag)
            {
                case "Create":
                    AppFolder _createdFolder = new AppFolder();
                    FolderNamePage dialog = new FolderNamePage();
                    ContentDialogResult result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        _createdFolder.Name = dialog.FolderName;
                    }
                    navFrame.Navigate(typeof(EditFolder), _createdFolder);
                    break;
                case "Remove":
                    navFrame.Navigate(typeof(RemoveFolder));
                    break;
                default:
                    break;
            }
        }
        private async void CreateSpecialFolders(object sender, TappedRoutedEventArgs e)
        {
            if (PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().Any(x => x.Name == "Favorite") || PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().Any(x => x.Name == "Most Used"))
            {
                return;
            }
            switch (((MenuFlyoutItem)sender).Tag)
            {
                case "new":
                    AppFolder _createdFolder = new AppFolder();
                    FolderNamePage dialog = new FolderNamePage();
                    ContentDialogResult result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        _createdFolder.Name = dialog.FolderName;
                    }
                    navFrame.Navigate(typeof(EditFolder), _createdFolder);
                    break;
                case "remove":
                    navFrame.Navigate(typeof(RemoveFolder));
                    break;
                case "favorite":
                    if (!PackageHelper.Apps.GetOriginalCollection().Any(x => x.Name == "Favorites"))
                    {
                        if (AnyFavorites())
                        {
                            AppFolder folder = new AppFolder()
                            {
                                Name = "Favorites",
                                Description = "Favorited apps",
                                ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                                InstalledDate = System.DateTime.Now
                            };

                            PackageHelper.Apps.AddFolder(folder);
                            PackageHelper.Apps.RecalculateThePageItems();
                            break;
                        }
                    }
                    else
                    {
                        AppFolder favorite = (AppFolder)PackageHelper.Apps.GetOriginalCollection().First(x => x.Name == "Favorites");
                        navFrame.Navigate(typeof(FolderView), favorite);
                    }

                    showMessage.Show("No apps selected as favorite", 2000);
                    break;
                case "used":
                    if (AnyMostUsed())
                    {
                        AppFolder usedfolder = new AppFolder()
                        {
                            Name = "Most Used",
                            Description = "Apps Launched over 5 times using this app",
                            ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                            InstalledDate = DateTime.Now
                        };
                        PackageHelper.Apps.AddFolder(usedfolder);
                        PackageHelper.Apps.RecalculateThePageItems();
                        break;
                    }
                    showMessage.Show("No apps launched more than 5 times", 1000);
                    break;
                default:
                    return;
            }
        }
        private bool AnyFavorites()
        {
            var apps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            foreach (var item in folders)
            {
                apps.AddRange(item.FolderApps.ToList());
            }
            bool isfavorite = apps.Any(x => x.Favorite == true);
            return isfavorite;
        }
        private bool AnyMostUsed()
        {
            var apps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            foreach (var item in folders)
            {
                apps.AddRange(item.FolderApps.ToList());
            }
            bool mostusded = apps.Any(x => x.LaunchedCount > 5);
            return mostusded;
        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            showMessage = Inapp;
            navFrame.Height = MainNavigation.ActualHeight;
            navFrame.Width = MainNavigation.ActualWidth - 50;
            PackageHelper.pageVariables.IsNext = _currentPage < (_numofPages - 1);
            PackageHelper.pageVariables.IsPrevious = _currentPage > 0;
        }
        private void NextPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PackageHelper.pageVariables.IsNext)
            {
                pageChanged?.Invoke(new PageChangedEventArgs(_currentPage + 1));
            }
        }

        private void PreviousPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PackageHelper.pageVariables.IsPrevious)
            {
                pageChanged?.Invoke(new PageChangedEventArgs(_currentPage - 1));
            }
        }



        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            navFrame.Height = MainNavigation.ActualHeight;
            navFrame.Width = MainNavigation.ActualWidth - 50;
        }

        public void UpdateIndicator(PageChangedEventArgs e)
        {
            PackageHelper.pageVariables.IsPrevious = e.PageIndex > 0;
            PackageHelper.pageVariables.IsNext = e.PageIndex < _numofPages - 1;
            int itemsCount = listView.Items.Count();
            foreach (var item in pages)
            {
                if (item.PageNum == e.PageIndex)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            Debug.WriteLine(e.PageIndex);
            PackageHelper.Apps.PageChanged(new PageChangedEventArgs(e.PageIndex));
            Bindings.Update();
            //AdjustIndicatorStackPanel(e.PageIndex);

        } 

        private void ellButtons_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var pagenumber = int.Parse(((Button)sender).Tag.ToString());

            pageChanged?.Invoke(new PageChangedEventArgs(pagenumber + 1));
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ellipse el = ((Ellipse)listView.SelectedItem);
            foreach (Ellipse els in listView.Items)
            {
                if ((int)(els.Tag) == ((int)el.Tag))
                {
                    els.Fill = new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    els.Fill = new SolidColorBrush(Colors.Gray);
                }
                SettingsHelper.totalAppSettings.LastPageNumber = ((int)el.Tag);
                pageChanged?.Invoke(new PageChangedEventArgs(((int)el.Tag)));
            }
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void BackImageSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {

            navFrame.Navigate(typeof(AppBackgroundSettings));
        }

        private void AppTileSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            navFrame.Navigate(typeof(AppTilesSettings));
        }

        private void AppSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            navFrame.Navigate(typeof (AppSettings));
        }
    }
}

