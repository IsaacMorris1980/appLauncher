using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Interfaces
{
    public interface IApporFolder
    {
        string Name { get; set; }
        string Description { get; set; }
        bool Favorite { get; set; }
        int ListPos { get; set; }
        int LaunchedCount { get; set; }
        Color TextColor { get; set; }
        Color BackColor { get; set; }
        SolidColorBrush TextBrush { get; }
        SolidColorBrush BackBrush { get; }

    }
}
