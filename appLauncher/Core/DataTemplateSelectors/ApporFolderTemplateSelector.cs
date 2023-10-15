using appLauncher.Core.Model;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.DataTemplateSelectors
{
    public class TileorFolderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TileTemplate { get; set; }
        public DataTemplate FolderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is FinalTiles)
            {
                return TileTemplate;
            }
            if (item is AppFolder)
            {
                return FolderTemplate;
            }

            return base.SelectTemplateCore(item);
        }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
