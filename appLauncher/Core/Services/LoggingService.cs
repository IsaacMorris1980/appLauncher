using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Services
{
    public class LoggingService : ILogging
    {
        public async Task<bool> WriteLog(string itemstolog)
        {
            await Task.Delay(1500);
            return true;
        }
    }
}
