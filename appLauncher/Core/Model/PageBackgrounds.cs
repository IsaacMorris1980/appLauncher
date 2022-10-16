using Newtonsoft.Json;

using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Model
{
    public class PageBackgrounds : ModelBase
    {
        public string pageBackgroundImageFullPathLocation { get; set; }
        public string pageBackgroundImageDisplayName { get; set; }
        [JsonIgnore]
        public string pageBackgroundImageBytes { get; set; }
        public BitmapImage pageBackgroundDisplayImage { get; set; }



    }
}
