using appLauncher.Model;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using appLauncher.Helpers;

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
            GlobalVariables.settingslist.CollectionChanged += Settingslist_CollectionChanged;
        }

        private void Settingslist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex <= GlobalVariables.backgroundImage.Count() - 1)
                    {
                        List<BackgroundImages> lbi = GlobalVariables.backgroundImage.ToList();
                        foreach (var items in lbi)
                        {
                            int oldindex = GlobalVariables.backgroundImage.IndexOf(GlobalVariables.backgroundImage.First(x => x.Filename == items.Filename));
                            int newindex = GlobalVariables.settingslist.IndexOf(GlobalVariables.settingslist.First(x => x.Filename == items.Filename));
                            GlobalVariables.backgroundImage.Move(oldindex, newindex);

                        }
                    }
                    else
                    {
                        GlobalVariables.backgroundImage.Add((BackgroundImages)e.NewItems[0]);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.NewStartingIndex==-1)
                    {
                        BackgroundImages bi = (BackgroundImages)e.OldItems[0];
                          GlobalVariables.backgroundImage.RemoveAt(GlobalVariables.backgroundImage.IndexOf(GlobalVariables.backgroundImage.First(x => x.Filename == bi.Filename)));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
             //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.)
             //{
             //    List<BackgroundImages> lbi = GlobalVariables.backgroundImage.ToList();
             //    foreach (var items in lbi)
             //    {
             //        int oldindex = GlobalVariables.backgroundImage.IndexOf(GlobalVariables.backgroundImage.First(x => x.Filename == items.Filename));
             //        int newindex = GlobalVariables.settingslist.IndexOf(GlobalVariables.settingslist.First(x => x.Filename == items.Filename));
             //        GlobalVariables.backgroundImage.Move(oldindex, newindex);

            //    }
            //}
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.NewStartingIndex == -1)
            //{
            //    BackgroundImages bi = (BackgroundImages)e.OldItems[0];
            //   GlobalVariables.backgroundImage.RemoveAt(GlobalVariables.backgroundImage.IndexOf(GlobalVariables.backgroundImage.First(x => x.Filename == bi.Filename)));
            //}


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
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".svg");
            picker.FileTypeFilter.Add(".tif");
            picker.FileTypeFilter.Add(".bmp");    

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
                     
                        bool exits = filesInFolder.Any(x => x.DisplayName == item.DisplayName);
                        if (!exits)
                        {
                            
                            BackgroundImages bi = new BackgroundImages();
                            bi.Filename = item.DisplayName;
                            bi.Bitmapimage = new BitmapImage(new Uri( (await item.CopyAsync(backgroundImageFolder)).Path));
                            GlobalVariables.settingslist.Add(bi);
                         //  GlobalVariables.backgroundImage.Add(bi);
                         //  await   item.CopyAsync(backgroundImageFolder);
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
                        GlobalVariables.settingslist.Add(bi);
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
            
            if (imagelist.SelectedIndex !=-1)
            {
                BackgroundImages bi = (BackgroundImages)imagelist.SelectedItem;
                if (GlobalVariables.settingslist.Any(x=>x.Filename==bi.Filename))
                {
                    var files = (from x in GlobalVariables.settingslist where x.Filename == bi.Filename select x).ToList();
                    foreach (var item in files)
                    {
                     GlobalVariables.settingslist.Remove(item);
                    }
                }
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                var filesinfolder = await backgroundImageFolder.GetFilesAsync();
                if (filesinfolder.Any(x=>x.DisplayName == bi.Filename))
                {
                    IEnumerable<StorageFile> files = (from x in filesinfolder where x.DisplayName== bi.Filename select x).ToList();
                    foreach (var item in files)
                    {
                        await item.DeleteAsync();
                    }
                }
            }
           
        }

    

      
      
    }
}
