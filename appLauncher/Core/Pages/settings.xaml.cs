using appLauncher.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Pages
{

    /// <summary>
    /// Page where the launcher settings are configured
    /// </summary>
    public sealed partial class settings : Page
    {
        public finalAppItem selectedapp;
        public int selectedindex;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        public settings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Runs when the app has navigated to this page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (GlobalVariables.bgimagesavailable)
            {
                imageButton.Content = "Add Image";
            }
        }

        /// <summary>
        /// Launches the file picker and allows the user to pick an image from their pictures library.<br/>
        /// The image will then be used as the background image in the main launcher page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void imageButton_Click(object sender, RoutedEventArgs e)
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
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);

                if (GlobalVariables.bgimagesavailable)
                {
                    BitmapImage bitmap = new BitmapImage();
                    var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    foreach (StorageFile item in file)
                    {
                        BackgroundImages bi = new BackgroundImages();
                        bi.Filename = item.DisplayName;
                        bi.Bitmapimage = new BitmapImage(new Uri(item.Path));
                        bool exits = filesInFolder.Any(x => x.DisplayName == item.DisplayName);
                        if (!exits)
                        {

                            GlobalVariables.backgroundImage.Add(bi);
                            await item.CopyAsync(backgroundImageFolder);
                        }


                    }
                }
                else
                {
                    foreach (var item in file)
                    {
                        BackgroundImages bi = new BackgroundImages();
                        bi.Filename = item.DisplayName;
                        bi.Bitmapimage = new BitmapImage(new Uri(item.Path));
                        GlobalVariables.backgroundImage.Add(bi);
                        await item.CopyAsync(backgroundImageFolder);
                    }

                    App.localSettings.Values["bgImageAvailable"] = true;
                    GlobalVariables.bgimagesavailable = true;
                }
                //   StorageFile savedImage = await file.CopyAsync(backgroundImageFolder);
                //    ((Window.Current.Content as Frame).Content as MainPage).loadSettings();
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }

        private void AddWebAppButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private async void RemoveButton_ClickAsync(object sender, RoutedEventArgs e)
        {

            if (imagelist.SelectedIndex != -1)
            {
                BackgroundImages bi = (BackgroundImages)imagelist.SelectedItem;
                if (GlobalVariables.backgroundImage.Any(x => x.Filename == bi.Filename))
                {
                    var files = (from x in GlobalVariables.backgroundImage where x.Filename == bi.Filename select x).ToList();
                    foreach (var item in files)
                    {
                        GlobalVariables.backgroundImage.Remove(item);
                    }
                }
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                var filesinfolder = await backgroundImageFolder.GetFilesAsync();
                if (filesinfolder.Any(x => x.DisplayName == bi.Filename))
                {
                    IEnumerable<StorageFile> files = (from x in filesinfolder where x.DisplayName == bi.Filename select x).ToList();
                    foreach (var item in files)
                    {
                        await item.DeleteAsync();
                    }
                }
            }

        }


        private void TextColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            selectedapp.textColor = TextColor.Color;
        }

        private void BackColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            selectedapp.backColor = BackColor.Color;
        }



        private void AppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            selectedapp = ((finalAppItem)e.ClickedItem);
            selectedindex = AppsListView.Items.IndexOf(selectedapp);
            TestApps.Items.Clear();
            TestApps.Items.Add(selectedapp);
            TextColor.IsEnabled = true;
            BackColor.IsEnabled = true;
            PreviewChanges.IsEnabled = true;
        }

        private void PreviewChanges_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectedapp = (finalAppItem)AppsListView.Items[selectedindex];
            TestApps.Items.Clear();
            TestApps.Items.Add(selectedapp);
            TextColor.IsEnabled = false;
            BackColor.IsEnabled = false;
            SaveChanges.IsEnabled = true;
        }

        private async void SaveChanges_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            int index = AllApps.listOfApps.IndexOf(AllApps.listOfApps.First(x => x.appName == selectedapp.appName));
            if (index > -1)
            {
                AllApps.listOfApps[index] = selectedapp;
            }
            await GlobalVariables.SaveCollectionAsync();
            PreviewChanges.IsEnabled = false;
        }


    }
}
