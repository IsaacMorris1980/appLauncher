using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
        private Apps selectedapp;
        private string sectionofapp;
        private string Appscolor;
        private string apptextcolor;
        private string appbackcolor;
        string AppToggleTip = $"Change settings on.{Environment.NewLine}On:  All apps settings {Environment.NewLine}Off:  Only Single app settings";
        string CrashToggleTip = $"Disable Crash Reporting?{Environment.NewLine}On:  Crashes are not reported{Environment.NewLine}Off:  Crashes are reported";
        string AnalyticsToggleTip = $"Disable Analytic Reporting.{Environment.NewLine}On:  Analytics/Navigation not reported{Environment.NewLine}Off: Analytics/Navigation is reported";
        private bool crashreporting = true;
        private bool anaylitcreporting = true;

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

                    if (SettingsHelper.totalAppSettings.BgImagesAvailable)
                    {
                        foreach (StorageFile item in file)
                        {
                            ImageHelper.AddPageBackround(pageBackgrounds: new PageBackgrounds
                            {
                                BackgroundImageDisplayName = item.DisplayName,

                                filepath = item.Path,

                                BackgroundImageBytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(filename: item)
                            });



                        }
                    }
                    else
                    {
                        foreach (var item in file)
                        {
                            ImageHelper.AddPageBackround(pageBackgrounds: new PageBackgrounds
                            {
                                BackgroundImageDisplayName = item.DisplayName,
                                BackgroundImageBytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(filename: item)
                            });
                        }

                        SettingsHelper.totalAppSettings.BgImagesAvailable = true;
                    }

                }
                else
                {
                    Debug.WriteLine("Operation cancelled.");
                }
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Exception occured while adding background");
                Crashes.TrackError(ex);
            }
        }

        private void RemoveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (imagelist.SelectedIndex != -1)
                {
                    DisplayImages displ = (DisplayImages)imagelist.SelectedItem;
                    ImageHelper.RemovePageBackground(displ.displayName);
                    imagelist.Items.Remove(imagelist.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void Appslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Appslist.SelectedIndex > -1)
            {
                selectedapp = (Apps)Appslist.SelectedItem;
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
                int appselected = packageHelper.Apps.IndexOf(packageHelper.Apps.FirstOrDefault(x => x.FullName == selectedapp.FullName));
                if (appselected > -1)
                {
                    packageHelper.Apps.GetOriginalCollection()[appselected] = selectedapp;
                }

            }
            else
            {
                ObservableCollection<Apps> packs = packageHelper.Apps.GetOriginalCollection();
                for (int i = 0; i < packageHelper.Apps.GetOriginalCollection().Count; i++)
                {
                    packageHelper.Apps.GetOriginalCollection()[i].TextColor = selectedapp.TextColor;
                    packageHelper.Apps.GetOriginalCollection()[i].LogoColor = selectedapp.LogoColor;
                    packageHelper.Apps.GetOriginalCollection()[i].BackColor = selectedapp.BackColor;
                }
            }

            Appslist.IsHitTestVisible = false;
            Appslist.Visibility = Visibility.Collapsed;
            Appslist.SelectedIndex = -1;
            Preview.IsHitTestVisible = false;
            SaveChanges.IsHitTestVisible = false;
            TestApps.Items.Clear();

        }











        private void SaveSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SettingsHelper.totalAppSettings.disableCrashReporting = crashreporting;
            SettingsHelper.totalAppSettings.disableAnalytics = anaylitcreporting;
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
                selectedapp = packageHelper.searchApps[0];
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
                Appslist.Visibility = Visibility.Visible;
                Appslist.IsHitTestVisible = true;
                TestApps.Visibility = Visibility.Visible;
                TestApps.IsHitTestVisible = true;
                allapps = false;
            }
        }

        private void AppsLogoColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args != null)
            {
                if (args.NewColor.A == 0)
                {
                    selectedapp.LogoColor = Colors.Transparent;
                }
                else
                {
                    selectedapp.LogoColor = args.NewColor;
                }

            }
        }

        private void AppsBackgroundColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args != null)
            {
                if (args.NewColor.A == 0)
                {
                    selectedapp.BackColor = Colors.Transparent;
                }
                else
                {
                    selectedapp.BackColor = args.NewColor;
                }

            }
        }

        private void AppsTextColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args != null)
            {
                if (args.NewColor.A == 0)
                {
                    selectedapp.TextColor = Colors.Transparent;
                }
                else
                {
                    selectedapp.TextColor = args.NewColor;
                }

            }
            selectedapp.TextColor = (args != null) ? args.NewColor : selectedapp.TextColor;
        }

        private void TrackCrash_Toggled(object sender, RoutedEventArgs e)
        {
            SettingsHelper.totalAppSettings.disableCrashReporting = TrackCrash.IsOn;
        }

        private void TrackNavigation_Toggled(object sender, RoutedEventArgs e)
        {
            SettingsHelper.totalAppSettings.disableAnalytics = TrackNavigation.IsOn;
        }

        private void AppTextColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args != null)
            {
                if (args.NewColor.A == 0)
                {
                    SettingsHelper.totalAppSettings.appForgroundColor = Colors.Transparent;
                }
                else
                {
                    SettingsHelper.totalAppSettings.appForgroundColor = args.NewColor;
                }


            }
        }

        private void AppBackgroundColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args != null)
            {
                if (args.NewColor.A == 0)
                {
                    SettingsHelper.totalAppSettings.appBackgroundColor = Colors.Transparent;
                }
                else
                {
                    SettingsHelper.totalAppSettings.appBackgroundColor = args.NewColor;
                }

            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)

        {




            selectedapp = packageHelper.Apps.GetOriginalCollection()[0];
            SettingsHelper.totalAppSettings.ShowApps = !AppSettings.IsOn;
            Appslist.Visibility = (SettingsHelper.totalAppSettings.ShowApps == true) ? Visibility.Visible : Visibility.Collapsed;
            Appslist.IsHitTestVisible = SettingsHelper.totalAppSettings.ShowApps;
        }

        private void AboutPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}
}
