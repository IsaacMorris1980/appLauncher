using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class ColorComboItem
    {
        public ColorComboItem() { }
        private string colorname;
        private SolidColorBrush colorbrush;
        public string ColorName
        {
            get
            {
                return colorname;
            }
            set
            {
                colorname = value;
            }
        }
        public SolidColorBrush ColorBrush
        {
            get
            {
                return colorbrush;
            }
            set
            {
                colorbrush = value;
            }
        }
    }
}
