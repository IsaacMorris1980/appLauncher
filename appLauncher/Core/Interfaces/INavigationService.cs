using System;

using Windows.UI.Xaml.Controls; // For Frame

namespace appLauncher.Core.Services
{
    /// <summary>
    /// Defines the contract for a navigation service within the application.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Sets the root navigation frame for the service. This must be called once during app startup.
        /// </summary>
        /// <param name="frame">The root Frame of the application.</param>
        void SetRootFrame(Frame frame);

        /// <summary>
        /// Navigates to a specified page type, optionally passing a parameter.
        /// </summary>
        /// <param name="pageType">The type of the page to navigate to.</param>
        /// <param name="parameter">An optional parameter to pass to the target page's OnNavigatedTo method.</param>
        /// <returns>True if navigation was successful; otherwise, false.</returns>
        bool Navigate(Type pageType, object parameter = null);

        /// <summary>
        /// Navigates back to the previous page in the navigation stack.
        /// </summary>
        /// <returns>True if navigation was successful; otherwise, false.</returns>
        bool GoBack();

        // You can add more navigation methods here as needed, e.g.:
        // bool NavigateToAppDetail(AppTile app, bool isEditMode = false);
        // bool NavigateToFolderDetail(AppFolder folder, bool isEditMode = false);
    }
}
