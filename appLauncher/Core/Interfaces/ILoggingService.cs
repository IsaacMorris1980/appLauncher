using appLauncher.Core.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Interfaces
{
    public interface ILoggingService
    {
        void Log(string message, LogLevel level = LogLevel.Info);
        void LogException(Exception ex, string message = null);
        Task FlushAsync();
    }
}
