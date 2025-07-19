using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;
// Removed: using Microsoft.Extensions.DependencyInjection; // No longer needed
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using appLauncher.Core.Converters;
using appLauncher.Core.Services; // Import your new ServiceLocator

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the FolderDetail page, handling folder information display, editing, and app management.
    /// Delegates logic to the <see cref="FolderDetailViewModel"/>.
    /// </summary>
    public sealed partial class FolderDetailPage : Page
    {
        public FolderDetailViewModel ViewModel { get; set; }

        public FolderDetailPage()
        {
            // Resolve the ViewModel using your manual ServiceLocator
            ViewModel = App.ServiceLocator.Resolve<FolderDetailViewModel>();
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

            // The navigation parameter can be a complex object or a simple one.
            // We'll expect an object that contains both the folder and an optional flag for edit mode.
            if (e.Parameter is Tuple<AppFolder, bool> navigationTuple)
            {
                ViewModel.SetFolder(navigationTuple.Item1); // Set the folder
                ViewModel.IsEditMode = navigationTuple.Item2; // Set the edit mode based on the flag
            }
            else if (e.Parameter is AppFolder folder)
            {
                // If only the folder is passed, default to view mode
                ViewModel.SetFolder(folder);
                ViewModel.IsEditMode = false;
            }
        }

        /// <summary>
        /// Handles text changes in the search AutoSuggestBox and triggers ViewModel search.
        /// </summary>
        private void SearchField_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.SearchAppsCommand.Execute(sender.Text);
            }
        }

        /// <summary>
        /// Handles selection of a suggestion from the search AutoSuggestBox and launches the app.
        /// </summary>
        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is FinalTiles app)
            {
                ViewModel.LaunchAppCommand.Execute(app);
            }
            sender.Text = string.Empty; // Clear search box after selection
            sender.ItemsSource = ViewModel.SearchResults; // Reset suggestions
        }

        /// <summary>
        /// Handles item click events on the ListBox of apps in the folder, launching the app.
        /// </summary>
        private void AppsinFolderView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is FinalTiles app)
            {
                ViewModel.LaunchAppCommand.Execute(app);
            }
        }

        /// <summary>
        /// Handles right-tap (context menu) on an app item in the list.
        /// </summary>
        private void AppItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Show the context flyout directly from the UI element
            if (sender is FrameworkElement element && element.ContextFlyout != null)
            {
                element.ContextFlyout.ShowAt(element);
            }
        }

        /// <summary>
        /// Handles tap on the "Info" context menu item for an app.
        /// </summary>
        private void Info_Tapped(object sender, TappedRoutedEventArgs e)
        {
            object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item is FinalTiles app)
            {
                // Navigate to AppDetailPage for this app, defaulting to info mode
                Frame.Navigate(typeof(AppDetailPage), app);
            }
        }

        /// <summary>
        /// Handles tap on the "Edit Tile" context menu item for an app.
        /// </summary>
        private void EditAppTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item is FinalTiles app)
            {
                // Navigate to AppDetailPage for this app, explicitly setting it to edit mode
                // Pass a Tuple<FinalTiles, bool> where bool indicates edit mode
                Frame.Navigate(typeof(AppDetailPage), Tuple.Create(app, true));
            }
        }

       
    }
}
