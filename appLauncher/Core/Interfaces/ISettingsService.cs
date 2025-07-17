using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;

namespace appLauncher.Core.Interfaces
{
    public interface ISettingsService
    {
        AppSettings AppSettings { get; }
        Task LoadAppSettingsAsync();
        Task SaveAppSettingsAsync();
        List<ColorComboItem> GetStaticColorPropertyBag();
        ColorComboItem MatchColor(Color c);
        void SetApplicationResources();
    }
}
