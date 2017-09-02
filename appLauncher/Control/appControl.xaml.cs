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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace appLauncher.Control
{
    public sealed partial class appControl : UserControl
    {
        public AppListEntry appItem { get { return this.DataContext as AppListEntry; } }
        public appControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (appItem != null)
            {
                var logoStream = appItem.DisplayInfo.GetLogo(new Size(50,50));
                BitmapImage imageFromStream = new BitmapImage();
                IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                imageFromStream.SetSource(whatIWant);
                this.appIcon.Source = imageFromStream;
            }
        }
    }
}
