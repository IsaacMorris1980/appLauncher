using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using appLauncher.Model;
using System.Collections.ObjectModel;
using appLauncher.Helpers;

namespace appLauncher
{
  public static class GlobalVariables
    {
		public static int appsperscreen { get; set; }
		public static int pagestomake { get; set; }
		public static PaginationObservableCollection<finalAppItem> finalAppItems { get; set; }

		public static async Task Logging(string texttolog)
		{
			StorageFolder stors = ApplicationData.Current.LocalFolder;
			await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), texttolog);
			await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), Environment.NewLine);
		}
        public static int NumofRoworColumn(int padding,int objectsize,int sizetofit)
        {
            int amount = 0;
            int intsize = objectsize +  (padding + padding);
            int size = intsize;
            while (size+intsize<sizetofit)
            {
                amount += 1;
                size += intsize;
            }
            return amount;

        }
        public static async Task LoadCollectionAsync()
        {
       
            await packageHelper.getAllAppsAsync();
            ObservableCollection<finalAppItem> oc1 = AllApps.listOfApps;
           
                ObservableCollection<finalAppItem> oc = new ObservableCollection<finalAppItem>();
                StorageFile item = (StorageFile) await  ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                    var te = await FileIO.ReadLinesAsync(item);
                foreach (string y  in te)
                {
                    foreach (finalAppItem items in oc1)
                    {
                        if (items.appEntry.DisplayInfo.DisplayName==y)
                        {
                            oc.Add(items);
                        }
                    }
                }
                AllApps.listOfApps = oc;
                

           
            
        }
        public static async Task SaveCollectionAsync()
        {
            
                var te = from x in AllApps.listOfApps select x.appEntry.DisplayInfo.DisplayName;
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt",CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(item, te);
        }
        public static async Task<bool> isFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }
    }
}
