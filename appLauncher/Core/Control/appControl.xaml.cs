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
        public appControl()
        {
            this.InitializeComponent();
            this.Loaded += AppControl_Loaded;
        }

        private void AppControl_Loaded(object sender, RoutedEventArgs e)
        {
            GridViewMain.ItemsSource = packageHelper.Apps;
        }

        public void SwitchedToThisPage()
        {
            GridViewMain.ItemsSource = packageHelper.Apps;
        }
        public void SwitchedFromThisPage()
        {
            GridViewMain.ItemsSource = null;
        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            GlobalVariables.isdragging = true;
            GlobalVariables.itemdragged.initialindex = packageHelper.Apps.IndexOf((Apps)e.Items[0]);
        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            GridView view = sender as GridView;


            //Find the position where item will be dropped in the gridview
            Point pos = e.GetPosition(view.ItemsPanelRoot);

            //Get the size of one of the list items
            GridViewItem gvi = (GridViewItem)view.ContainerFromIndex(0);
            double itemHeight = gvi.ActualHeight + gvi.Margin.Top + gvi.Margin.Bottom;
            double itemwidth = gvi.ActualHeight + gvi.Margin.Left + gvi.Margin.Right;

            //Determine the index of the item from the item position (assumed all items are the same size)
            int index = Math.Min(view.Items.Count - 1, (int)(pos.Y / itemHeight));
            int indexy = Math.Min(view.Items.Count - 1, (int)(pos.X / itemwidth));
            AppPaginationObservableCollection t = (AppPaginationObservableCollection)view.ItemsSource;
            int listindex = ((index * GlobalVariables.columns) + (indexy));

            int moveto = 0;
            if (listindex >= t.Count())
            {
                moveto = (GlobalVariables.pagenum * GlobalVariables.appsperscreen) + listindex;
                if (moveto >= packageHelper.Apps.Count())
                {
                    moveto = (packageHelper.Apps.Count() - 1);
                }
            }
            if (listindex <= t.Count() - 1)
            {
                moveto = (GlobalVariables.pagenum * GlobalVariables.appsperscreen) + listindex;
            }
            packageHelper.Apps.Move(GlobalVariables.itemdragged.initialindex, moveto);
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
            Apps fi = (Apps)e.ClickedItem;
            await packageHelper.LaunchApp(fi.FullName);
        }
    }
}

