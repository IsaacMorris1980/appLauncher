// appLauncher.Core.Services/LoggingService.cs
using Newtonsoft.Json; // Make sure this NuGet package is installed in your project
using System;
using System.Threading.Tasks;
using Windows.Storage;
using appLauncher.Core.Interfaces; // Reference the interface

namespace appLauncher.Core.Services
{
    public class LoggingService : ILoggingService
    {
        private const string LogFileName = "errors.json"; // Changed from "erros.json" for correct spelling

        /// <summary>
        /// Logs an exception by serializing it to a JSON string and appending it to a log file.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public async Task LogExceptionAsync(Exception exception)
        {
            try
            {
                // Serialize the exception details. Using exception.ToString() provides more detail than just exception.Message
                string logEntry = JsonConvert.SerializeObject(
                    new
                    {
                        Timestamp = DateTimeOffset.Now,
                        ExceptionType = exception.GetType().FullName,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        InnerException = exception.InnerException?.Message, // Get message of inner exception if it exists
                        FullDetails = exception.ToString() // Provides full exception details
                    },
                    Formatting.Indented
                );

                StorageFile logFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(LogFileName, CreationCollisionOption.OpenIfExists);

                // Append the log entry with a separator for readability
                await FileIO.AppendTextAsync(logFile, logEntry + Environment.NewLine + "---" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // If logging fails, fall back to debug output. This is a critical failure.
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: Failed to log exception. Original exception: {exception.Message}. Logging exception: {ex.Message}");
            }
        }       
    }
}