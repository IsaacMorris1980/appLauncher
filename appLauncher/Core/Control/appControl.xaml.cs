using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using System;
using System.Linq;

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236


namespace appLauncher.Core.Control
{
    public sealed partial class appControl : UserControl
    {
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

            GridViewMain.ItemsSource = packageHelper.appTiles;

        }

        //private void Dispatcher_Tick(object sender, object e)
        //{
        //    ProgressRing.IsActive = false;
        //    dispatcher.Stop();
        //    GridViewMain.ItemsSource = packageHelper.appTiles;
        //}
        public void SwitchedToThisPage()
        {
            //if (dispatcher != null)
            //{
            //    ProgressRing.IsActive = true;
            //    dispatcher.Start();
            //}
            GridViewMain.ItemsSource = packageHelper.appTiles;
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
            GlobalVariables.itemdragged = (AppTile)item;
            GlobalVariables.oldindex = packageHelper.appTiles.GetIndexof((AppTile)item);
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
            var t = (PaginationObservableCollection)view.ItemsSource;
            int listindex = ((index * GlobalVariables.columns) + (indexy));
            int appnewindex = 0;

            if (listindex >= t.Count())
            {
                //t.InsertItem(t.Count(), GlobalVariables.itemdragged);
                appnewindex = packageHelper.appTiles.IndexOf(GlobalVariables.itemdragged);
            }
            if (listindex <= t.Count() - 1)
            {
                AppTile te;
                te = t[((index * GlobalVariables.columns) + (indexy))];
                appnewindex = packageHelper.appTiles.IndexOf(te);
            }


            packageHelper.appTiles.Moved(GlobalVariables.oldindex, appnewindex, GlobalVariables.itemdragged);
            //   AllApps.listOfApps.Move(GlobalVariables.oldindex,GlobalVariables.newindex);
            GlobalVariables.SetPageNumber((int)this.DataContext);
            SwitchedToThisPage();

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
            GlobalVariables.SetPageNumber(c.SelectedIndex);

        }

        private async void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            AppTile fi = (AppTile)e.ClickedItem;
            await fi.LaunchAsync();
        }
    }
}
