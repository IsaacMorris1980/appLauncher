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
			GridViewMain.ItemsSource =(ObservableCollection<finalAppItem>) GlobalVariables.finalAppItems;
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

        private void GridViewMain_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pt = e.GetCurrentPoint(GridViewMain);
            GeneralTransform te = this.TransformToVisual(this.Parent as UIElement);
            Point test = te.TransformPoint(new Point(0, 0));
            double right = test.X + (this.ActualWidth - 100);
            double left = test.X + 100;
            if (GlobalVariables.isdragging)
            {
                //if (pt.Position.X > right)
                //{
                //    var ten = this.Parent as FlipView;
                //    if (ten.SelectedIndex < ten.Items.Count() - 1)
                //    {
                //        ten.SelectedIndex += 1;
                //    }

                //}
                //if (pt.Position.X < left)
                //{
                //    var ten = this.Parent as FlipView;
                //    if (ten.SelectedIndex > 0)
                //    {
                //        ten.SelectedIndex -= 1;
                //    }
                //}
            }
        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            GlobalVariables.isdragging = true;  
           
        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            GlobalVariables.isdragging = false;
            Point pt = e.GetPosition(this);
           
        }

        private async void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            var d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            FlipView c =(FlipView) ((Window.Current.Content as Frame).Content as MainPage).getFlipview();
            Point startpoint = e.GetPosition(this);
            if (GlobalVariables.startingpoint.X==0)
            {
                GlobalVariables.startingpoint = startpoint;

            }
            else
            {
               
                var a = this.TransformToVisual(c);
                var b = a.TransformPoint(new Point(0 , 0));
                if (GlobalVariables.startingpoint.X>startpoint.X && startpoint.X<(b.X+100))
                {
                    await ((Window.Current.Content as Frame).Content as MainPage).ChangeSelectedAsync("left");
                    GlobalVariables.startingpoint = startpoint;
                }
                else if(GlobalVariables.startingpoint.X<startpoint.X && startpoint.X>(b.X+d.ActualWidth-100))
                {
                    await ((Window.Current.Content as Frame).Content as MainPage).ChangeSelectedAsync("right");
                    GlobalVariables.startingpoint = startpoint;
                    
                }
            }
        }

        private void GridViewMain_DragEnter(object sender, DragEventArgs e)
        {

        }

        
    }
}
