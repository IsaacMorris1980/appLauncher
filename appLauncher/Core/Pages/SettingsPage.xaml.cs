using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
        private List<DisplayImages> displayImages = new List<DisplayImages>();
        private bool allapps = false;
        private AppTiles selectedapp;
        private string sectionofapp;
        private string Appscolor;
        private string apptextcolor;
        private string appbackcolor;
        string AppToggleTip = $"Change settings on.{Environment.NewLine}On:  All apps settings {Environment.NewLine}Off:  Only Single app settings";
        string ReportToggleTip = $" Enable crash reporting?{Environment.NewLine}On:  Crashes and Navigation information is reported{Environment.NewLine}Off: Nothing reported";


        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void AddButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (!SettingsHelper.totalAppSettings.Images)
                {
                    return;
                }
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
                };
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
                IReadOnlyList<StorageFile> file = await picker.PickMultipleFilesAsync();
                if (file.Any())
                {
                    foreach (StorageFile item in file)
                    {
                        ImageHelper.AddPageBackround(pageBackgrounds: new PageBackgrounds
                        {
                            BackgroundImageDisplayName = item.DisplayName,
                            FilePath = item.Path,
                            BackgroundImageBytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(fileName: item)
                        });
                    }
                }
                else
                {
                    Debug.WriteLine("Operation cancelled.");
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void RemoveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (!SettingsHelper.totalAppSettings.Images)
                {
                    return;
                }
                if (imagelist.SelectedIndex != -1)
                {
                    DisplayImages displ = (DisplayImages)imagelist.SelectedItem;
                    ImageHelper.RemovePageBackground(displ.DisplayName);
                    imagelist.Items.Remove(imagelist.SelectedItem);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Appslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Appslist.SelectedIndex > -1)
            {
                selectedapp = (AppTiles)Appslist.SelectedItem;
            }
        }


        private void Preview_Tapped(object sender, TappedRoutedEventArgs e)
        {

            TestApps.Items.Add(selectedapp);

        }

        private void SaveChanges_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!allapps)
            {
                int appselected = PackageHelper.Apps.IndexOf(PackageHelper.Apps.FirstOrDefault(x => x.FullName == selectedapp.FullName));
                if (appselected > -1)
                {
                    PackageHelper.Apps.GetOriginalCollection()[appselected] = selectedapp;
                }

            }
            else
            {
                ObservableCollection<AppTiles> packs = PackageHelper.Apps.GetOriginalCollection();
                for (int i = 0; i < PackageHelper.Apps.GetOriginalCollection().Count; i++)
                {
                    PackageHelper.Apps.GetOriginalCollection()[i].TextColor = selectedapp.TextColor;
                    PackageHelper.Apps.GetOriginalCollection()[i].LogoColor = selectedapp.LogoColor;
                    PackageHelper.Apps.GetOriginalCollection()[i].BackColor = selectedapp.BackColor;
                }
            }

            Appslist.IsHitTestVisible = false;
            Appslist.Visibility = Visibility.Collapsed;
            Appslist.SelectedIndex = -1;
            Preview.IsHitTestVisible = false;
            SaveChanges.IsHitTestVisible = false;
            TileLogoColor.IsHitTestVisible = false;
            TileTextColor.IsHitTestVisible = false;
            TileBackColor.IsHitTestVisible = false;
            TileBackOpacity.IsHitTestVisible = false;
            LogoOpacity.IsHitTestVisible = false;
            TileTextOpacity.IsHitTestVisible = false;
            TestApps.Items.Clear();

        }











        private void SaveSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int time = 0;
            if (int.TryParse(ChangeTime.Text, out time))
            {


                if (time <= 0)
                {
                    SettingsHelper.totalAppSettings.ImageRotationTime = TimeSpan.FromSeconds(15);
                }
                else
                {
                    SettingsHelper.totalAppSettings.ImageRotationTime = TimeSpan.FromSeconds(time);
                }
            }
            else
            {
                SettingsHelper.totalAppSettings.ImageRotationTime = TimeSpan.FromSeconds(15);
            }

            SettingsHelper.SetApplicationResources();
            SettingsHelper.SaveAppSettingsAsync().ConfigureAwait(true);
            Frame.Navigate(typeof(SettingsPage));

        }

        private void AppSettings_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
            {
                SettingsHelper.totalAppSettings.ShowApps = !((ToggleSwitch)sender).IsOn;
                Appslist.Visibility = Visibility.Collapsed;
                Appslist.IsHitTestVisible = false;
                Preview.IsHitTestVisible = true;
                TileLogoColor.IsHitTestVisible = true;
                TileTextColor.IsHitTestVisible = true;
                TileBackColor.IsHitTestVisible = true;
                TileBackOpacity.IsHitTestVisible = true;
                LogoOpacity.IsHitTestVisible = true;
                TileTextOpacity.IsHitTestVisible = true;
                selectedapp = PackageHelper.SearchApps[0];
                TestApps.Visibility = Visibility.Visible;
                TestApps.IsHitTestVisible = true;
                allapps = true;
            }
            else
            {
                SettingsHelper.totalAppSettings.ShowApps = !((ToggleSwitch)sender).IsOn;
                Appslist.Visibility = Visibility.Visible;
                Appslist.IsHitTestVisible = true;
                Preview.IsHitTestVisible = true;
                TileLogoColor.IsHitTestVisible = true;
                TileTextColor.IsHitTestVisible = true;
                TileBackColor.IsHitTestVisible = true;
                TileBackOpacity.IsHitTestVisible = true;
                LogoOpacity.IsHitTestVisible = true;
                TileTextOpacity.IsHitTestVisible = true;
                Appslist.Visibility = Visibility.Visible;
                Appslist.IsHitTestVisible = true;
                TestApps.Visibility = Visibility.Visible;
                TestApps.IsHitTestVisible = true;
                allapps = false;
            }
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)

        {
            //selectedapp = packageHelper.Apps.GetOriginalCollection()[0];
            //SettingsHelper.totalAppSettings.ShowApps = !AppSettings.IsOn;
            //Appslist.Visibility = (SettingsHelper.totalAppSettings.ShowApps == true) ? Visibility.Visible : Visibility.Collapsed;
            //Appslist.IsHitTestVisible = SettingsHelper.totalAppSettings.ShowApps;
            if (SettingsHelper.totalAppSettings.Reporting)
            {
                await ((App)Application.Current).reportScreenViews.CollectScreenViews("Settings");
            }

        }

        private void AboutPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }





        private void TileLogoColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string logocolor = ((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName;
            selectedapp.LogoColor = logocolor.ToColor();
        }
        private void LogoOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            double logoopacyity = e.NewValue;
            Color logoopacitycolor = selectedapp.LogoColor;
            logoopacitycolor.A = Convert.ToByte((double)(logoopacyity / 10) * 255);
            selectedapp.LogoColor = logoopacitycolor;
        }
        private void TileBackColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string tilebackcolor = ((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName;
            selectedapp.BackColor = tilebackcolor.ToColor();
        }
        private void TileBackOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            double bacoopacity = e.NewValue;
            double decimalbackopacity = bacoopacity / 10;
            Color backcolor = selectedapp.BackColor;
            backcolor.A = Convert.ToByte(decimalbackopacity * 255);
            selectedapp.BackColor = backcolor;
        }
        private void TileTextColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string c = ((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName;
            selectedapp.TextColor = c.ToColor();
        }
        private void TileTextOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            double c = e.NewValue;
            double d = c / 10;
            Color f = selectedapp.TextColor;
            f.A = Convert.ToByte(d * 255);
            selectedapp.TextColor = f;
        }
        private void ApplicationTextColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string c = ((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName;
            SettingsHelper.totalAppSettings.AppForgroundColor = c.ToColor();
        }

        private void ApplicationTextOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            double c = e.NewValue;
            double d = c / 10;
            Color f = SettingsHelper.totalAppSettings.AppForgroundColor;
            f.A = Convert.ToByte(d * 255);
            SettingsHelper.totalAppSettings.AppForgroundColor = f;
        }
        private void ApplicationBackColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string c = ((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName;
            SettingsHelper.totalAppSettings.AppBackgroundColor = c.ToColor();
        }
        private void ApplicationBackOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            double c = e.NewValue;
            double d = c / 10;
            Color f = SettingsHelper.totalAppSettings.AppBackgroundColor;
            f.A = Convert.ToByte(d * 255);
            SettingsHelper.totalAppSettings.AppBackgroundColor = f;
        }

        private void Searching_Toggled(object sender, RoutedEventArgs e)
        {
            if (!Searching.IsOn)
            {
                SettingsHelper.totalAppSettings.Search = false;
                PackageHelper.SearchApps.Clear();
                return;
            }
            PackageHelper.SearchApps = PackageHelper.Apps.OrderBy(x => x.Name).ToList();
            SettingsHelper.totalAppSettings.Search = true;
        }

        private void Filter_Toggled(object sender, RoutedEventArgs e)
        {
            if (!Filter.IsOn)
            {
                SettingsHelper.totalAppSettings.Filter = false;
                var order = PackageHelper.Apps.OrderBy(x => x.Name).ToList();
                PackageHelper.Apps = new AppPaginationObservableCollection(order);
                return;
            }
            SettingsHelper.totalAppSettings.Filter = true;
        }

        private async void BackImages_Toggled(object sender, RoutedEventArgs e)
        {
            if (!BackImages.IsOn)
            {
                await ImageHelper.SaveImageOrder();
                ImageHelper.backgroundImage.Clear();
                SettingsHelper.totalAppSettings.Images = false;
                return;
            }
            await ImageHelper.LoadBackgroundImages();
            SettingsHelper.totalAppSettings.Images = true;

        }

        private void Tiles_Toggled(object sender, RoutedEventArgs e)
        {
            if (!Tiles.IsOn)
            {
                SettingsHelper.totalAppSettings.Tiles = false;
                return;
            }
            SettingsHelper.totalAppSettings.Tiles = true;
        }

        private void LauncherSettings_Toggled(object sender, RoutedEventArgs e)
        {
            if (!LauncherSettings.IsOn)
            {
                var a = SettingsHelper.totalAppSettings;
                SettingsHelper.totalAppSettings = new GlobalAppSettings()
                {
                    Filter = a.Filter,
                    Search = a.Search,
                    Tiles = a.Tiles,
                    AppSettings = false,
                    AppsPerPage = a.AppsPerPage,
                    LastPageNumber = a.LastPageNumber,
                    Images = a.Images,
                    ShowApps = a.ShowApps
                };
                return;

            }
            SettingsHelper.totalAppSettings.AppSettings = true;
        }

        private void Report_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
            {
                SettingsHelper.totalAppSettings.Reporting = true;
            }
            else
            {
                SettingsHelper.totalAppSettings.Reporting = false;
            }
        }
    }
}


