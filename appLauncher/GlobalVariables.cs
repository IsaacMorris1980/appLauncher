using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using appLauncher.Model;

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
	}
}
