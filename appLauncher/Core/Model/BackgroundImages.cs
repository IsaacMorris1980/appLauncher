using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Model
{
   public class BackgroundImages
    {
       private StorageItemThumbnail thumbnail;
       private string filename;
       private BitmapImage bitmapimage;
       public string Filename { get => filename; set => filename = value; }
       public BitmapImage Bitmapimage { get => bitmapimage; set => bitmapimage = value; }
    }
}
