using System.Threading.Tasks;

using Windows.ApplicationModel; // Required for Package
using Windows.System; // Required for Launcher

namespace appLauncher.Core.Services
{
    /// <summary>
    /// Defines the contract for a service responsible for both application installation processes
    /// and launching installed applications.
    /// </summary>
    public interface IInstallationService
    {
        /// <summary>
        /// Attempts to install a certificate.
        /// </summary>
        /// <returns>A string indicating the result of the certificate installation (e.g., "Success", "Error message").</returns>
        Task<string> InstallCertificateAsync();

        /// <summary>
        /// Attempts to load and install application dependencies.
        /// </summary>
        /// <returns>A string indicating the result of the dependency installation (e.g., "Success", "Error message").</returns>
        Task<string> LoadDependenciesAsync();

        /// <summary>
        /// Attempts to install the main application package.
        /// </summary>
        /// <returns>A string indicating the result of the application installation (e.g., "Success", "Error message").</returns>
        Task<string> InstallApplicationAsync();

        /// <summary>
        /// Launches an application given its full name.
        /// </summary>
        /// <param name="packageFullName">The full name of the package to launch.</param>
        /// <returns>A Task representing the asynchronous operation, indicating success or failure of the launch.</returns>
        Task<bool> LaunchApplicationAsync(string packageFullName);

        /// <summary>
        /// Removes an application given its full name.
        /// </summary>
        /// <param name="packageFullName">The full name of the package to remove.</param>
        /// <returns>A Task representing the asynchronous operation, indicating success or failure of the removal.</returns>
        Task<bool> RemoveApplicationAsync(string packageFullName);
    }
}
