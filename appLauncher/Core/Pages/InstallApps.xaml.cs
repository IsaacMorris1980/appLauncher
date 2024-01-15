using appLauncher.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Windows.Management.Deployment;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallApps : Page
    {
        private static PackageManager pkgMgr = new PackageManager();
        private string results;
        public InstallApps()
        {
            this.InitializeComponent();
        }
        private async void Deps_Tapped(object sender, TappedRoutedEventArgs e)
        {

            results = await InstallorRemoveApplication.LoadDependancies();
            if (results != "Success")
            {
                DepsInstalled.IsChecked = false;
            }
            DepsInstalled.IsChecked = true;
            DepsInstalled.Content = "Deps Installed";
        }

        private async void Certs_Tapped(object sender, TappedRoutedEventArgs e)
        {
            results = await InstallorRemoveApplication.InstallCertificate();
            if (results != "Success")
            {
                CertisInstalled.IsChecked = false;
            }
            CertisInstalled.IsChecked = true;
            CertisInstalled.Content = "Cert is installed";
        }
        private async void Install_Tapped(object sender, TappedRoutedEventArgs e)
        {
            results = await InstallorRemoveApplication.InstallCertificate();
            if (results == "Certficate Installed")
            {
                CertisInstalled.IsChecked = true;
            }
            results = await InstallorRemoveApplication.LoadDependancies();
            if (results == "Success")
            {
                DepsInstalled.IsChecked = true;

            }
            results = await InstallorRemoveApplication.InstallApplication();
            if (results != "Success")
            {
                ErrororSuccess.Text = results;
                InstallInfo.Text = "Retry to install application";
                CertisInstalled.IsChecked = false;
                DepsInstalled.IsChecked = false;
                AppisInstalled.IsChecked = false;
            }
            else
            {
                AppisInstalled.IsChecked = true;
                ErrororSuccess.Text = results;
                await Task.Delay(1500);
                CertisInstalled.IsChecked = false;
                DepsInstalled.IsChecked = false;
                AppisInstalled.IsChecked = false;
                InstallInfo.Text = "Install another application?";
            }
        }





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


        private void Install_Tapped_1(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
