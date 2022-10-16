using appLauncher.Helpers;
using appLauncher.Model;


using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


using Windows.Foundation;
using Windows.Storage;

using Windows.UI.Xaml.Media.Imaging;

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
        public static bool bgimagesavailable { get; set; }
        public static event EventHandler AppsRetreived;

        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public static List<BackgroundImages> backgroundImage { get; set; } = new List<BackgroundImages>();
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

            await packageHelper.getAllAppsAsync();

            if (await IsFilePresent("collection.txt"))
            {
                List<finalAppItem> orderedapplist = new List<finalAppItem>();
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                orderedapplist = JsonConvert.DeserializeObject<List<finalAppItem>>(apps);
                for (int i = 0; i < orderedapplist.Count - 1; i++)
                {
                    var index = AllApps.listOfApps.IndexOf(AllApps.listOfApps.First(x => x.appName == orderedapplist[i].appName));
                    var app = AllApps.listOfApps[index];
                    app.backColor = orderedapplist[i].backColor;
                    app.textColor = orderedapplist[i].textColor;
                    AllApps.listOfApps.Move(index, i);
                }
                if (AppsRetreived != null)
                {
                    AppsRetreived(true, EventArgs.Empty);
                }
            }
            else
            {
                if (AppsRetreived != null)
                {
                    AppsRetreived(true, EventArgs.Empty);
                }
            }





        }
        public static async Task SaveCollectionAsync()
        {

            List<finalAppItem> finals = AllApps.listOfApps.ToList();

            string te = JsonConvert.SerializeObject(finals, Formatting.Indented);
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
                    var images = await FileIO.ReadLinesAsync(item);
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                    var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    if (images.Count() > 0)
                    {
                        foreach (string y in images)
                        {
                            foreach (var items in filesInFolder)
                            {
                                if (items.DisplayName == y)
                                {
                                    BackgroundImages bi = new BackgroundImages();
                                    bi.Filename = items.DisplayName;
                                    bi.Bitmapimage = new BitmapImage(new Uri(items.Path));
                                    backgroundImage.Add(bi);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var items in filesInFolder)
                        {
                            BackgroundImages bi = new BackgroundImages();
                            bi.Filename = items.DisplayName;
                            bi.Bitmapimage = new BitmapImage(new Uri(items.Path));
                            backgroundImage.Add(bi);


                        }
                    }
                }
                catch (Exception e)
                {
                    await Logging(e.ToString());
                }
            }
            else
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);
                var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                foreach (var items in filesInFolder)
                {
                    BackgroundImages bi = new BackgroundImages();
                    bi.Filename = items.DisplayName;
                    bi.Bitmapimage = new BitmapImage(new Uri(items.Path));
                    backgroundImage.Add(bi);

                }
            }


        }

        public static async Task SaveImageOrder()
        {
            List<string> imageorder = new List<string>();
            imageorder = (from x in backgroundImage select x.Filename).ToList();
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteLinesAsync(item, imageorder);



        }
        public static async Task LoadAppColor()
        {
            if (await IsFilePresent("Colors.xml"))
            {
                string item = ApplicationData.Current.LocalFolder.Path + "\\Colors.xml";
                XElement xe = XElement.Load(item);
                var a = xe.Attribute("ForegroundColor");
                var b = a.Value.Split(",");
                AppTileColor.foregroundColor = ColorHelper.FromArgb(Convert.ToByte(b[0]), Convert.ToByte(b[1]), Convert.ToByte(b[2]), Convert.ToByte(b[3]));
                a = xe.Attribute("BackgroundColor");
                b = a.Value.Split(",");
                AppTileColor.backgroundColor = ColorHelper.FromArgb(Convert.ToByte(b[0]), Convert.ToByte(b[1]), Convert.ToByte(b[2]), Convert.ToByte(b[3]));
                AppTileColor.foregroundOpacity = Convert.ToDouble((xe.Attribute("ForegroundOpacity")).Value);
                AppTileColor.backgroundOpacity = Convert.ToDouble((xe.Attribute("BackgroundOpacity")).Value);
            }
        }
        public static void SaveAppColors()
        {
            XElement xe = new XElement("AppTileColor");
            xe.SetAttributeValue("ForegroundColor", $"{AppTileColor.foregroundColor.A},{AppTileColor.foregroundColor.R},{AppTileColor.foregroundColor.G},{AppTileColor.foregroundColor.B}");
            xe.SetAttributeValue("BackgroundColor", $"{AppTileColor.backgroundColor.A},{AppTileColor.backgroundColor.R},{AppTileColor.backgroundColor.G},{AppTileColor.backgroundColor.B}");
            xe.SetAttributeValue("BackgroundOpacity", AppTileColor.backgroundOpacity);
            xe.SetAttributeValue("ForegroundOpacity", AppTileColor.foregroundOpacity);
            string item = ApplicationData.Current.LocalFolder.Path + "\\Colors.xml";
            xe.Save(item, SaveOptions.None);
        }
    }

}

