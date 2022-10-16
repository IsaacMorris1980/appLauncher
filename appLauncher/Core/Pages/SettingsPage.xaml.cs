using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using Microsoft.AppCenter.Crashes;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Pages
{
    /// <summary>
    /// Page where the launcher settings are configured
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public AppTile testapp;
        string appcoloritems;
        string appcolor;
        string appopacity;
        int selctedint;
        public SettingsPage()
        {
            this.InitializeComponent();
        }
        private ObservableCollection<AppTile> settingsapps = new ObservableCollection<AppTile>(packageHelper.searchApps.ToList());

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void RemoveButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (imagelist.SelectedItems.Count != -1)
            {
                List<PageBackgrounds> bi = (List<PageBackgrounds>)imagelist.SelectedItems;
                foreach (var item in bi)
                {
                    ImageHelper.backgroundImage.Remove(item);
                }

                StorageFolder backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                IReadOnlyList<StorageFile> filesinfolder = await backgroundImageFolder.GetFilesAsync();
                var foundfiles = filesinfolder.Where(x => bi.Any(y => y.pageBackgroundImageDisplayName == x.DisplayName));
                foreach (var item in foundfiles)
                {
                    await item.DeleteAsync();
                }
            }
        }


        private async void AddButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            try
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

                    var exits = file.Where(x => !ImageHelper.backgroundImage.Any(y => y.pageBackgroundImageDisplayName == x.DisplayName));

                    if (exits.Count() > 0)
                    {
                        foreach (var item in exits)
                        {
                            PageBackgrounds pageback = new PageBackgrounds();
                            pageback.pageBackgroundImageDisplayName = item.DisplayName;
                            pageback.pageBackgroundImageFullPathLocation = item.Path;
                            pageback.pageBackgroundImageBytes = await ImageHelper.ConvertImageFiletoByteArrayAsync(item);
                            pageback.pageBackgroundDisplayImage = await ImageHelper.ConvertfromByteArraytoBitmapImage(pageback.pageBackgroundImageBytes);
                            ImageHelper.backgroundImage.Add(pageback);
                            //ImageHelper.backgroundImage.Add(pageback);
                            ////   await item.CopyAsync(backgroundImageFolder);

                        }

                    }





                    await ImageHelper.SaveImageOrder();
                }
                else
                {
                    Debug.WriteLine("Operation cancelled.");
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        private void AppTileColorItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppTileColorItems.SelectedIndex > -1)
            {
                appcoloritems = ((ComboBoxItem)AppTileColorItems.SelectedItem).Content.ToString();
                switch (appcoloritems)
                {
                    case "Text Color":
                        ColorCombo.Text = testapp.appTileTextColor;
                        OpacityCombo.Text = testapp.appTileTextOpacity;
                        break;
                    case "Logo Color":
                        OpacityCombo.Text = testapp.appTileLogoOpacity;
                        ColorCombo.Text = testapp.appTileLogoColor;
                        break;
                    case "Background Color":
                        ColorCombo.Text = testapp.appTileBackgroundColor;
                        OpacityCombo.Text = testapp.appTileBackgroundOpacity;
                        break;
                    default:
                        break;
                }
                ColorCombo.IsEnabled = true;
                settingsinappnotifications.Show("Select item color", 1500);
            }

        }

        private async void SaveChanges_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

            int index = packageHelper.appTiles.IndexOf(packageHelper.appTiles.First(x => x.appTileFullName == testapp.appTileFullName));
            if (index > -1)
            {
                packageHelper.appTiles[index] = testapp;
            }
            await packageHelper.SaveCollectionAsync();
            PreviewChanges.IsEnabled = false;
            SaveChanges.IsEnabled = false;
            AppTileColorItems.IsEnabled = false;
            ColorCombo.IsEnabled = false;
            OpacityCombo.IsEnabled = false;
        }

        private void appCarosel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (appCarosel.SelectedIndex > -1)
            {
                testapp = (AppTile)appCarosel.SelectedItem;
                selctedint = (int)appCarosel.SelectedIndex;
                TestApps.Items.Clear();
                TestApps.Items.Add(testapp);
                AppTileColorItems.IsEnabled = true;
                settingsinappnotifications.Show("Select app color area", 1500);
                return;
            }


        }

        private void OpacityCombo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = SettingsHelper.totalAppSettings.AppOpacity.Where(p => p.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void ColorCombo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = SettingsHelper.totalAppSettings.AppColors.Where(p => p.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void ColorCombo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            appcolor = (string)args.SelectedItem;
            sender.Text = (string)args.SelectedItem;
            OpacityCombo.IsEnabled = true;
            settingsinappnotifications.Show("Select opacity 0 transparent and 255 is fully opaque", 1500);
        }
        private void OpacityCombo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            appopacity = (string)args.SelectedItem;
            sender.Text = (string)args.SelectedItem;
            settingsinappnotifications.Show("If you like these changes click save", 1500);
            PreviewChanges.IsEnabled = true;
        }

        private void PreviewChanges_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            switch (appcoloritems)
            {
                case "Text Color":
                    testapp.appTileTextOpacity = appopacity;
                    testapp.appTileTextColor = appcolor;
                    break;
                case "Logo Color":
                    testapp.appTileLogoOpacity = appopacity;
                    testapp.appTileLogoColor = appcolor;
                    break;
                case "Background Color":
                    testapp.appTileBackgroundOpacity = appopacity;
                    testapp.appTileBackgroundColor = appcolor;
                    break;
                default:
                    break;
            }
            testapp = (AppTile)appCarosel.Items[selctedint];
            TestApps.Items.Clear();
            TestApps.Items.Add(testapp);
            AppTileColorItems.SelectedIndex = -1;
            ColorCombo.IsEnabled = false;
            OpacityCombo.IsEnabled = false;
            SaveChanges.IsEnabled = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            bool propertyChanged = false;

            //If we have a different value, do stuff
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
                propertyChanged = true;
            }

            return propertyChanged;
        }

        //The C#6 version of the common implementation
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async void TrackCrash_ToggledAsync(object sender, RoutedEventArgs e)
        {
            if (!TrackCrash.IsOn)
            {
                SettingsHelper.totalAppSettings.disableCrashReporting = false;
                await SettingsHelper.CheckAppSettings();
            }
            else
            {
                SettingsHelper.totalAppSettings.disableCrashReporting = true;
                await SettingsHelper.CheckAppSettings();
            }
        }

        private async void TrackNavigation_ToggledAsync(object sender, RoutedEventArgs e)
        {
            if (!TrackNavigation.IsOn)
            {
                SettingsHelper.totalAppSettings.disableAnalytics = false;
                await SettingsHelper.CheckAppSettings();
            }
            else
            {
                SettingsHelper.totalAppSettings.disableAnalytics = true;
                await SettingsHelper.CheckAppSettings();
            }
        }

        private void ForgroundColor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = SettingsHelper.totalAppSettings.AppColors.Where(p => p.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void ForgroundColor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SettingsHelper.totalAppSettings.appForgroundColor = (string)args.SelectedItem;
            sender.Text = (string)args.SelectedItem;
        }

        private void ForgroundOpacity_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = SettingsHelper.totalAppSettings.AppOpacity.Where(p => p.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void ForgroundOpacity_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SettingsHelper.totalAppSettings.appForegroundOpacity = (string)args.SelectedItem;
            sender.Text = (string)args.SelectedItem;
        }

        private void BackgroundColor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = SettingsHelper.totalAppSettings.AppColors.Where(p => p.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void BackgroundColor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SettingsHelper.totalAppSettings.appBackgroundColor = (string)args.SelectedItem;
            sender.Text = (string)args.SelectedItem;
        }

        private void BackgroundOpacity_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                sender.ItemsSource = SettingsHelper.totalAppSettings.AppOpacity.Where(p => p.Contains(auto.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void BackgroundOpacity_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SettingsHelper.totalAppSettings.appBackgroundOpacity = (string)args.SelectedItem;
            sender.Text = (string)args.SelectedItem;
        }
    }
}
