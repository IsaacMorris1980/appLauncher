using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Services
{
    public static class PackageService 
    {
        public static AppPaginationObservableCollection AllApps { get; set; } = new AppPaginationObservableCollection();
        public static Task Load()
        {
            throw new NotImplementedException();
        }
        public static Task Reload()
        {
            throw new NotImplementedException();
        }

        public static Task Save()
        {
            throw new NotImplementedException();
        }
    }
}
