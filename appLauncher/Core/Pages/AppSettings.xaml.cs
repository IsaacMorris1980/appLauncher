
using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;

using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppSettings : Page
    {
        public SettingsViewModel ViewModel { get; }

        public AppSettings()
        {
            this.InitializeComponent();

            // Resolve ViewModel via dependency injection container from App
            ViewModel = App.Services?.GetService(typeof(SettingsViewModel)) as SettingsViewModel
                        ?? (App.Current as dynamic)?.Services?.GetRequiredService<SettingsViewModel>();
        }
    }
}
