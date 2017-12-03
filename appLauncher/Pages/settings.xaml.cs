using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Pages
{
    /// <summary>
    /// Page where the launcher settings are configured
    /// </summary>
    public sealed partial class settings : Page
    {
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

            
            if ((string)App.localSettings.Values["bgImageAvailable"] == "1")
            {
                imageButton.Content = "Change Image";
            }
        }

        private async void imageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            //Yes, there is GIF support ;)
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".gifv");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                Debug.WriteLine("Picked photo: " + file.Name);
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                if ((string)App.localSettings.Values["bgImageAvailable"] == "1")
                {
                    var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    if (filesInFolder.Count > 0)
                    {
                        foreach (var photo in filesInFolder)
                        {
                            await photo.DeleteAsync();
                        }

                    }
                }
                StorageFile savedImage = await file.CopyAsync(backgroundImageFolder);
                App.localSettings.Values["bgImageAvailable"] = "1";
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }
    }
}
