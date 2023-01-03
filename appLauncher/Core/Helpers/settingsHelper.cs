using appLauncher.Core.Model;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;

namespace appLauncher.Core.Helpers
{
    public static class SettingsHelper
    {
        public static GlobalAppSettings totalAppSettings { get; set; } = new GlobalAppSettings();
        public static async Task<bool> IsFilePresent(string fileName, string folderpath = "")

        {
            IStorageItem item;
            if (folderpath == "")
            {
                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderpath);
                item = await folder.TryGetItemAsync(fileName);

            }

            return item != null;

        }

        public static async Task LoadAppSettingsAsync()
        {



            if (await SettingsHelper.IsFilePresent("GlobalAppSettings.txt"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("GlobalAppSettings.txt");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    GlobalAppSettings appsettings = JsonConvert.DeserializeObject<GlobalAppSettings>(apps);
                    appsettings.AppColors = ColorStructToList();
                    for (int i = 1; i < 256; i++)
                    {
                        appsettings.AppOpacity.Add(i.ToString());
                    }
                    totalAppSettings = appsettings;
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during reordering app list to last apps position");
                    Crashes.TrackError(e);
                }
            }
            else
            {
                GlobalAppSettings appSettings = new GlobalAppSettings();
                appSettings.AppColors = ColorStructToList();

                for (int i = 0; i < 256; i++)
                {
                    appSettings.AppOpacity.Add(i.ToString());
                }
                totalAppSettings = appSettings;

            }



        }
        public static async Task SaveAppSettingsAsync()
        {
            try
            {
                var te = JsonConvert.SerializeObject(totalAppSettings, Formatting.Indented);
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("GlobalAppSettings.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, te);
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during saving Global App Settings");
                Crashes.TrackError(es);
            }
        }
        public static List<string> ColorStructToList()
        {
            List<string> allcolors = new List<string>();
            foreach (var color in typeof(Colors).GetRuntimeProperties())
            {
                allcolors.Add(color.Name);
            }
            //Type color = typeof(Windows.UI.Colors);
            //var colors = Enum.GetValues(enumType: typeof(Windows.UI.Colors));


            //for (int i = 0; i < colors.Length; i++)
            //{
            //    allcolors.Add(colors.GetValue(i).ToString());
            //}

            return allcolors;
        }
        public static void ConfigureAppCenter()
        {
            AppCenter.Start("f3879d12-8020-4309-9fbf-71d9d24bcf9b", typeof(Crashes), typeof(Analytics));
            Crashes.SetEnabledAsync(false);
            Analytics.SetEnabledAsync(false);
        }
        public static async Task CheckAppSettings()
        {
            if (!AppCenter.Configured)
            {
                ConfigureAppCenter();

            }
            if (!SettingsHelper.totalAppSettings.disableCrashReporting)
            {
                await Crashes.SetEnabledAsync(true);
            }
            if (!SettingsHelper.totalAppSettings.disableAnalytics)
            {
                await Analytics.SetEnabledAsync(true);
            }
            if (SettingsHelper.totalAppSettings.disableCrashReporting)
            {
                await Crashes.SetEnabledAsync(false);
            }
            if (SettingsHelper.totalAppSettings.disableAnalytics)
            {
                await Analytics.SetEnabledAsync(false);
            }
        }
        public static void SetApplicationResources()
        {
            Application.Current.Resources["AppBarButtonForegroundPointerOver"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["AppBarButtonBackgroundPointerOver"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["AppBarButtonBorderBrushPointerOver"] = SettingsHelper.totalAppSettings.AppBorderColorBrush;
            Application.Current.Resources["ComboBoxBackground"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackgroundPointerOver"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxForeground"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["ComboBoxDropDownBackground"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxPlaceHolderForeground"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
        }

    }
}
