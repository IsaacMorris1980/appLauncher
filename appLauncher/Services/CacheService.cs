using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Services
{
    public sealed class CacheService
    {
        public static Stack<List<SettingsItem>> SettingsCache = null;
        public static Stack<string> SettingsViewTitleCache = null;
    }
}
