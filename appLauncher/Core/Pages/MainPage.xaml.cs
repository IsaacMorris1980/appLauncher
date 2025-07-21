// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.Converters;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;   // Import the ViewModel
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// The page where the apps are displayed. This code-behind primarily handles UI interactions
    /// and delegates logic to the <see cref="MainViewModel"/>.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; set; } // Public property for DataContext

        // UI-related state that might still live in code-behind for direct UI interaction
        private DispatcherTimer _sizeChangeDebounceTimer = new DispatcherTimer();
        private DraggedItem _itemDragged = new DraggedItem(); // Consider moving DraggedItem to ViewModel if it holds more state
        private bool _isDragging = false; // UI state for drag operation
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            // Resolve ViewModel using the manual ServiceLocator
            // This assumes App.ServiceLocator is configured in App.xaml.cs
            ViewModel = App.ServiceLocator.Resolve<MainViewModel>();
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
            // Setup UI event handlers
            this.SizeChanged += MainPage_SizeChanged;

            // Configure debounce timer for size changes
            _sizeChangeDebounceTimer.Tick += SizeChangeDebounceTimer_Tick;
            _sizeChangeDebounceTimer.Interval = TimeSpan.FromMilliseconds(300); // Debounce interval

            // Initial setup for GridView dimensions (before first layout calculation)
            // These will be updated by SizeChangeDebounceTimer_Tick when the page renders
            GridViewMain.Width = this.ActualWidth;
            GridViewMain.Height = this.ActualHeight;

            // Call ViewModel's load command to start loading data
            ViewModel.LoadDataCommand.Execute(null);

            // No direct ThreadPoolTimer for image rotation here; ImageService manages it,
            // and ViewModel.CurrentBackgroundBrush will update the UI via binding.
        }

        /// <summary>
        /// Handles the SizeChanged event of the page to debounce layout recalculations.
        /// </summary>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _sizeChangeDebounceTimer.Stop();
            _sizeChangeDebounceTimer.Start();
        }

        /// <summary>
        /// Tick handler for the size change debounce timer. Recalculates layout via ViewModel.
        /// </summary>
        private void SizeChangeDebounceTimer_Tick(object sender, object e)
        {
            _sizeChangeDebounceTimer.Stop();
            // Ensure GridView dimensions are updated before recalculating layout in ViewModel
            GridViewMain.Width = this.ActualWidth;
            GridViewMain.Height = this.ActualHeight;
            ViewModel.RecalculateLayout(GridViewMain.ActualWidth, GridViewMain.ActualHeight);
        }

        /// <summary>
        /// Disables the scroll viewer for the GridView to prevent manual scrolling.
        /// </summary>
        private void disableScrollViewer(GridView gridView)
        {
            try
            {
                var border = (Border)VisualTreeHelper.GetChild(gridView, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.IsVerticalRailEnabled = false;
                scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            catch (Exception es)
            {
                // Log this error, perhaps using the ViewModel's logging service if accessible
                Debug.WriteLine($"Error disabling scroll viewer: {es.Message}");
            }
        }



        /// <summary>
        /// Shows the context flyout for an AppBarButton.
        /// </summary>
        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((AppBarButton)sender).ContextFlyout.ShowAt((AppBarButton)sender);
        }   
        /// <summary>
        /// Handles mouse wheel changes for page navigation, delegating to ViewModel commands.
        /// </summary>
        private void GridViewMain_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint((UIElement)sender).Properties.MouseWheelDelta;
            if (delta > 0) // Scroll up/forward
            {
                ViewModel.NavigateNextPageCommand.Execute(null);
            }
            else // Scroll down/backward
            {
                ViewModel.NavigatePreviousPageCommand.Execute(null);
            }
        }

        /// <summary>
        /// Handles drag over events for potential page navigation during drag-and-drop.
        /// </summary>
        private async void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            Point currentPosition = e.GetPosition(GridViewMain);

            // Get the transform of the GridView to determine its absolute position relative to the window content
            GeneralTransform transform = GridViewMain.TransformToVisual(Window.Current.Content);
            Point gridViewOrigin = transform.TransformPoint(new Point(0, 0));

            // Check if drag is near left edge to navigate previous page
            if (currentPosition.X < (gridViewOrigin.X + 50)) // 50px from left edge
            {
                if (ViewModel.IsPreviousPageEnabled)
                {
                    ViewModel.NavigatePreviousPageCommand.Execute(null);
                    await Task.Delay(500); // Debounce page navigation during drag
                }
            }
            // Check if drag is near right edge to navigate next page
            else if (currentPosition.X > (gridViewOrigin.X + GridViewMain.ActualWidth - 50)) // 50px from right edge
            {
                if (ViewModel.IsNextPageEnabled)
                {
                    ViewModel.NavigateNextPageCommand.Execute(null);
                    await Task.Delay(500); // Debounce page navigation during drag
                }
            }
        }

        /// <summary>
        /// Handles the start of a drag operation, storing the initial index of the dragged item.
        /// </summary>
        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _isDragging = true;
            if (e.Items.Count > 0 && e.Items[0] is IApporFolder draggedItem)
            {
                // Store the initial index of the dragged item from the ViewModel's Apps collection
                _itemDragged.InitialIndex = ViewModel.Apps.IndexOf(draggedItem);
                // Set data for the drop operation
                e.Data.SetData("DraggedItem", draggedItem);
            }
        }

        /// <summary>
        /// Handles the drop of an item, calculating the target index and delegating to ViewModel for move.
        /// </summary>
        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            _isDragging = false; // Reset dragging state
            GridView view = sender as GridView;

            if (e.DataView.Contains("DraggedItem"))
            {
                // Find the position where item will be dropped in the gridview
                Point pos = e.GetPosition(view.ItemsPanelRoot);

                // Get the size of one of the list items (assuming all items are same size)
                GridViewItem gvi = (GridViewItem)view.ContainerFromIndex(0);
                if (gvi == null) return; // Should not happen if items exist

                double itemHeight = gvi.ActualHeight + gvi.Margin.Top + gvi.Margin.Bottom;
                double itemWidth = gvi.ActualWidth + gvi.Margin.Left + gvi.Margin.Right;

                // Determine the index of the item from the item position within the current GridView
                int targetIndexX = (int)(pos.X / itemWidth);
                int targetIndexY = (int)(pos.Y / itemHeight);

                // Calculate the linear index within the current page displayed by the GridView
                int listIndexOnPage = (targetIndexY * ViewModel.Columns) + targetIndexX;

                // Calculate the absolute index in the original collection managed by the ViewModel
                int targetAbsoluteIndex = (ViewModel.CurrentPageNum * ViewModel.AppsPerScreen) + listIndexOnPage;

                // Ensure the target index is within valid bounds of the *full* collection
                targetAbsoluteIndex = Math.Min(targetAbsoluteIndex, ViewModel.Apps.GetOriginalCollection().Count - 1);
                targetAbsoluteIndex = Math.Max(0, targetAbsoluteIndex); // Ensure not negative

                // Perform the move operation via the ViewModel
                ViewModel.MoveAppCommand.Execute(Tuple.Create(_itemDragged.InitialIndex, targetAbsoluteIndex));
            }
        }

        /// <summary>
        /// Handles item click events on the GridView, navigating to folder view or launching app.
        /// </summary>
        private void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            IApporFolder selectedItem = (IApporFolder)e.ClickedItem;
            if (selectedItem == null) return;

            if (selectedItem.GetType() == typeof(AppFolder))
            {
                Frame.Navigate(typeof(FolderDetailPage), selectedItem);
            }
            else if (selectedItem.GetType() == typeof(FinalTiles))
            {
                ViewModel.LaunchAppCommand.Execute(((FinalTiles)selectedItem).FullName);
            }
        }

        /// <summary>
        /// Disables scroll viewer when pointer enters the page.
        /// </summary>
        private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            disableScrollViewer(GridViewMain);
        }

        /// <summary>
        /// Handles right-tap (context menu) on a RelativePanel.
        /// </summary>
        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Show the context flyout directly from the UI element
            ((RelativePanel)sender).ContextFlyout.ShowAt((RelativePanel)sender);
        }

        /// <summary>
        /// Handles tap on the "Edit" context menu item.
        /// </summary>
        private void Edit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item is FinalTiles app)
            {
                // Navigate to AppDetailPage for app editing (pass true for edit mode)
                Frame.Navigate(typeof(AppDetailPage), Tuple.Create(app, true));
            }
            else if (item is AppFolder folder)
            {
                // Navigate to FolderDetailPage for folder editing (pass true for edit mode)
                Frame.Navigate(typeof(FolderDetailPage), Tuple.Create(folder, true));
            }
        }

        /// <summary>
        /// Handles tap on the "Info" context menu item.
        /// </summary>
        private void Info_Tapped(object sender, TappedRoutedEventArgs e)
        {
            object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item is FinalTiles app)
            {
                // Navigate to AppDetailPage for app info (default view mode)
                Frame.Navigate(typeof(AppDetailPage), app);
            }
            else if (item is AppFolder folder)
            {
                // Navigate to FolderDetailPage for folder info (default view mode)
                Frame.Navigate(typeof(FolderDetailPage), folder);
            }
        }
        /// <summary>
        /// Handles the Loaded event for the GridView, triggering layout recalculation in ViewModel.
        /// </summary>
        private void GridViewMain_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial layout calculation on GridView load
            ViewModel.RecalculateLayout(GridViewMain.ActualWidth, GridViewMain.ActualHeight);
        }
    }

 
}
