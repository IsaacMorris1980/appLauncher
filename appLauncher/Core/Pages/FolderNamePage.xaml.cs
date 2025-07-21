using appLauncher.Core.Services;
using appLauncher.Core.ViewModels;

using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    public sealed partial class FolderNamePage : ContentDialog
    {
        public FolderNameViewModel viewModel { get; set; }
        public FolderNamePage()
        {
            this.InitializeComponent();
            viewModel = App.ServiceLocator.Resolve<FolderNameViewModel>();
        }
        public string FolderName { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            FolderName = SetFolderName.Text;
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
