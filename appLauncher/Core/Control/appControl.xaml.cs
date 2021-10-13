using applauncher.Core.Models;

using appLauncher.Core.Helpers;
using appLauncher.Core.Pages;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236


namespace appLauncher.Core.Control
{
    public sealed partial class appControl : UserControl
    {
        int page;
        DispatcherTimer dispatcher;

        public appControl()
        {
            this.InitializeComponent();
            this.Loaded += AppControl_Loaded;
        }

        private void AppControl_Loaded(object sender, RoutedEventArgs e)
        {
            GridViewMain.ItemsSource = packageHelper.Bags.Skip(packageHelper.pagenum * packageHelper.appsperscreen).Take(packageHelper.appsperscreen).ToList();
        }

        private void Dispatcher_Tick(object sender, object e)
        {
            ProgressRing.IsActive = false;
            dispatcher.Stop();
            GridViewMain.ItemsSource = packageHelper.Bags.Skip(packageHelper.pagenum * packageHelper.appsperscreen).Take(packageHelper.appsperscreen).ToList();
        }

        public void SwitchedToThisPage()
        {
            GridViewMain.ItemsSource = packageHelper.Bags.Skip(packageHelper.pagenum * packageHelper.appsperscreen).Take(packageHelper.appsperscreen).ToList();
        }

        public void SwitchedFromThisPage()
        {
            GridViewMain.ItemsSource = null;
        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            packageHelper.isdragging = true;
            object item = e.Items.First();
            var source = sender;
            e.Data.Properties.Add("item", item);
            packageHelper.itemdragged = (AppTile)item;
            packageHelper.oldindex = packageHelper.Bags.IndexOf((AppTile)item);
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
            var t = (List<AppTile>)view.ItemsSource;
            var te = t[(index * packageHelper.columns) + indexy];
            packageHelper.newindex = packageHelper.Bags.IndexOf(te);
            packageHelper.Bags.RemoveAt(packageHelper.oldindex);
            packageHelper.Bags.Insert(packageHelper.newindex, packageHelper.itemdragged);
            packageHelper.pagenum = (int)this.DataContext;
            SwitchedToThisPage();
            ((Window.Current.Content as Frame).Content as MainPage).UpdateIndicator(packageHelper.pagenum);
        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            FlipView c = (FlipView)((Window.Current.Content as Frame).Content as MainPage).getFlipview();
            Point startpoint = e.GetPosition(this);
            if (packageHelper.startingpoint.X == 0)
            {
                packageHelper.startingpoint = startpoint;
            }
            else
            {
                var a = this.TransformToVisual(c);
                var b = a.TransformPoint(new Point(0, 0));
                if (packageHelper.startingpoint.X > startpoint.X && startpoint.X < (b.X + 100))
                {
                    if (c.SelectedIndex > 0)
                    {
                        c.SelectedIndex -= 1;
                        packageHelper.startingpoint = startpoint;
                    }
                }
                else if (packageHelper.startingpoint.X < startpoint.X && startpoint.X > (b.X + d.ActualWidth - 100))
                {
                    if (c.SelectedIndex < c.Items.Count() - 1)
                    {
                        c.SelectedIndex += 1;
                        packageHelper.startingpoint = startpoint;
                    }
                }
            }
            packageHelper.pagenum = c.SelectedIndex;
            ((Window.Current.Content as Frame).Content as MainPage).UpdateIndicator(packageHelper.pagenum);
        }

        private async void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            AppTile fi = (AppTile)e.ClickedItem;
            await fi.LaunchAsync();
        }
    }
}
