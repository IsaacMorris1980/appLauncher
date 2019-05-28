using appLauncher.Views.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Model
{
    public class SettingsItem
    {

        
        // Fields
        private List<SettingsItem> _children;

        // Constants
        private const string SettingsViewString = "SettingsView";
        private const string SettingsViewsNamespaceStartString = "appLauncher.Views.Settings.";

        // Properties
        public string IconGlyphCode { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Constructors
        public SettingsItem(string title, string description, string iconGlyphCode, List<SettingsItem> children)
        {
            Title = title;
            Description = description;
            IconGlyphCode = iconGlyphCode;
            _children = children;
        }

        // Methods

        public SettingsItemArgs NavigateToChild()
        {
            SettingsItemArgs argsToReturn;
            if (_children != null)
            {
                argsToReturn = new SettingsItemArgs(typeof(SettingsView), Title, _children);
            }
            else
            {
                argsToReturn = new SettingsItemArgs(Type.GetType(SettingsViewsNamespaceStartString + Title + SettingsViewString,true));
            }

            return argsToReturn;
        }
    }

    public class SettingsItemArgs
    {
        public string ViewTitle { get; set; }
        public Type ViewType { get; set; }
        public List<SettingsItem> SettingsItems { get; set; }
        
        public SettingsItemArgs(Type viewType, string viewTitle = null, List<SettingsItem> settingsItems = null)
        {
            ViewTitle = viewTitle;
            ViewType = viewType;
            SettingsItems = settingsItems;
        }
    }


}
