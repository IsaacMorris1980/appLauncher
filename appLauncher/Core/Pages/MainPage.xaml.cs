// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using appLauncher.Core.CustomEvent;

using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.ViewModels;

using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace appLauncher.Core.Pages
{

    /// <summary>
    /// The page where the apps are displayed. Most of the user interactions with the app launcher will be here.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; }

        public MainPage()
        {
            this.InitializeComponent();

            // Resolve ViewModel via DI container
            ViewModel = App.Services?.GetService(typeof(MainPageViewModel)) as MainPageViewModel
                        ?? (App.Current as dynamic)?.Services?.GetRequiredService<MainPageViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel?.LoadBackground();
        }

        private void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ViewModel?.ItemClickCommand != null && ViewModel.ItemClickCommand.CanExecute(e.ClickedItem))
            {
                ViewModel.ItemClickCommand.Execute(e.ClickedItem);
            }
        }
    }
}