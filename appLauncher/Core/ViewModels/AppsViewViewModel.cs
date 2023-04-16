using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace appLauncher.Core.ViewModels
{
    public class AppsViewViewModel
    {
        private int maxRows;
        private int maxColumns;
        bool firstrun { get; set; } = true;
        public int Maxicons { get; private set; }
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // milliseconds;
        private Button oldAnimatedButton;
        private Button buttontoanimate;
        private Ellipse ellipseToAnimate;
        private int previousSelectedIndex = 0;

        public void dragEvent(object sender, DragEventArgs e)
        {

        }
        public void dragDrop(object sender, DragEventArgs e)
        {

        }
        public void dragStarting(object sender, DragStartingEventArgs e)
        {

        }
        public void mouseWheel(object sender, PointerRoutedEventArgs e)
        {

        }
        public void Btn_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }


    }
}
