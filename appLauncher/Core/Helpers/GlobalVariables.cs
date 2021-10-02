using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using appLauncher.mobile.Core.Models;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using applauncher.mobile.Core.Model;
using System.Collections.Concurrent;

namespace appLauncher.mobile.Core.Helpers

{
    public static class GlobalVariables
    {
        public static int appsperscreen { get; set; }
        public static AppTile itemdragged { get; set; }
        public static int columns { get; set; }
        public static int oldindex { get; set; }
        public static int newindex { get; set; }
        public static int pagenum { get; set; }
        public static bool isdragging { get; set; }
        public static bool bgimagesavailable { get; set; }

        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public static ObservableCollection<BackgroundImages> backgroundImage { get; set; } = new ObservableCollection<BackgroundImages>();
        public static Point startingpoint { get; set; }

        public static ConcurrentQueue<AppTile> Apps { get; set; } = new ConcurrentQueue<AppTile>();
        public static ReadOnlyObservableCollection<AppTile> queriedApps
        {
            get
            {
                return new ReadOnlyObservableCollection<AppTile>(new ObservableCollection<AppTile>(Apps));
            }
        }

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
        //public static async Task LoadCollectionAsync()
        //{

        //    List<AppTile> oc1 = AllApps.listOfApps.ToList();
        //    ObservableCollection<AppTile> oc = new ObservableCollection<finalAppItem>();
        //    if (await IsFilePresent("collection.txt"))
        //    {

        //        StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
        //        var apps = await FileIO.ReadLinesAsync(item);
        //        if (apps.Count() > 1)
        //        {
        //            foreach (string y in apps)
        //            {
        //                foreach (finalAppItem items in oc1)
        //                {
        //                    if (items.appEntry.DisplayInfo.DisplayName == y)
        //                    {
        //                        oc.Add(items);
        //                    }
        //                }
        //            }
        //        }
        //        AllApps.listOfApps = (oc.Count > 0) ? oc : new ObservableCollection<finalAppItem>(oc1);
        //    }





        //}
        public static async Task SaveCollectionAsync()
        {


            var te = JsonConvert.SerializeObject(GlobalVariables.Apps, Formatting.Indented);
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(item, te);


        }
        public static async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }
        public static async Task LoadBackgroundImages()
        {

            if (await IsFilePresent("images.txt"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                    GlobalVariables.backgroundImage = JsonConvert.DeserializeObject<ObservableCollection<BackgroundImages>>(await FileIO.ReadTextAsync(item));
                    //var images = await FileIO.ReadLinesAsync(item);
                    //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    //var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                    //var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    //if (images.Count() > 0)
                    //{
                    //    foreach (string y in images)
                    //    {
                    //        foreach (var items in filesInFolder)
                    //        {
                    //            if (items.DisplayName == y)
                    //            {
                    //                BackgroundImages bi = new BackgroundImages();
                    //                bi.Filename = items.DisplayName;
                    //                bi.Bitmapimage = new BitmapImage(new Uri(items.Path));
                    //                backgroundImage.Add(bi);
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (var items in filesInFolder)
                    //    {
                    //        BackgroundImages bi = new BackgroundImages();
                    //        bi.Filename = items.DisplayName;
                    //        bi.Bitmapimage = new BitmapImage(new Uri(items.Path));
                    //        backgroundImage.Add(bi);


                    //    }
                    //}
                }
                catch (Exception e)
                {
                    await Logging(e.ToString());
                }
            }

        }

    

        public static async Task SaveImageOrder()
        {
            var imageorder = JsonConvert.SerializeObject(GlobalVariables.backgroundImage,Formatting.Indented);
                
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, imageorder);

           

        }
    }
    
    }

