using UwpAppLauncher.Interfaces;
using UwpAppLauncher.Model;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpAppLauncher.Templates
{
    public class appLauncherDataTemplates : DataTemplateSelector
    {
        public DataTemplate AppsDatatTemplate { get; set; }
        public DataTemplate FoldersDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            IApporFolder mb = (IApporFolder)item;
            if (mb is FolderTile)
            {
                return FoldersDataTemplate;
            }
            return AppsDatatTemplate;
        }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
