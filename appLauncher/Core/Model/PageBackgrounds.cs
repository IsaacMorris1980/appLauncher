using appLauncher.Core.Brushes;

using System.Drawing;

namespace appLauncher.Core.Model
{
    public class PageBackgrounds : ModelBase
    {
        public string imagefullpathlocation { get; set; }
        public string imagedisplayname { get; set; }
        public string backgroundimageoverlaycolor { get; set; } = "Blue";
        public string backgroundimageopacity { get; set; } = "150";
        public MaskedBackgroundImage PageBackgroundBrush()
        {
            Color frontcolor = Color.FromArgb(int.Parse(this.backgroundimageopacity), Color.FromName(this.backgroundimageoverlaycolor));
            Windows.UI.Color uicolor = new Windows.UI.Color();
            uicolor.A = frontcolor.A;
            uicolor.R = frontcolor.R;
            uicolor.G = frontcolor.G;
            uicolor.B = frontcolor.B;

            MaskedBackgroundImage brush = new MaskedBackgroundImage(imagefullpathlocation, uicolor);
            return brush;
        }
    }
}
