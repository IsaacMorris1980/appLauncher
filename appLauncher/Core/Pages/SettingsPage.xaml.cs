using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Windows.Storage;
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
        StorageFolder local = ApplicationData.Current.LocalFolder;
        private bool allapps = false;
        private AppTile selectedapp;
        private string sectionofapp;
        private string apptilecolor;
        private string apptileopac;
        private string apptextcolor;
        private string apptextopac;
        private string appbackcolor;
        private string appbackopac;
        private string appbordercolor;
        private string appborderopac;
        private bool crashreporting = true;
        private bool anaylitcreporting = true;
        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void AddButton_TappedAsync(object sender, TappedRoutedEventArgs e)
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
                var backgroundImageFolder = await local.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);

                if (GlobalVariables.bgimagesavailable)
                {
                    BitmapImage bitmap = new BitmapImage();
                    var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    foreach (StorageFile item in file)
                    {
                        PageBackgrounds bi = new PageBackgrounds
                        {
                            ImageFullPath = item.Path,
                            BackgroundImageDisplayName = item.DisplayName,
                            pageBackgroundDisplayImage = new BitmapImage(new Uri(item.Path)),
                            BackgroundImageBytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(item)
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
                    foreach (var item in file)
                    {
                        PageBackgrounds bi = new PageBackgrounds
                        {
                            ImageFullPath = item.Path,
                            BackgroundImageDisplayName = item.DisplayName,
                            pageBackgroundDisplayImage = new BitmapImage(new Uri(item.Path)),
                            BackgroundImageBytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(item)
                        };
                        ImageHelper.backgroundImage.Add(bi);
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

        private async void RemoveButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            if (imagelist.SelectedIndex != -1)
            {
                PageBackgrounds bi = (PageBackgrounds)imagelist.SelectedItem;
                if (ImageHelper.backgroundImage.Any(x => x.ImageFullPath == bi.ImageFullPath))
                {
                    var files = (from x in ImageHelper.backgroundImage where x.ImageFullPath == bi.ImageFullPath select x).ToList();
                    foreach (var item in files)
                    {
                        ImageHelper.backgroundImage.Remove(item);
                    }
                }
                var backgroundImageFolder = await local.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                var filesinfolder = await backgroundImageFolder.GetFilesAsync();
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
        private void NumofApps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NumofApps.SelectedIndex > -1)
            {
                ComboBoxItem allappsselected = (ComboBoxItem)NumofApps.SelectedItem;

                if (string.Equals(allappsselected.Content, "Yes"))
                {
                    allapps = true;
                    Appslist.Visibility = Visibility.Collapsed;
                    Appslist.IsHitTestVisible = false;
                    SectionofTile.IsHitTestVisible = true;
                    selectedapp = (AppTile)Appslist.Items[0];
                }
                else
                {
                    allapps = false;
                    Appslist.Visibility = Visibility.Visible;
                    Appslist.IsHitTestVisible = true;
                    SectionofTile.IsHitTestVisible = true;
                }
            }

        }
        private void Appslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Appslist.SelectedIndex > -1)
            {
                selectedapp = (AppTile)Appslist.SelectedItem;
            }
        }
        private void SectionofTile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (SectionofTile.SelectedIndex > -1)
            {
                ComboBoxItem itemtochange = (ComboBoxItem)SectionofTile.SelectedItem;
                sectionofapp = (string)itemtochange.Content;
                ChangeColorCombo.IsHitTestVisible = true;
                OpactiytComboChange.IsHitTestVisible = true;
                Preview.IsHitTestVisible = true;
                SaveChanges.IsHitTestVisible = true;
            }
        }
        private void ChangeColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChangeColorCombo.SelectedIndex > -1)
            {
                apptilecolor = ((string)ChangeColorCombo.Items[ChangeColorCombo.SelectedIndex]);
            }
        }
        private void OpactiytComboChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OpactiytComboChange.SelectedIndex > -1)
            {
                apptileopac = (string)OpactiytComboChange.Items[OpactiytComboChange.SelectedIndex];
            }
        }

        private void Preview_Tapped(object sender, TappedRoutedEventArgs e)
        {

            switch (sectionofapp)
            {
                case "Text Color":
                    selectedapp.appTileTextColor = apptilecolor;
                    selectedapp.appTileTextOpacity = apptileopac;
                    break;
                case "Logo Color":
                    selectedapp.appTileLogoColor = apptilecolor;
                    selectedapp.appTileLogoOpacity = apptileopac;
                    break;
                case "Background Color":
                    selectedapp.appTileBackgroundColor = apptilecolor;
                    selectedapp.appTileBackgroundOpacity = apptileopac;
                    break;
                default:
                    break;
            }
            TestApps.Items.Add(selectedapp);

        }

        private void SaveChanges_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!allapps)
            {
                int appselected = packageHelper.appTiles.IndexOf(packageHelper.appTiles.FirstOrDefault(x => x.appTileFullName == selectedapp.appTileFullName));
                if (appselected > -1)
                {
                    packageHelper.appTiles[appselected] = selectedapp;
                }

            }
            else
            {
                switch (sectionofapp)
                {
                    case "Text Color":
                        for (int i = 0; i < packageHelper.appTiles.Count; i++)
                        {
                            packageHelper.appTiles[i].appTileTextColor = apptilecolor;
                            packageHelper.appTiles[i].appTileTextOpacity = apptileopac;
                        }
                        break;
                    case "Logo Color":
                        for (int i = 0; i < packageHelper.appTiles.Count; i++)
                        {
                            packageHelper.appTiles[i].appTileLogoColor = apptilecolor;
                            packageHelper.appTiles[i].appTileLogoOpacity = apptileopac;
                        }
                        break;
                    case "Background Color":
                        for (int i = 0; i < packageHelper.appTiles.Count; i++)
                        {
                            packageHelper.appTiles[i].appTileBackgroundColor = apptilecolor;
                            packageHelper.appTiles[i].appTileBackgroundOpacity = apptileopac;
                        }
                        break;
                    default:
                        break;
                }
            }
            NumofApps.SelectedIndex = -1;
            Appslist.IsHitTestVisible = false;
            Appslist.Visibility = Visibility.Collapsed;
            Appslist.SelectedIndex = -1;
            SectionofTile.SelectedIndex = -1;
            ChangeColorCombo.SelectedIndex = -1;
            OpactiytComboChange.SelectedIndex = -1;
            SectionofTile.IsHitTestVisible = false;
            ChangeColorCombo.IsHitTestVisible = false;
            OpactiytComboChange.IsHitTestVisible = false;
            Preview.IsHitTestVisible = false;
            SaveChanges.IsHitTestVisible = false;
            TestApps.Items.Clear();

        }



        private void TrackCrash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrackCrash.SelectedIndex > -1)
            {
                ComboBoxItem crash = (ComboBoxItem)TrackCrash.SelectedItem;
                crashreporting = string.Equals(crash.Content, "Yes") ? true : false;


            }

        }
        private void TrackNavigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrackNavigation.SelectedIndex > -1)
            {
                ComboBoxItem navitem = (ComboBoxItem)TrackNavigation.SelectedItem;
                crashreporting = string.Equals(navitem.Content, "Yes") ? true : false;
            }
        }
        private void ForgroundColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ForgroundColor.SelectedIndex > -1)
            {
                apptextcolor = (string)ForgroundColor.Items[ForgroundColor.SelectedIndex];

            }
        }

        private void ForgroundOpacity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ForgroundOpacity.SelectedIndex > -1)
            {
                apptextopac = (string)ForgroundOpacity.Items[ForgroundOpacity.SelectedIndex];
            }
        }

        private void BackgroundColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackgroundColor.SelectedIndex > -1)
            {
                appbackcolor = (string)BackgroundColor.Items[BackgroundColor.SelectedIndex];
            }
        }

        private void BackgroundOpacity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackgroundOpacity.SelectedIndex > -1)
            {
                appbackopac = (string)BackgroundOpacity.Items[BackgroundOpacity.SelectedIndex];
            }
        }

        private void BorderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BorderColor.SelectedIndex > -1)
            {
                appbordercolor = (string)BorderColor.Items[BorderColor.SelectedIndex];
            }
        }

        private void BorderOpacity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BorderOpacity.SelectedIndex > -1)
            {
                appborderopac = (string)BorderOpacity.Items[BorderOpacity.SelectedIndex];
            }
        }

        private void SaveSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SettingsHelper.totalAppSettings.disableCrashReporting = crashreporting;
            SettingsHelper.totalAppSettings.disableAnalytics = anaylitcreporting;
            SettingsHelper.totalAppSettings.appForgroundColor = string.IsNullOrEmpty(apptextcolor) ? "Orange" : apptextcolor;
            SettingsHelper.totalAppSettings.appForegroundOpacity = string.IsNullOrEmpty(apptextopac) ? "255" : apptextopac;
            SettingsHelper.totalAppSettings.appBackgroundColor = string.IsNullOrEmpty(appbackcolor) ? "Blue" : appbackcolor;
            SettingsHelper.totalAppSettings.appBackgroundOpacity = string.IsNullOrEmpty(appbackopac) ? "255" : appbackopac;
            SettingsHelper.totalAppSettings.appBorderColor = string.IsNullOrEmpty(appbordercolor) ? "Black" : appbordercolor;
            ForgroundColor.SelectedIndex = -1;
            ForgroundOpacity.SelectedIndex = -1;
            BackgroundColor.SelectedIndex = -1;
            BackgroundOpacity.SelectedIndex = -1;
            BorderColor.SelectedIndex = -1;
            SettingsHelper.SetApplicationResources();
            SettingsHelper.SaveAppSettingsAsync().ConfigureAwait(true);
            Frame.Navigate(typeof(SettingsPage));

        }


    }
}
