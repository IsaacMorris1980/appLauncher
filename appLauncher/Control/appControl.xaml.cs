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
using System.Collections.ObjectModel;
using Windows.UI.Input;
using Windows.UI.Core;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236


namespace appLauncher.Control
{
    public sealed partial class appControl : UserControl
    {
		int page;
		DispatcherTimer dispatcher;




        public ObservableCollection<finalAppItem> listOfApps
        {
            get { return (ObservableCollection<finalAppItem>)GetValue(listOfAppsProperty); }
            set { SetValue(listOfAppsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for listOfApps.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty listOfAppsProperty =
            DependencyProperty.Register("listOfApps", typeof(ObservableCollection<finalAppItem>), typeof(appControl), null);


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

            //dispatcher = new DispatcherTimer();
            //dispatcher.Interval = TimeSpan.FromSeconds(2);
            //dispatcher.Tick += Dispatcher_Tick;
            //if (GlobalVariables.pagenum==0)
            //{
            //	SwitchedToThisPage();
            //}

            GridViewMain.ItemsSource = AllApps.listOfApps.Skip(GlobalVariables.pagenum * GlobalVariables.appsperscreen).Take(GlobalVariables.appsperscreen).ToList();

        }

		private void Dispatcher_Tick(object sender, object e)
		{
			ProgressRing.IsActive = false;
			dispatcher.Stop();
            GridViewMain.ItemsSource = AllApps.listOfApps.Skip(GlobalVariables.pagenum * GlobalVariables.appsperscreen).Take(GlobalVariables.appsperscreen).ToList();
		}
		public void SwitchedToThisPage()
		{
            //if (dispatcher != null)
            //{
            //    ProgressRing.IsActive = true;
            //    dispatcher.Start();
            //}
            GridViewMain.ItemsSource = AllApps.listOfApps.Skip(GlobalVariables.pagenum * GlobalVariables.appsperscreen).Take(GlobalVariables.appsperscreen).ToList();
        }

		public void SwitchedFromThisPage()
		{
			GridViewMain.ItemsSource = null;
		}

         private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
             GlobalVariables.isdragging = true;
            object item = e.Items.First();
            var source = sender;
            e.Data.Properties.Add("item", item);
            GlobalVariables.itemdragged = (finalAppItem)item;
            GlobalVariables.oldindex = AllApps.listOfApps.IndexOf((finalAppItem)item);
        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            GridView view = sender as GridView;
           
            // Get your data

         //   var item = e.Data.Properties.Where(p => p.Key == "item").Single();

            //Find the position where item will be dropped in the gridview
            Point pos = e.GetPosition(view.ItemsPanelRoot);

            //Get the size of one of the list items
            GridViewItem gvi = (GridViewItem)view.ContainerFromIndex(0);
            double itemHeight = gvi.ActualHeight + gvi.Margin.Top + gvi.Margin.Bottom;
            double itemwidth = gvi.ActualHeight + gvi.Margin.Left + gvi.Margin.Right;

            //Determine the index of the item from the item position (assumed all items are the same size)
            int index = Math.Min(view.Items.Count - 1, (int)(pos.Y / itemHeight));
            int indexy = Math.Min(view.Items.Count - 1, (int)(pos.X / itemwidth));
            var t = (List<finalAppItem>)view.ItemsSource;
            var te = t[((index * GlobalVariables.columns) + (indexy))];
            GlobalVariables.newindex = AllApps.listOfApps.IndexOf(te);
            AllApps.listOfApps.Move(GlobalVariables.oldindex,GlobalVariables.newindex);
          GlobalVariables.pagenum = (int)this.DataContext;
            SwitchedToThisPage();
          ((Window.Current.Content as Frame).Content as MainPage).UpdateIndicator(GlobalVariables.pagenum);

        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            
            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            FlipView c = (FlipView)((Window.Current.Content as Frame).Content as MainPage).getFlipview();
            Point startpoint = e.GetPosition(this);
            if (GlobalVariables.startingpoint.X == 0)
            {
                GlobalVariables.startingpoint = startpoint;

            }
            else
            {

                var a = this.TransformToVisual(c);
                var b = a.TransformPoint(new Point(0, 0));
                if (GlobalVariables.startingpoint.X > startpoint.X && startpoint.X < (b.X + 100))
                {
                    if (c.SelectedIndex > 0)
                    {
                        c.SelectedIndex -= 1;
                        GlobalVariables.startingpoint = startpoint;
                    }

                }
                else if (GlobalVariables.startingpoint.X < startpoint.X && startpoint.X > (b.X + d.ActualWidth - 100))
                {
                    if (c.SelectedIndex < c.Items.Count() - 1)
                    {
                        c.SelectedIndex += 1;
                        GlobalVariables.startingpoint = startpoint;
                    }

                }
            }
            GlobalVariables.pagenum = c.SelectedIndex;
            ((Window.Current.Content as Frame).Content as MainPage).UpdateIndicator(GlobalVariables.pagenum);
        }

        private async void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            finalAppItem fi = (finalAppItem)e.ClickedItem;
            await fi.appEntry.LaunchAsync();
        }
    }
}
