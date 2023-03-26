using Newtonsoft.Json;

namespace appLauncher.Core.Model
{
    public class PageBackgrounds : ModelBase
    {

        private string _backgroundDisplayName;
        private byte[] _backgroundImage;

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




    }
}
