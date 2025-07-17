using System;
using System.Reflection;

using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.Services
{
    /// <summary>
    /// A concrete implementation of <see cref="INavigationService"/> that uses
    /// the UWP Frame for navigation.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private Frame _rootFrame;

        /// <summary>
        /// Sets the root navigation frame for the service. This must be called once during app startup.
        /// </summary>
        /// <param name="frame">The root Frame of the application.</param>
        public void SetRootFrame(Frame frame)
        {
            _rootFrame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        /// <summary>
        /// Navigates to a specified page type, optionally passing a parameter.
        /// </summary>
        /// <param name="pageType">The type of the page to navigate to.</param>
        /// <param name="parameter">An optional parameter to pass to the target page's OnNavigatedTo method.</param>
        /// <returns>True if navigation was successful; otherwise, false.</returns>
        public bool Navigate(Type pageType, object parameter = null)
        {
            if (_rootFrame == null)
            {
                System.Diagnostics.Debug.WriteLine("NavigationService: Root Frame is not set.");
                return false;
            }

            // Ensure the pageType is a valid Page
            if (!typeof(Page).IsAssignableFrom(pageType))
            {
                System.Diagnostics.Debug.WriteLine($"NavigationService: {pageType.Name} is not a valid Page type.");
                return false;
            }

            return _rootFrame.Navigate(pageType, parameter);
        }

        /// <summary>
        /// Navigates back to the previous page in the navigation stack.
        /// </summary>
        /// <returns>True if navigation was successful; otherwise, false.</returns>
        public bool GoBack()
        {
            if (_rootFrame != null && _rootFrame.CanGoBack)
            {
                _rootFrame.GoBack();
                return true;
            }
            return false;
        }
    }
}
