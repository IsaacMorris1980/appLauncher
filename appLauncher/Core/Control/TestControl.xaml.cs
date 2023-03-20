using appLauncher.Core.CustomEvent;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace appLauncher.Core.Control
{
    public sealed partial class TestControl : UserControl
    {
        private int Maxicons;
        private const int iconsDisplayed = 3;
        private int lowerIndex;
        private int selectedIndex;
        public TestControl()
        {
            this.InitializeComponent();
        }
        public void SetupIndicators(PageNumChangedArgs e)
        {
            flipViewIndicator.Children.Clear();
            for (int i = 0; i < iconsDisplayed; i++)
            {
                Ellipse el = new Ellipse();
                el.Width = 8;
                el.Height = 8;
                el.Tag = i;
                el.Tapped += El_Tapped;
                flipViewIndicator.Children.Add(el);
            }
        }
        public void UpdatePageIndicators(PageChangedEventArgs e)
        {
            if ((lowerIndex + iconsDisplayed) <= Maxicons)
            {

            }

        }

        private void El_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
