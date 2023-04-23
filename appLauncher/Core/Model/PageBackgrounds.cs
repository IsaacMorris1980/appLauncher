using Newtonsoft.Json;

using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Model
{
    public class PageBackgrounds : ModelBase
    {

        private string _backgroundDisplayName;
        private byte[] _backgroundImage;
        private string _imagepath;
        private BitmapImage _image;

        public string BackgroundImageDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_backgroundDisplayName))
                {
                    return "";
                }
                return _backgroundDisplayName;
            }
            set
            {
                SetProperty(ref _backgroundDisplayName, value);
            }
        }
        public string filepath
        {
            get
            {
                return _imagepath;
            }
            set
            {
                SetProperty(ref _imagepath, value);
            }
        }
        [JsonIgnore]
        public BitmapImage GetImage
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }
        public byte[] BackgroundImageBytes
        {
            get
            {
                byte[] bytes;
                if (_backgroundImage == null)
                {
                    bytes = new byte[1];
                }
                return _backgroundImage;
            }
            set
            {
                SetProperty(ref _backgroundImage, value);
            }
        }




    }
}
