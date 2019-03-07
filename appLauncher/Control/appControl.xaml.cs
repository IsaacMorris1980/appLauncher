using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using appLauncher;
using Windows.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236


namespace appLauncher.Control
{
    public sealed partial class appControl : UserControl
    {
		int page;
		DispatcherTimer dispatcher;
        public static Color Failbackcolor = Colors.Blue;
        //Each copy of this control is binded to an app.
        //public finalAppItem appItem { get { return this.DataContext as finalAppItem; } }
        public appControl()
        {
            this.InitializeComponent();
			this.Loaded += AppControl_Loaded;
          //  this.DataContextChanged += (s, e) => Bindings.Update();
        }

		private void AppControl_Loaded(object sender, RoutedEventArgs e)
		{
			page = (int)this.DataContext;
			GlobalVariables.finalAppItems.CurrentPage = page;
			dispatcher = new DispatcherTimer();
			dispatcher.Interval = TimeSpan.FromSeconds(2);
			dispatcher.Tick += Dispatcher_Tick;
			if (page==0)
			{
				SwitchedToThisPage();
			}
			
		}

		private void Dispatcher_Tick(object sender, object e)
		{
			ProgressRing.IsActive = false;
			page = (int)this.DataContext;
			GlobalVariables.finalAppItems.CurrentPage = page;
			dispatcher.Stop();
			GridViewMain.ItemsSource = GlobalVariables.finalAppItems;
		}
		public void SwitchedToThisPage()
		{
			if (dispatcher != null)
			{
				ProgressRing.IsActive = true;
				dispatcher.Start();
			}
		}

		public void SwitchedFromThisPage()
		{
			GridViewMain.ItemsSource = null;
		}
	}
}
