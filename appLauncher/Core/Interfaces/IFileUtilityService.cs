// appLauncher.Core.Interfaces/IFileService.cs (Create this new file)
using System.Threading.Tasks;

using Windows.Storage; // If you need to return StorageFile/Folder directly, otherwise keep it clean

namespace appLauncher.Core.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Checks if a file is present in the specified location.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <param name="folderPath">Optional. The full path to the folder. If empty, checks the application's LocalFolder.</param>
        /// <returns>True if the file is found, false otherwise.</returns>
        Task<bool> IsFilePresentAsync(string fileName, string folderPath = "");

        Task<string> ReadTextFromFileAsync(string fileName);
        Task<bool> WriteTextToFileAsync(string fileName, string content);
        Task<byte[]> ReadBytesFromFileAsync(StorageFile file);
        Task<bool> DeleteFileAsync(string fileName);
    }
}