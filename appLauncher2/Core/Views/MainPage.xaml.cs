using appLauncher2.Core.ViewModels;

using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace appLauncher2.Core.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel MainModel { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            MainModel = new MainPageViewModel();
        }
    }
}
