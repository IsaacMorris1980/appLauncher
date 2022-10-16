using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Model
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
