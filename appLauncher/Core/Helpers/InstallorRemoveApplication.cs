using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Management.Deployment;

namespace appLauncher.Core.Helpers
{
    public static class InstallorRemoveApplication
    {
        private static PackageManager pkgMgr = new PackageManager();
        //Idea and code from https://github.com/colinkiama/EasyCertInstall/blob/master/EasyCertInstall/Program.cs
        public static async Task<string> InstallCertificate()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add(".cer");
                var files = await picker.PickSingleFileAsync();
                X509Certificate2 cert = new X509Certificate2(files.Path);
                using (X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.MaxAllowed);
                    store.Add(cert);
                }
                return "Certficate Installed";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //Idea and code from https://github.com/colinkiama/UWP-Package-Installer
        public static async Task<string> LoadDependancies()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                List<Uri> depuris = new List<Uri>();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add(".appx");
                picker.FileTypeFilter.Add(".appxbundle");

                var files = await picker.PickMultipleFilesAsync();
                if (files != null)
                {

                    foreach (var dependency in files)
                    {
                        await pkgMgr.AddPackageAsync(new Uri(dependency.Path), null, DeploymentOptions.None);
                    }
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
        //Idea and code from  https://github.com/colinkiama/UWP-Package-Installer
        public static async Task<string> InstallApplication()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                List<Uri> depuris = new List<Uri>();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add(".appx");
                picker.FileTypeFilter.Add(".appxbundle");

                var files = await picker.PickSingleFileAsync();
                if (files != null)
                {
                    await pkgMgr.AddPackageAsync(new Uri(files.Path), null, DeploymentOptions.None);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Failed to install {ex.Message}";
            }

        }
        public static async Task<string> RemoveApplication(string fullname)
        {
            string returnValue = string.Empty;
            IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation =
       pkgMgr.RemovePackageAsync(fullname, RemovalOptions.None);

            ManualResetEvent opCompletedEvent = new ManualResetEvent(false);

            // Define the delegate using a statement lambda
            deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };

            // Wait until the operation completes
            opCompletedEvent.WaitOne();

            // Check the status of the operation
            if (deploymentOperation.Status == AsyncStatus.Error)
            {
                DeploymentResult deploymentResult = deploymentOperation.GetResults();
                Debug.WriteLine("Error code: {0}", deploymentOperation.ErrorCode);
                Debug.WriteLine("Error text: {0}", deploymentResult.ErrorText);
                returnValue = $"Error code {deploymentOperation.ErrorCode} Error text: {deploymentResult.ErrorText} ";
            }
            else if (deploymentOperation.Status == AsyncStatus.Canceled)
            {
                Debug.WriteLine("Removal canceled");
                returnValue = "Removal Canceled";
            }
            else if (deploymentOperation.Status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Removal succeeded");
                PackageHelper.Apps.RemoveApp(fullname);

                PackageHelper.RemoveFromSearch(fullname);

                await PackageHelper.SaveCollectionAsync();
                returnValue = "Removal Succeeded";
            }
            else
            {
                returnValue = "Removal status unknown";
                Debug.WriteLine("Removal status unknown");
            }

            return returnValue;
        }


    }
}
