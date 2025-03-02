using appLauncher.Core.PageModel;

using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private string appversion;
        public AboutPage()
        {
            this.InitializeComponent();
        }                
    }
}
