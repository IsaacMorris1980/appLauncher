
﻿using appLauncher.Brushes;
using appLauncher.Helpers;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;

using Windows.UI;

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Model
{

	/// <summary>
	/// A class made up of the app list entry and the app logo of each app. This is what each app control displayed represents.
	/// </summary>
	public class finalAppItem
	{
		[JsonIgnore]
		public Package appPackage { get; set; }
		[JsonIgnore]
		public AppListEntry appEntry { get; set; }
		[JsonIgnore]
		public BitmapImage appLogo { get; set; }
		public string appName { get; set; }
		public Color textColor { get; set; } = Colors.Red;
		public Color backColor { get; set; } = Colors.Black;
		[JsonIgnore]
		public SolidColorBrush textbrush
		{
			get
			{
				return new SolidColorBrush(textColor);
			}
		}
		[JsonIgnore]
		public SolidColorBrush backbrush
		{
			get
			{
				return new SolidColorBrush(backColor);
			}

		}
	


	}


    }



    public static class AllApps
    {
        public static ObservableCollection<finalAppItem> listOfApps { get; set; }
        public static List<KeyValuePair<AppListEntry, Package>> Allpackages { get; set; }

		/// <summary>
		/// Gets installed apps from device and stores them in an ObservableCollection of finalAppItem, which can be accessed from anywhere.
		/// </summary>
		/// <returns></returns>
		public static async Task getApps()
		{
			bool isLoaded = false;
			await packageHelper.getAllAppsAsync();
			isLoaded = true;

		}


		//public static async Task getAppsForSplash()
		//{

		//    listOfApps = await packageHelper.getAllAppsAsync();

		//}
	}

}
