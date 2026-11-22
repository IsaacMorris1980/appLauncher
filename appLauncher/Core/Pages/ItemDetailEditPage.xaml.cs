using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ItemDetailEditPage : Page
    {
        public ItemDetailEditViewModel ViewModel { get; }

        public ItemDetailEditPage()
        {
            this.InitializeComponent();

            // Resolve ViewModel via DI container
            ViewModel = App.Services?.GetService(typeof(ItemDetailEditViewModel)) as ItemDetailEditViewModel
                        ?? (App.Current as dynamic)?.Services?.GetRequiredService<ItemDetailEditViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Pass the selected item parameter into ViewModel
            if (e.Parameter is IApporFolder item)
            {
                ViewModel?.Initialize(item);
            }
        }
    }
}
