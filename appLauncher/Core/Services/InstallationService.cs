using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates; // Note: UWP sandbox limitations apply to X509Store
using System.Threading.Tasks;

using appLauncher.Core.Interfaces;
using Windows.ApplicationModel; // Required for Package
using Windows.Management.Deployment;
using Windows.Storage; // For FileOpenPicker
using Windows.System; // Required for Launcher
using Windows.UI.Popups; // For MessageDialog (used for user feedback, not alerts)

namespace appLauncher.Core.Services
{
    /// <summary>
    /// A service that encapsulates the logic for installing certificates, dependencies, and applications,
    /// and also for launching and removing installed applications.
    /// </summary>
    public class InstallationService : IInstallationService
    {
        private readonly PackageManager _packageManager;
        private readonly ILoggingService _loggingService;

        public InstallationService(ILoggingService loggingService)
        {
            _packageManager = new PackageManager();
            _loggingService = loggingService;
        }

        /// <summary>
        /// Attempts to install a certificate from a user-selected .cer file.
        /// NOTE: Direct certificate installation using X509Store is generally NOT supported
        /// for sandboxed UWP applications without special capabilities or a brokered component.
        /// This method is included for structural completeness based on original code,
        /// but its functionality might be limited or require a different approach in a deployed UWP app.
        /// </summary>
        /// <returns>A string indicating the result of the certificate installation.</returns>
        public async Task<string> InstallCertificateAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add(".cer");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file == null)
                {
                    return "Certificate installation cancelled by user.";
                }

                // This part is problematic for UWP sandbox
                // X509Certificate2 cert = new X509Certificate2(file.Path);
                // using (X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.CurrentUser))
                // {
                //     store.Open(OpenFlags.MaxAllowed);
                //     store.Add(cert);
                // }

                // For UWP, typically the certificate is installed by the user manually,
                // or it's bundled with the appx/appxbundle and handled by PackageManager.
                // Simulating success for demonstration, but real-world UWP needs different handling.
                await new MessageDialog("Certificate selected. (Note: Direct certificate installation via X509Store is limited in UWP sandbox.)", "Certificate Installation").ShowAsync();
                return "Certficate Selected (Manual Installation Required)"; // Changed message to reflect reality

            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return $"Failed to install certificate: {ex.Message}";
            }
        }

        /// <summary>
        /// Attempts to load and install application dependencies (.appx, .appxbundle).
        /// Idea and code from https://github.com/colinkiama/UWP-Package-Installer
        /// </summary>
        /// <returns>A string indicating the result of the dependency installation.</returns>
        public async Task<string> LoadDependenciesAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add(".appx");
                picker.FileTypeFilter.Add(".appxbundle");

                var files = await picker.PickMultipleFilesAsync();
                if (files != null && files.Count > 0)
                {
                    foreach (var dependency in files)
                    {
                        // PackageManager.AddPackageAsync is compatible with 15063
                        await _packageManager.AddPackageAsync(new Uri(dependency.Path), null, DeploymentOptions.None);
                    }
                    return "Success";
                }
                return "Dependency installation cancelled or no files selected.";
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return $"Failed to load dependencies: {ex.Message}";
            }
        }

        /// <summary>
        /// Attempts to install the main application package (.appx, .appxbundle).
        /// Idea and code from https://github.com/colinkiama/UWP-Package-Installer
        /// </summary>
        /// <returns>A string indicating the result of the application installation.</returns>
        public async Task<string> InstallApplicationAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add(".appx");
                picker.FileTypeFilter.Add(".appxbundle");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // PackageManager.AddPackageAsync is compatible with 15063
                    await _packageManager.AddPackageAsync(new Uri(file.Path), null, DeploymentOptions.None);
                    return "Success";
                }
                return "Application installation cancelled or no file selected.";
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return $"Failed to install application: {ex.Message}";
            }
        }

        /// <summary>
        /// Launches an application given its full name.
        /// </summary>
        /// <param name="packageFullName">The full name of the package to launch.</param>
        /// <returns>A Task representing the asynchronous operation, indicating success or failure of the launch.</returns>
        public async Task<bool> LaunchApplicationAsync(string packageFullName)
        {
            try
            {
                // Find the package by its full name
                Package package = _packageManager.FindPackageForUser(string.Empty, packageFullName); // string.Empty for current user

                if (package != null)
                {
                    // Get the main application entry point for the package
                    var appEntries = await package.GetAppListEntriesAsync();
                    if (appEntries.Count > 0)
                    {
                        // Launch the first app entry
                        bool launched = await appEntries[0].LaunchAsync();
                        if (!launched)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to launch app {packageFullName}.");
                        }
                        return launched;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"No app entries found for package {packageFullName}.");
                        return false;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Package {packageFullName} not found for the current user.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                System.Diagnostics.Debug.WriteLine($"Error launching application {packageFullName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes an application given its full name.
        /// </summary>
        /// <param name="packageFullName">The full name of the package to remove.</param>
        /// <returns>True if the application was successfully removed; otherwise, false.</returns>
        public async Task<bool> RemoveApplicationAsync(string packageFullName)
        {
            try
            {
                // PackageManager.RemovePackageAsync is compatible with 15063
                // The method takes the package full name directly.
                await _packageManager.RemovePackageAsync(packageFullName, (RemovalOptions)DeploymentOptions.None);
                System.Diagnostics.Debug.WriteLine($"Application {packageFullName} removed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                System.Diagnostics.Debug.WriteLine($"Error removing application {packageFullName}: {ex.Message}");
                return false;
            }
        }
    }
}
