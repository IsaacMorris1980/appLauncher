using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class ColorComboItem
    {
        public ColorComboItem() { }
        private string _colorname;
        private SolidColorBrush _colorbrush;
        public string ColorName
        {
            get
            {
                return _colorname;
            }
            set
            {
                _colorname = value;
            }
        }
        public SolidColorBrush ColorBrush
        {
            get
            {
                return _colorbrush;
            }
            set
            {
                _colorbrush = value;
            }
        }
    }
}
