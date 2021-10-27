using appLauncher.Core.Brushes;
using Windows.UI;

namespace appLauncher.Core.Model
{
    public class BackgroundImages
    {
       public string Filename { get; set; }
       public byte[] Backgroundimage { get;set; }
        public Color Backgroundimagecolor { get; set; } = Colors.Blue;
        public double BackgroundImageOpacity { get; set; } = 1;
        public MaskedBrush BackgroundBrush()
        {
            return new MaskedBrush(this.Backgroundimage,this.Backgroundimagecolor);
        }
    }
}
