using Windows.UI.Core;

namespace appLauncher.Core
{
    public static class DesktopBackButton
    {
        public static void ShowBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

        }

        public static void HideBackButton()
        {

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

    }
}
