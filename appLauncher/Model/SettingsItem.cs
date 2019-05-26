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
        private List<SettingsItem> child;

        // Properties
        public string IconGlyphCode { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Constructors
        public SettingsItem(string title, string description, string iconGlyphCode)
        {
            Title = title;
            Description = description;
            IconGlyphCode = iconGlyphCode;
        }

        // Methods

        public void NavigateToChild()
        {
            if (child != null)
            {
                
            }
        }


    }
}
