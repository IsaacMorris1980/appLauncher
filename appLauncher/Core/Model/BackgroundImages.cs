using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Model
{
    public class BackgroundImages
    {

        private string filename;
        private BitmapImage bitmapimage;
        public string Filename { get => filename; set => filename = value; }
        public BitmapImage Bitmapimage { get => bitmapimage; set => bitmapimage = value; }
    }
}
