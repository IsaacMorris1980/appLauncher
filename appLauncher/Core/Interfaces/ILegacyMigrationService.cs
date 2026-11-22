using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Interfaces
{
    public interface ILegacyMigrationService
    {
        Task<bool> HasLegacyFilesAsync();
        Task<bool> HasLegacySettingsAsync();
        Task<bool> HasLegacyAppAndFoldersAsync();
        Task<bool> HasLegacyBackgroundImagesAsync();

        Task MigrateAllLegacyDataAsync();
        Task MigrateSettingsAsync();
        Task MigrateAppAndFoldersAsync();
        Task MigrateBackgroundImagesAsync();
    }
}
