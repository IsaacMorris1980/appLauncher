using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Page where the launcher settings are configured
    /// </summary>
    public sealed partial class settings : Page
    {
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private AppTile appTile { get; set; }

        private BackgroundImages backgroundImages { get; set; }
        private int selectedIndex { get; set; } = 0;
        public settings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Runs when the app has navigated to this page.
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (await logHelper.IsFilePresent("images.txt"))
            {
                BackgroundImageAddButton.Content = "Add Image";
            }
        }

        /// <summary>
        /// Launches the file picker and allows the user to pick an image from their pictures library.<br/>
        /// The image will then be used as the background image in the main launcher page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BackgroundImageAddButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //Standard Image Support
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpe");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".svg");
            picker.FileTypeFilter.Add(".tif");
            picker.FileTypeFilter.Add(".tiff");
            picker.FileTypeFilter.Add(".bmp");
            //JFIF Support
            picker.FileTypeFilter.Add(".jif");
            picker.FileTypeFilter.Add(".jfif");
            //GIF Support
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".gifv");
            var file = await picker.PickMultipleFilesAsync();
            if (file.Any())
            {
                if (await logHelper.IsFilePresent("images.txt"))
                {
                    BitmapImage bitmap = new BitmapImage();
                    foreach (StorageFile item in file)
                    {
                        BackgroundImages bi = new BackgroundImages();
                        bi.Filename = item.DisplayName;
                        bi.Backgroundimage = await imageHelper.ReturnImage(item);
                        if (!imageHelper.backgroundImage.Contains(bi))
                        {
                            imageHelper.backgroundImage.Add(bi);
                        }
                    }
                }
                else
                {
                    foreach (StorageFile item in file)
                    {
                        BackgroundImages bi = new BackgroundImages();
                        bi.Filename = item.DisplayName;
                        bi.Backgroundimage = await imageHelper.ReturnImage(item);
                        imageHelper.backgroundImage.Add(bi);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }

        private void BackgroundImageDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (BackgroundImagelist.SelectedIndex != -1)
            {
                BackgroundImages bi = (BackgroundImages)BackgroundImagelist.SelectedItem;
                if (imageHelper.backgroundImage.Any(x => x.Filename == bi.Filename))
                {
                    List<BackgroundImages> files = (from x in imageHelper.backgroundImage where x.Filename == bi.Filename select x).ToList();
                    foreach (var item in files)
                    {
                        imageHelper.backgroundImage.Remove(item);
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundImages bi = (BackgroundImages)BackgroundImagelist.SelectedItem;
            bi.Backgroundimagecolor = BackgroundImageColorPicker.Color;
            bi.BackgroundImageOpacity = BackgroundImageColorPicker.Opacity / 100;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (SettingsPiviot.SelectedIndex)
            {
                case 0:
                    settingsHelper.appSettings.fontsize = double.Parse(settingsFontSize.Text);
                    settingsHelper.appSettings.ForegroundColor = AppForgroundColorPicker.Color;
                    break;
                case 1:
                    if (appTile==null)
                    {
                        return;
                    }
                    appTile.AppTileForegroundcolor = AppTileForegroundColorPicker.Color;
                    appTile.AppTileForegroundOpacity = AppTileForegroundColorPicker.Opacity;
                    appTile.AppTileBackgroundcolor = AppTileBackgroundColorPicker.Color;
                    appTile.AppTileBackgroundOpacity = AppTileBackgroundColorPicker.Opacity;
                    packageHelper.Bags.RemoveAt(selectedIndex);
                    packageHelper.Bags.Insert(selectedIndex, appTile);
                    appTile = null;
                    selectedIndex = -1;
                    break;
                case 2:
                    if (backgroundImages==null)
                    {
                        return;
                    }
                    backgroundImages.Backgroundimagecolor = BackgroundImageColorPicker.Color;
                    backgroundImages.BackgroundImageOpacity = BackgroundImageColorPicker.Opacity;
                    imageHelper.backgroundImage.RemoveAt(selectedIndex);
                    imageHelper.backgroundImage.Insert(selectedIndex, backgroundImages);
                    backgroundImages = null;
                    selectedIndex = -1;
                    break;
                default:
                    break;
            }

            if (appTile == null && SettingsPiviot.SelectedIndex == 2)
            {
                
                backgroundImages.Backgroundimagecolor = BackgroundImageColorPicker.Color;
                backgroundImages.BackgroundImageOpacity = BackgroundImageColorPicker.Opacity;
                imageHelper.backgroundImage.RemoveAt(selectedIndex);
                imageHelper.backgroundImage.Insert(selectedIndex, backgroundImages);
                backgroundImages = null;
                selectedIndex = -1;
            }
            else if (backgroundImages == null && SettingsPiviot.SelectedIndex == 1)
            {
                appTile.AppTileForegroundcolor = AppTileForegroundColorPicker.Color;
                appTile.AppTileForegroundOpacity = AppTileForegroundColorPicker.Opacity;
                appTile.AppTileBackgroundcolor = AppTileBackgroundColorPicker.Color;
                appTile.AppTileBackgroundOpacity = AppTileBackgroundColorPicker.Opacity;
                packageHelper.Bags.RemoveAt(selectedIndex);
                packageHelper.Bags.Insert(selectedIndex, appTile);
                appTile = null;
                selectedIndex = -1;
            }

            else if(SettingsPiviot.SelectedIndex==0)
            {
                settingsHelper.appSettings.fontsize = double.Parse(settingsFontSize.Text);
                settingsHelper.appSettings.ForegroundColor = AppForgroundColorPicker.Color;

            }
            else
            {
                Crashes.TrackError(new ArgumentException());
            }
        }

        private void BackgroundImagelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackgroundImagelist.SelectedIndex!=-1)
            {
                backgroundImages = (BackgroundImages)BackgroundImagelist.SelectedItem;
                BackgroundImageColorPicker.Color = backgroundImages.Backgroundimagecolor;
                BackgroundImageColorPicker.Opacity = backgroundImages.BackgroundImageOpacity;
                selectedIndex = BackgroundImagelist.SelectedIndex;
            }
          
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            settingsFontSize.Text = settingsHelper.appSettings.fontsize.ToString();
            AppForgroundColorPicker.Color = settingsHelper.appSettings.ForegroundColor;
        }

        private void AppTilelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppTilelist.SelectedIndex!=-1)
            {
                appTile = (AppTile)AppTilelist.SelectedItem;
                AppTileBackgroundColorPicker.Color = appTile.AppTileBackgroundcolor;
                AppTileBackgroundColorPicker.Opacity = appTile.AppTileBackgroundOpacity;
                AppTileForegroundColorPicker.Color = appTile.AppTileForegroundcolor;
                AppTileForegroundColorPicker.Opacity = appTile.AppTileForegroundOpacity;
                selectedIndex = AppTilelist.SelectedIndex;
            }
        }
    }
}
