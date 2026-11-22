using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Interfaces
{
    public interface IPackageService
    {
        IReadOnlyList<IApporFolder> Items { get; }
        Task LoadCollectionAsync();
        Task SaveCollectionAsync();
        Task<bool> LaunchApp(string fullname)
        Task<string> InstallCertificateAsync();
        Task<string> LoadDependenciesAsync();
        Task<string> InstallApplicationAsync();
        Task<string> RemoveApplicationAsync(string packageFullName);
        void UpdateFolderContent(AppFolder folder, IEnumerable<FinalTiles> updatedApps);
        void UpdateApp(FinalTiles app); // Added here
    }
}
