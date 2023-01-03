using appLauncher.Core.Helpers;

using Newtonsoft.Json;

using System.Threading.Tasks;

using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Model
{
    public class PageBackgrounds : ModelBase
    {
        private string _imageFullPath;
        private string _backgroundDisplayName;
        private byte[] _backgroundImage;
        private BitmapImage _backgroundDisplayImage;

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_imageFullPath))
                {
                    return "";
                }
                return _imageFullPath;
            }
            set
            {
                SetProperty(ref _imageFullPath, value);
            }
        }
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
        [JsonIgnore]
        public byte[] BackgroundImageBytes
        {
            get
            {
                if (_backgroundImage.Length <= 0)
                {
                    return null;
                }
                return _backgroundImage;
            }
            set
            {
                SetProperty(ref _backgroundImage, value);
            }
        }
        public async Task<BitmapImage> PageBackround()
        {

            BitmapImage randomaccess = await ImageHelper.ConvertfromByteArraytoBitmapImage(BackgroundImageBytes);

            return randomaccess;
        }
        [JsonIgnore]
        public BitmapImage pageBackgroundDisplayImage
        {
            get
            {
                return _backgroundDisplayImage;
            }
            set
            {
                SetProperty(ref _backgroundDisplayImage, value);
            }
        }



    }
}
