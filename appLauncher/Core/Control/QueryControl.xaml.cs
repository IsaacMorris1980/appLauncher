using appLauncher.Core.Model;

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace appLauncher.Core.Control
{
    public sealed partial class QueryControl : UserControl
    {
        public AppTile AppQueryResult { get { return DataContext as AppTile; } }
        public QueryControl()
        {
            this.InitializeComponent();

        }
    }
}
