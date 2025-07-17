using appLauncher.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// Removed: using Microsoft.Extensions.DependencyInjection; // No longer needed
// Removed: using appLauncher.Core.Helpers; // No longer needed as logic moved to service/VM
// Removed: using System.Security.Cryptography.X509Certificates; // No longer needed
// Removed: using Windows.Management.Deployment; // No longer needed
using appLauncher.Core.Services; // Import your new ServiceLocator

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the InstallApps page.
    /// Delegates all installation logic to the <see cref="InstallAppsViewModel"/>.
    /// </summary>
    public sealed partial class InstallApps : Page
    {
        public InstallAppsViewModel ViewModel { get; set; }

        public InstallApps()
        {
            // Resolve the ViewModel using your manual ServiceLocator
            ViewModel = App.ServiceLocator.Resolve<InstallAppsViewModel>();
            this.DataContext = ViewModel;

            this.InitializeComponent();

            // If you have a BooleanToVisibilityConverter defined in a shared location (e.g., App.xaml
            // or a separate Converters.cs file in appLauncher.Core.Converters namespace),
            // you don't need to add it here. If it's only defined within this page's code-behind
            // for XAML usage, you might keep a class definition here.
            // For this version, assuming it's either global or in a dedicated Converters.cs file.
        }

        // All original private fields (pkgMgr, results) are removed.
        // All original static methods (InstallCertificate, LoadDependancies, InstallApplication) are removed,
        // as their logic is now encapsulated within the IInstallationService and called by the ViewModel.
        // All original event handlers (Deps_Tapped, Certs_Tapped, Install_Tapped, Install_Tapped_1) are removed,
        // as their logic is now handled by commands in the ViewModel.
    }
}
