using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        readonly StorageFolder local = ApplicationData.Current.LocalFolder;
        private bool allapps = false;
        private Apps selectedapp;
        private Color AppBackColor;
        private Color AppTextColor;


        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void AddButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            //Standard Image Support
            foreach (string item in SettingsHelper.totalAppSettings.SupportedImageTypes)
            {
                picker.FileTypeFilter.Add(item);
            }
            IReadOnlyList<StorageFile> file = await picker.PickMultipleFilesAsync();
            if (file.Any())
            {
                StorageFolder backgroundImageFolder = await local.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);

                if (SettingsHelper.totalAppSettings.BgImagesAvailable)
                {
                    BitmapImage bitmap = new BitmapImage();
                    IReadOnlyList<StorageFile> filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    foreach (StorageFile item in file)
                    {
                        byte[] imagebytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(item);
                        PageBackgrounds bi = new PageBackgrounds
                        {
                            ImageFullPath = item.Path,
                            BackgroundImageBytes = imagebytes,
                            BackgroundImageDisplayName = item.DisplayName
                        };
                        bool exits = filesInFolder.Any(x => x.DisplayName == item.DisplayName);
                        if (!exits)
                        {

                            ImageHelper.backgroundImage.Add(bi);
                            await item.CopyAsync(backgroundImageFolder);
                        }


                    }
                }
                else
                {
                    foreach (StorageFile item in file)
                    {
                        byte[] imagebytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(item);

                        PageBackgrounds bi = new PageBackgrounds
                        {
                            ImageFullPath = item.Path,
                            BackgroundImageDisplayName = item.DisplayName,
                            BackgroundImageBytes = imagebytes
                        };
                        ImageHelper.backgroundImage.Add(bi);
                        bi.ImageFullPath = Path.Combine(backgroundImageFolder.Path, bi.BackgroundImageDisplayName);
                        await item.CopyAsync(backgroundImageFolder);
                        SettingsHelper.totalAppSettings.BgImagesAvailable = true;
                    }


                }
                //   StorageFile savedImage = await file.CopyAsync(backgroundImageFolder);
                //    ((Window.Current.Content as Frame).Content as MainPage).loadSettings();
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }

        private async void RemoveButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            if (imagelist.SelectedIndex != -1)
            {
                PageBackgrounds bi = (PageBackgrounds)imagelist.SelectedItem;
                if (ImageHelper.backgroundImage.Any(x => x.ImageFullPath == bi.ImageFullPath))
                {
                    List<PageBackgrounds> files = (from x in ImageHelper.backgroundImage where x.ImageFullPath == bi.ImageFullPath select x).ToList();
                    foreach (PageBackgrounds item in files)
                    {
                        ImageHelper.backgroundImage.Remove(item);
                    }
                }
                StorageFolder backgroundImageFolder = await local.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                IReadOnlyList<StorageFile> filesinfolder = await backgroundImageFolder.GetFilesAsync();
                if (filesinfolder.Any(x => x.DisplayName == bi.BackgroundImageDisplayName))
                {
                    IEnumerable<StorageFile> files = (from x in filesinfolder where x.DisplayName == bi.BackgroundImageDisplayName select x).ToList();
                    foreach (var item in files)
                    {
                        await item.DeleteAsync();
                    }
                }
            }
        }

        private void Appslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Appslist.SelectedIndex > -1)
            {
                selectedapp = (Apps)Appslist.SelectedItem;
            }
            Preview.IsHitTestVisible = true;
        }


        private void Preview_Tapped(object sender, TappedRoutedEventArgs e)
        {

            selectedapp.LogoColor = AppTileLogoColor.Color;
            selectedapp.TextColor = AppTileTextColor.Color;
            selectedapp.BackColor = AppTileBackgroundColor.Color;
            TestApps.Items.Add(selectedapp);

        }

        private void SaveChanges_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!allapps)
            {
                int appselected = packageHelper.appTiles.GetIndexApp(packageHelper.appTiles.FirstOrDefault(x => x.FullName == selectedapp.FullName));
                if (appselected > -1)
                {
                    packageHelper.appTiles[appselected] = selectedapp;
                }

            }
            else
            {
                List<Apps> appslist = packageHelper.appTiles.GetOriginalCollection().ToList();
                List<Apps> simpleapps = new List<Apps>();
                for (int i = 0; i < appslist.Count; i++)
                {
                    Apps a = new Apps();
                    a = appslist[i];
                    a.TextColor = AppTileTextColor.Color;
                    a.BackColor = AppTileBackgroundColor.Color;
                    a.LogoColor = AppTileLogoColor.Color;
                    simpleapps.Add(a);

                }
                packageHelper.appTiles = new PaginationObservableCollection(new ObservableCollection<Apps>(simpleapps));
            }

            Appslist.IsHitTestVisible = false;
            Appslist.Visibility = Visibility.Collapsed;
            Appslist.SelectedIndex = -1;

            Preview.IsHitTestVisible = false;
            SaveChanges.IsHitTestVisible = false;
            TestApps.Items.Clear();
            TestApps.IsHitTestVisible = false;
            TestApps.Visibility = Visibility.Collapsed;

        }









        private void ColorBackground_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            AppBackColor = args.NewColor;
        }

        private void ColorForeground_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            AppTextColor = args.NewColor;
        }


        private void TrackCrashes_Toggled(object sender, RoutedEventArgs e)
        {
            if (TrackCrashes.IsOn)
            {
                SettingsHelper.totalAppSettings.disableCrashReporting = true;
            }
            else
            {
                SettingsHelper.totalAppSettings.disableCrashReporting = false;
            }
            SettingsHelper.CheckAppSettings().ConfigureAwait(false);
        }

        private void TrackAnalytics_Toggled(object sender, RoutedEventArgs e)
        {
            if (TrackAnalytics.IsOn)
            {
                SettingsHelper.totalAppSettings.disableAnalytics = true;
            }
            else
            {
                SettingsHelper.totalAppSettings.disableAnalytics = false;
            }
            SettingsHelper.CheckAppSettings().ConfigureAwait(false);
        }

        private void AllApps_Toggled(object sender, RoutedEventArgs e)
        {
            if (AllApps.IsOn)
            {
                Preview.IsHitTestVisible = true;
                selectedapp = packageHelper.searchApps[0];
                TestApps.Visibility = Visibility.Visible;
                TestApps.IsHitTestVisible = true;
            }
            else
            {
                Appslist.Visibility = Visibility.Visible;
                Appslist.IsHitTestVisible = true;

                Appslist.SelectedIndex = -1;
                TestApps.Visibility = Visibility.Visible;
                TestApps.IsHitTestVisible = true;
            }
        }

        private void SaveSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SettingsHelper.totalAppSettings.disableAnalytics = TrackAnalytics.IsOn;
            SettingsHelper.totalAppSettings.disableCrashReporting = TrackCrashes.IsOn;
            SettingsHelper.totalAppSettings.BackgroundColor = ColorBackground.Color;
        }
    }
}
