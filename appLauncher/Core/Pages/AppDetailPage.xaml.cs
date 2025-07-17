using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;
using appLauncher.Core.Converters;
using System;

using Windows.UI.Xaml; // For BooleanToVisibilityConverter and IValueConverter
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data; // For IValueConverter
using Windows.UI.Xaml.Navigation;

namespace appLauncher.Core.Pages
{
    
    /// <summary>
    /// Code-behind for the AppDetail page, handling both app information display and editing.
    /// Delegates logic to the <see cref="AppDetailViewModel"/>.
    /// </summary>
    public sealed partial class AppDetailPage : Page
    {
        public AppDetailViewModel ViewModel { get; set; }

        public AppDetailPage()
        {
            // Resolve the ViewModel using Dependency Injection
            ViewModel = App.ServiceLocator.Resolve<AppDetailViewModel>();
            this.DataContext = ViewModel;
            this.InitializeComponent();

            // Add converters to page resources programmatically if not defined in App.xaml
            // This ensures they are available for x:Bind
            if (!this.Resources.ContainsKey("BooleanToVisibilityConverter"))
            {
                this.Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
            }
            if (!this.Resources.ContainsKey("InverseBooleanToVisibilityConverter"))
            {
                this.Resources.Add("InverseBooleanToVisibilityConverter", new BooleanToVisibilityConverter { IsInverse = true });
            }
            if (!this.Resources.ContainsKey("EditModeToIconConverter"))
            {
                this.Resources.Add("EditModeToIconConverter", new EditModeToIconConverter());
            }
            if (!this.Resources.ContainsKey("EditModeToLabelConverter"))
            {
                this.Resources.Add("EditModeToLabelConverter", new EditModeToLabelConverter());
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is FinalTiles appTile)
            {
                ViewModel.SetApp(appTile); // Pass the app tile to the ViewModel
            }
        }
    }
}
