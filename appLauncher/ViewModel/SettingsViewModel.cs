using appLauncher.Core;
using appLauncher.Model;
using appLauncher.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace appLauncher.ViewModel
{
    class SettingsViewModel : Notifier
    {
        private const string RootSettingsString = "Settings";
        private string _viewTitle;


        private Stack<List<SettingsItem>> _settingsContexts = new Stack<List<SettingsItem>>();
        private Stack<string> _viewTitles = new Stack<string>();

        public string ViewTitle
        {
            get { return _viewTitle; }
            set
            {
                _viewTitle = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SettingsItem> _settingsItems;

        public ObservableCollection<SettingsItem> SettingsItems
        {
            get { return _settingsItems; }
            set
            {
                _settingsItems = value;
                NotifyPropertyChanged();
            }
        }


        public SettingsViewModel()
        {
            GenerateDefaultItems();
        }

        private void GenerateDefaultItems()
        {
            List<SettingsItem> personalisationSettings = new List<SettingsItem>
            {
                new SettingsItem("Background", "Customise background type", "\uEB9F",null),
                new SettingsItem("Font", "Change font size", "\uE8D2", null)
            };



            List<SettingsItem> itemsToAdd = new List<SettingsItem>
            {
                new SettingsItem("Personalisation", "Customise background, Font Size", "\uE771", personalisationSettings),
                new SettingsItem("About", "Support Info, Open Source Licenses", "\uE897", null)
            };


            AddSettingsContext(itemsToAdd, RootSettingsString);
        }

        internal void HandleGoingBack(NavigatingCancelEventArgs e)
        {
            if (_settingsContexts.Count > 0)
            {
                e.Cancel = true;

                if (_settingsContexts.Count == 1)
                {
                    UpdateData(_settingsContexts.Pop(), _viewTitles.Pop());

                }
                else
                {
                    _settingsContexts.Pop();
                    _viewTitles.Pop();
                    UpdateData(_settingsContexts.Pop(), _viewTitles.Pop());
                }
            }
        }

        internal void SettingsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (SettingsItem)e.ClickedItem;
            SettingsItemArgs childrenArgs = clickedItem.NavigateToChild();
            if (childrenArgs.SettingsItems != null)
            {
                AddSettingsContext(childrenArgs.SettingsItems, childrenArgs.ViewTitle);
            }
            else
            {
                NavService.Instance.Navigate(childrenArgs.ViewType);
            }

        }

        private void AddSettingsContext(List<SettingsItem> itemsToAdd, string viewTitle)
        {
            _settingsContexts.Push(itemsToAdd);
            _viewTitles.Push(viewTitle);
            UpdateData(itemsToAdd, viewTitle);
        }

        private void UpdateData(List<SettingsItem> freshSettings, string viewTitle = "Settings")
        {
            SettingsItems = new ObservableCollection<SettingsItem>(freshSettings);
            ViewTitle = viewTitle;
        }
    }
}
