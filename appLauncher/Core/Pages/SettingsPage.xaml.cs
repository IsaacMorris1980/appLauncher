using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.Collections.Generic;
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
        //private List<DisplayImages> displayImages = new List<DisplayImages>();
        private bool allapps = false;
        private FinalTiles selectedapp;
        private string sectionofapp;
        private string Appscolor;
        private string apptextcolor;
        private string appbackcolor;
        string AppToggleTip = $"Change settings{Environment.NewLine}On:  All apps settings{Environment.NewLine}Off:  Only Single app settings";
        string ReportToggleTip = $"Enable reporting?{Environment.NewLine}On:  Crashes and Navigation information is reported{Environment.NewLine}Off: Nothing reported";


        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void AddButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            try
            {

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
            catch (Exception)
            {

            }

        }

        private void RemoveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {

                ImageHelper.RemovePageBackground(((PageBackgrounds)imagelist.SelectedItem).BackgroundImageDisplayName);

            }
            catch (Exception)
            {

            }
        }

        private void Appslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Appslist.SelectedIndex > -1)
            {
                selectedapp = (FinalTiles)Appslist.SelectedItem;
            }
        }

        private void Preview_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TestApps.Items.Add(selectedapp);
        }

        private void SaveChanges_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (allapps)
            {
                for (int i = 0; i < PackageHelper.Apps.GetOriginalCollection().Count; i++)
                {
                    ((FinalTiles)PackageHelper.Apps.GetOriginalCollection()[i]).TextColor = selectedapp.TextColor;
                    ((FinalTiles)PackageHelper.Apps.GetOriginalCollection()[i]).LogoColor = selectedapp.LogoColor;
                    ((FinalTiles)PackageHelper.Apps.GetOriginalCollection()[i]).BackColor = selectedapp.BackColor;
                }
                ResetAppTilePage();
                return;
            }
            int appselected = PackageHelper.Apps.IndexOf(PackageHelper.Apps.FirstOrDefault(x => x.Name == selectedapp.Name));
            if (appselected > -1)
            {
                PackageHelper.Apps.GetOriginalCollection()[appselected] = selectedapp;
            }
            ResetAppTilePage();
        }

        private void ResetAppTilePage()
        {
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
            int time;
            int.TryParse(ChangeTime.Text, out time);
            SettingsHelper.totalAppSettings.ImageRotationTime = (time <= 0) ? TimeSpan.FromSeconds(15) : TimeSpan.FromSeconds(time);
            SettingsHelper.SetApplicationResources();
            SettingsHelper.SaveAppSettingsAsync().ConfigureAwait(true);
            Frame.Navigate(typeof(SettingsPage));
        }

        private void AppSettings_Toggled(object sender, RoutedEventArgs e)
        {
            bool ison = ((ToggleSwitch)sender).IsOn;
            Appslist.Visibility = ison == true ? Visibility.Collapsed : Visibility.Visible;
            Appslist.IsHitTestVisible = !ison;
            Preview.IsHitTestVisible = true;
            TileLogoColor.IsHitTestVisible = true;
            TileTextColor.IsHitTestVisible = true;
            TileBackColor.IsHitTestVisible = true;
            TileBackOpacity.IsHitTestVisible = true;
            LogoOpacity.IsHitTestVisible = true;
            TileTextOpacity.IsHitTestVisible = true;
            selectedapp = (FinalTiles)(ison == true ? PackageHelper.SearchApps[0] : null);
            TestApps.Visibility = Visibility.Visible;
            TestApps.IsHitTestVisible = true;
            allapps = ison == true ? true : false;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)

        {


        }

        private void AboutPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }





        private void TileLogoColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedapp.LogoColor = (((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName).ToColor();
        }
        private void LogoOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

            Color c = Color.FromArgb(Convert.ToByte((((double)e.NewValue / 10) * 255)), selectedapp.LogoColor.R, selectedapp.LogoColor.G, selectedapp.LogoColor.B);
            selectedapp.LogoColor = c;
        }
        private void TileBackColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            selectedapp.BackColor = (((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName).ToColor();
        }
        private void TileBackOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Color backcolor = Color.FromArgb(Convert.ToByte((((double)e.NewValue / 10) * 255)), selectedapp.BackColor.R, selectedapp.BackColor.G, selectedapp.BackColor.B);
            selectedapp.BackColor = backcolor;
        }
        private void TileTextColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedapp.TextColor = (((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName).ToColor();
        }

        private void TileTextOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

            Color f = Color.FromArgb(Convert.ToByte(((double)e.NewValue / 10) * 255), selectedapp.TextColor.R, selectedapp.TextColor.G, selectedapp.TextColor.B);
            selectedapp.TextColor = f;
        }
        private void ApplicationTextColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsHelper.totalAppSettings.AppForgroundColor = (((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName).ToColor();
        }

        private void ApplicationTextOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Color f = Color.FromArgb(Convert.ToByte((((double)e.NewValue / 10) * 255)), SettingsHelper.totalAppSettings.AppForgroundColor.R, SettingsHelper.totalAppSettings.AppForgroundColor.G, SettingsHelper.totalAppSettings.AppForgroundColor.B);
            SettingsHelper.totalAppSettings.AppForgroundColor = f;
        }
        private void ApplicationBackColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsHelper.totalAppSettings.AppBackgroundColor = (((ColorComboItem)((ComboBox)sender).SelectedItem).ColorName).ToColor();
        }
        private void ApplicationBackOpacity_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Color f = Color.FromArgb(Convert.ToByte((((double)e.NewValue / 10) * 255)), SettingsHelper.totalAppSettings.AppBackgroundColor.R, SettingsHelper.totalAppSettings.AppBackgroundColor.G, SettingsHelper.totalAppSettings.AppBackgroundColor.B);
            SettingsHelper.totalAppSettings.AppBackgroundColor = f;
        }




    }
}


