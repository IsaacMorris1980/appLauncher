using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using appLauncher.Model;
using System.Collections.ObjectModel;
using appLauncher.Helpers;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Nito.AsyncEx;

namespace appLauncher
{
    public static class GlobalVariables
    {
        public static int appsperscreen { get; set; }
        public static finalAppItem itemdragged { get; set; }
        public static int columns { get; set; }
        public static int oldindex { get; set; }
        public static int newindex { get; set; }
        public static int pagenum { get; set; }
        public static bool isdragging { get; set; }
        public static bool isSavingImages { get; set; }
        public static bool isSavingCollection { get; set; }
        public static bool bgimagesavailable { get; set; }
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public static ObservableCollection<string> backgroundimagenames { get; set; }
        public static ObservableCollection<BitmapImage> backgroundImage { get; set; }
        public static Point startingpoint { get; set; }
        
                

        public static async Task Logging(string texttolog)
        {
            StorageFolder stors = ApplicationData.Current.LocalFolder;
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), texttolog);
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), Environment.NewLine);
        }
        public static int NumofRoworColumn(int padding, int objectsize, int sizetofit)
        {
            int amount = 0;
            int intsize = objectsize + (padding + padding);
            int size = intsize;
            while (size + intsize < sizetofit)
            {
                amount += 1;
                size += intsize;
            }
            return amount;

        }
        public static async Task LoadCollectionAsync()
        {

              List<finalAppItem> oc1 = AllApps.listOfApps.ToList();
            ObservableCollection<finalAppItem> oc = new ObservableCollection<finalAppItem>();
            if (await IsFilePresent("collection.txt"))
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
               var apps = await FileIO.ReadLinesAsync(item);
                if (apps.Count() > 1)
                {
                    foreach (string y in apps)
                    {
                        foreach (finalAppItem items in oc1)
                        {
                            if (items.appEntry.DisplayInfo.DisplayName == y)
                            {
                                oc.Add(items);
                            }
                        }
                    }
                }
                AllApps.listOfApps = (oc.Count > 0) ? oc : new ObservableCollection<finalAppItem>(oc1);
            }





        }
        public static async Task<bool> SaveCollectionAsync()
        {
            List<finalAppItem> finals = AllApps.listOfApps.ToList();

            var te = from x in finals select x.appEntry.DisplayInfo.DisplayName;
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteLinesAsync(item, te);
            return true;

        }
        public static async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }
       public static async Task LoadBackgroundImages()
       {
            if (await IsFilePresent("images.txt") && bgimagesavailable)
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                var images = await FileIO.ReadLinesAsync(item);
                backgroundImage = new ObservableCollection<BitmapImage>();
                backgroundimagenames = new ObservableCollection<string>();
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                if (images.Any())
                {
                    foreach (string y in images)
                    {
                        foreach (var items in filesInFolder)
                        {
                            if (items.DisplayName==y)
                            {
                                backgroundImage.Add(new BitmapImage(new Uri(items.Path)));
                                backgroundimagenames.Add(y);
                            }
                        }
                    }
                }
              
            }
            else if (bgimagesavailable)
           {
                backgroundImage = new ObservableCollection<BitmapImage>();
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                if (filesInFolder.Any())
                {
                    foreach (var item in filesInFolder)
                    {
                        backgroundImage.Add(new BitmapImage(new Uri(item.Path)));
                        backgroundimagenames.Add(item.DisplayName);
                    }
                }
                

            }
            
        }

        public static async Task<bool> SaveImageOrder()
        {
          
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteLinesAsync(item, backgroundimagenames);
            return true;
        }

    }
}
