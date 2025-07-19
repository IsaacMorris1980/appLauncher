using appLauncher.Core.Interfaces;
using appLauncher.Core.Services; // For ILoggingService

using System;
using System.IO; // For Stream
using System.Runtime.InteropServices.WindowsRuntime; // For AsBuffer()
using System.Threading.Tasks;

using Windows.Storage;

namespace appLauncher.Core.Services
{
    /// <summary>
    /// Provides utility functions for file system operations, such as checking file presence,
    /// reading text, writing text, and converting files to byte arrays.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        /// <param name="loggingService">The logging service to use for error logging.</param>
        public FileService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Checks if a file exists in the specified folder or the application's local folder.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <param name="folderPath">Optional. The path to the folder where the file is expected. If empty, checks the application's local folder.</param>
        /// <returns>True if the file is present; otherwise, false.</returns>
        //public async Task<bool> IsFilePresent(string fileName, string folderPath = "")
        //{
        //    IStorageItem item = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(folderPath))
        //        {
        //            item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
        //        }
        //        else
        //        {
        //            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        //            item = await folder.TryGetItemAsync(fileName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await _loggingService.LogExceptionAsync(ex);
        //        return false; // Assume file not present if an error occurs during access check
        //    }
        //    return item != null;
        //}

        /// <summary>
        /// Reads all text from a specified file in the application's local folder.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        /// <returns>The content of the file as a string, or null if the file does not exist or an error occurs.</returns>
        public async Task<string> ReadTextFromFileAsync(string fileName)
        {
            try
            {
                StorageFile file = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
                if (file != null)
                {
                    return await FileIO.ReadTextAsync(file);
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
            }
            return null; // Return null if file not found or error
        }

        /// <summary>
        /// Writes text to a specified file in the application's local folder.
        /// If the file exists, it will be replaced.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="content">The text content to write.</param>
        /// <returns>True if the write operation was successful; otherwise, false.</returns>
        public async Task<bool> WriteTextToFileAsync(string fileName, string content)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, content);
                return true;
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return false;
            }
        }

        /// <summary>
        /// Converts a <see cref="StorageFile"/> representing an image into a byte array.
        /// </summary>
        /// <param name="file">The <see cref="StorageFile"/> to convert.</param>
        /// <returns>A byte array representing the file data, or null if an error occurs.</returns>
        public async Task<byte[]> ReadBytesFromFileAsync(StorageFile file)
        {
            if (file == null) return null;

            try
            {
                using (var inputStream = await file.OpenSequentialReadAsync())
                {
                    var readStream = inputStream.AsStreamForRead();
                    byte[] buffer = new byte[readStream.Length];
                    await readStream.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return null;
            }
        }

        /// <summary>
        /// Deletes a specified file from the application's local folder.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>True if the file was deleted or did not exist; otherwise, false if an error occurred.</returns>
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                StorageFile file = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
                if (file != null)
                {
                    await file.DeleteAsync();
                }
                return true; // Return true if deleted or not found (already gone)
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return false;
            }
        }

        public async Task<bool> IsFilePresentAsync(string fileName, string folderPath = "")
        {
            try
            {
                IStorageItem item;
                if (folderPath == "")
                {
                    item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
                }
                else
                {
                    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                    item = await folder.TryGetItemAsync(fileName);
                }
                return item != null;
            }
            catch (Exception ex)
            {
                await _loggingService.LogExceptionAsync(ex);
                return false;
            }
        }
    }
}