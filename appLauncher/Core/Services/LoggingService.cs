using appLauncher.Core.Enums;
using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;

namespace appLauncher.Core.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);
        private const string LogFileName = "app_launcher.log";

        /// <summary>
        /// Logs a general text message with a specified log level.
        /// </summary>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            string formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Debug.WriteLine(formattedMessage);

            // Fire-and-forget file write to keep synchronous caller paths unblocked
            _ = WriteToFileAsync(formattedMessage);
        }

        /// <summary>
        /// Logs an exception with an optional context message and stack trace.
        /// </summary>
        public void LogException(Exception ex, string message = null)
        {
            string context = string.IsNullOrEmpty(message) ? "An error occurred" : message;
            string formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {context}: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}";

            Debug.WriteLine(formattedMessage);
            _ = WriteToFileAsync(formattedMessage);
        }

        /// <summary>
        /// Safely writes log entries to the local app folder using a semaphore lock.
        /// </summary>
        private async Task WriteToFileAsync(string logEntry)
        {
            await _fileLock.WaitAsync();
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(LogFileName, CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(file, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoggingService] Failed to write to log file: {ex.Message}");
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task FlushAsync()
        {
            await Task.CompletedTask;
        }
    }
}
