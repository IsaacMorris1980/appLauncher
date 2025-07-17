// appLauncher.Core.Interfaces/ILoggingService.cs
using System;
using System.Threading.Tasks;

namespace appLauncher.Core.Interfaces
{
    public interface ILoggingService
    {
        /// <summary>
        /// Logs an exception to a persistent store (e.g., a file).
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        Task LogExceptionAsync(Exception exception);
        // Task LogWarningAsync(string message);
        // Task LogErrorAsync(string message);
    }

}