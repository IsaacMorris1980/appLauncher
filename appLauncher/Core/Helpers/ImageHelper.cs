using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class ImageHelper
    {
        public static ObservableCollection<PageBackgrounds> backgroundImage { get; set; } = new ObservableCollection<PageBackgrounds>();

        public static async Task LoadBackgroundImages()
        {
            List<PageBackgrounds> imageslist = new List<PageBackgrounds>();

            if (await IsFilePresent("images.txt"))
            {
                try
                {
                    string folderpath = ApplicationData.Current.LocalFolder.Path;

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                    string imagesstring = await FileIO.ReadTextAsync(item);
                    ObservableCollection<PageBackgrounds> images = JsonConvert.DeserializeObject<ObservableCollection<PageBackgrounds>>(imagesstring);
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    var backgroundImageFolder = await localFolder.CreateFolderAsync("backgroundImage", CreationCollisionOption.OpenIfExists);

                    var filesInFolder = await backgroundImageFolder.GetFilesAsync();
                    var test = (from x in filesInFolder select x.Path).ToList();
                    IEnumerable<string> extrafiles = test.Where(x => !images.Any(y => y.imagefullpathlocation == x));
                    IEnumerable<PageBackgrounds> selected = images.Where(x => !test.Any(y => y == x.imagefullpathlocation)).ToList();
                    imageslist.AddRange(selected);
                    if (extrafiles.Count() > 0)
                    {
                        foreach (string y in extrafiles)
                        {
                            var a = Path.GetFileName(y);
                            var ab = await backgroundImageFolder.GetFileAsync(a);
                            imageslist.Add(new PageBackgrounds
                            {
                                imagefullpathlocation = ab.Path,
                                imagedisplayname = ab.DisplayName,
                                backgroundimageopacity = "255",
                                backgroundimageoverlaycolor = "Transparent"
                            });
                        }
                    }
                    else
                    {
                        imageslist.AddRange(selected);
                    }
                }
                catch (Exception e)
                {
                    await GlobalVariables.Logging(e.ToString());
                }
                backgroundImage = new ObservableCollection<PageBackgrounds>(imageslist);
            }
        }

        public static async Task SaveImageOrder()
        {
            string imageorder = JsonConvert.SerializeObject(backgroundImage, Formatting.Indented);
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(item, imageorder);



        }

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
    }
}
