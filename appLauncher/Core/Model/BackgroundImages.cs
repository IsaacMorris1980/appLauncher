using applauncher.mobile.Core.Brushes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.mobile.Core.Models
{
   public class BackgroundImages
    {
       public string Filename { get; set; }
       public byte[] Backgroundimage { get;set; }
        public Color Backgroundimagecolor { get; set; } = Colors.Transparent;
        public double BackgroundImageOpacity { get; set; } = 1;
        public MaskedBrush BackgroundBrush()
        {
            return new MaskedBrush()
            {
                logo = Backgroundimage.AsBuffer().AsStream().AsRandomAccessStream(),
                overlaycolor = Backgroundimagecolor,
                Opacity = BackgroundImageOpacity
            };
        }
    }
}
