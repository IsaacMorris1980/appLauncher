using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using UwpAppLauncher.Model;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UwpAppLauncher.Helpers
{
    public sealed class SettingsHelper
    {
        public GlobalAppSettings _appSettings = new GlobalAppSettings();
        public async Task LoadSettings()
        {
            if (await ((App)Application.Current).globalVariables.IsFilePresent("globalappsettings.json"))
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("globalappsettings.json");
                if (item != null)
                {
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    _appSettings = JsonConvert.DeserializeObject<GlobalAppSettings>(apps);
                    if (!string.Equals(_appSettings.AppVersion, GetAppVersion()))
                    {
                        await ((App)Application.Current).ConvertolderApp();
                        await ((App)Application.Current).CheckAppSettings();

                    }
                    else
                    {
                        _appSettings = new GlobalAppSettings();
                        ((App)Application.Current).globalVariables = new GlobalVariables();
                        await ((App)Application.Current).packageHelper.LoadCollectionAsync();
                        await ((App)Application.Current).imageHelper.LoadBackgroundImages();
                        await ((App)Application.Current).CheckAppSettings();
                    }

                }


            }
        }
        public async Task SaveSettings()
        {

            string settingstring = JsonConvert.SerializeObject(_appSettings, Formatting.Indented);
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("globalappsettings.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(item, settingstring);
        }
        public string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
