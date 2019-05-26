using appLauncher.Core;
using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.ViewModel
{
    class SettingsViewModel:Notifier
    {
        private string _viewTitle;

        public string ViewTitle
        {
            get { return _viewTitle; }
            set { _viewTitle = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SettingsItem> _settingsItems;

        public ObservableCollection<SettingsItem> SettingsItems
        {
            get { return _settingsItems; }
            set { _settingsItems = value;
                NotifyPropertyChanged();
            }
        }

        


    }
}
