using UwpAppLauncher.Model;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpAppLauncher.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class FolderView : Page
    {
        public FolderTile folder;
        private App app = (App)Application.Current;
        public FolderView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            folder = (FolderTile)e.Parameter;
        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {

        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {

        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {

        }

        private void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}