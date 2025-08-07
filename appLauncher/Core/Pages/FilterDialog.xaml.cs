using appLauncher.Core.Helpers;

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    public sealed partial class FilterDialog : ContentDialog
    {
        public FilterDialog()
        {
            this.InitializeComponent();
        }
        private  string filterstring = string.Empty;

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PackageHelper.Apps.GetFilteredApps(filterstring);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PackageHelper.Apps.GetFilteredApps(String.Empty);
        }

        private void AppName_Checked(object sender,RoutedEventArgs e)
        {
            filterOptions.Visibility = Visibility.Collapsed;
            if (DevName.IsChecked==true)
            {
                DevName.IsChecked = false;
            }
            if (InstalledDate.IsChecked==true)
            {
                InstalledDate.IsChecked = false;
            }
            AZ.Content = "Filter A-Z";
            ZA.Content = "Filter Z-A";
            filterOptions.Visibility = Visibility.Visible;
        }

        private void AppName_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((DevName.IsChecked == false) && (InstalledDate.IsChecked == false))
            {
                filterOptions.Visibility = Visibility.Collapsed;
            }
            
        }

        private void DevName_Checked(object sender, RoutedEventArgs e)
        {
            filterOptions.Visibility = Visibility.Collapsed;
            if (AppName.IsChecked == true)
            {
                AppName.IsChecked = false;
            }
            if (InstalledDate.IsChecked == true)
            {
                InstalledDate.IsChecked = false;
            }
            AZ.Content = "Filter A-Z";
            ZA.Content = "Filter Z-A";
            filterOptions.Visibility = Visibility.Visible;
        }

        private void DevName_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((AppName.IsChecked == false) && (InstalledDate.IsChecked == false))
            {
                filterOptions.Visibility = Visibility.Collapsed;
            }
        }

        private void InstalledDate_Checked(object sender, RoutedEventArgs e)
        {
            filterOptions.Visibility = Visibility.Collapsed;
            AZ.Content = "Filter Newest - Oldest";
            ZA.Content = "Filter Oldest - Newest";
            filterOptions.Visibility = Visibility.Visible;
            setfilteroptions();
        }

        private void InstalledDate_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((AppName.IsChecked == false) && (InstalledDate.IsChecked == false))
            {
                filterOptions.Visibility = Visibility.Collapsed;
            }
            setfilteroptions();
        }

        private void AZ_Checked(object sender, RoutedEventArgs e)
        {
            setfilteroptions();
        }

        private void AZ_Unchecked(object sender, RoutedEventArgs e)
        {
            setfilteroptions();
        }

        private void ZA_Checked(object sender, RoutedEventArgs e)
        {
            setfilteroptions();
        }

        private void ZA_Unchecked(object sender, RoutedEventArgs e)
        {
            setfilteroptions();
        }

        private void setfilteroptions()
        {
            if (AppName.IsChecked == true)
            {
                DevName.IsChecked = false;
                InstalledDate.IsChecked = false;
                if (AZ.IsChecked == true)
                {
                    filterstring = "alphaAZ";
                }
                else if (ZA.IsChecked == true)
                {
                    filterstring = "alphaZA";
                }
                else
                {
                    filterstring = string.Empty;
                }
            }
            else if (DevName.IsChecked==true)
            {
                AppName.IsChecked = false;
                InstalledDate.IsChecked = false;
                if (AZ.IsChecked == true)
                {
                    filterstring = "devAZ";
                }
                else if (ZA.IsChecked == true)
                {
                    filterstring = "devZA";
                }
                else
                {
                    filterstring = string.Empty;
                }
            }
            else if (InstalledDate.IsChecked==true)
            {
                AppName.IsChecked = false;
                DevName.IsChecked = false;
                if (AZ.IsChecked == true)
                {
                    filterstring = "installNewest";
                }
                else if (ZA.IsChecked == true)
                {
                    filterstring = "alphaZA";
                }
                else
                {
                    filterstring = string.Empty;
                }
            }
            else
            {
                filterstring = string.Empty;
            }
        }

       
    }
}
