using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Interfaces
{
    public interface ISettingsService
    {
        GlobalAppSettings TotalAppSettings { get; }
        Task LoadAppSettingsAsync();
        Task SaveAppSettingsAsync();
        void SetApplicationResources();
    }
}
