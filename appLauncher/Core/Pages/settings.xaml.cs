using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;

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
        private BackgroundImages oldImage { get; set; }
        private BackgroundImages newImage { get; set; }

        private AppTile oldappTile { get; set; }
        private AppTile newappTile { get; set; }

        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
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

            if (await Logging.IsFilePresent("images.txt"))
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
                if (await Logging.IsFilePresent("images.txt"))
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

        private void BackgroundImagelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackgroundImagelist.SelectedIndex != -1)
            {
                oldImage =(BackgroundImages)BackgroundImagelist.SelectedItem;
                BackgroundImageOpacityText.Text = oldImage.BackgroundImageOpacity.ToString();
                BackgroundImageColorPicker.SelectedItem = oldImage.Backgroundimagecolor;
            }
        }
    }
}
