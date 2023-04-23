using appLauncher.Core.Enums;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class CrashAnalyticsHelper
    {
        public static async Task LoggingCrash(Exception itemtolog)
        {

            switch (SettingsHelper.totalAppSettings.crasherror)
            {
                case ErrorType.Crashes:
                    Crashes.TrackError((Exception)itemtolog);
                    break;
                case ErrorType.Analytics:
                    break;
                case ErrorType.Both:
                    break;
                case ErrorType.File:
                    StorageFile errorfile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("errors.json", CreationCollisionOption.OpenIfExists);
                    string errorstr = itemtolog.ToString() + Environment.NewLine + Environment.NewLine;
                    await FileIO.WriteTextAsync(errorfile, errorstr);
                    break;
                default:
                    break;
            }



        }
        public static async Task LoggingAnalytics(string itemtolog)
        {
            switch (SettingsHelper.totalAppSettings.analyticerror)
            {
                case ErrorType.Crashes:
                    break;
                case ErrorType.Analytics:
                    Analytics.TrackEvent(itemtolog);
                    break;
                case ErrorType.Both:
                    break;
                case ErrorType.File:
                    StorageFile analyticsfile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("analytics.json", CreationCollisionOption.OpenIfExists);
                    string analyticsstr = itemtolog + Environment.NewLine + Environment.NewLine;
                    await FileIO.WriteTextAsync(analyticsfile, analyticsstr);
                    break;
                default:
                    break;
            }
        }
    }
}
