using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
    public class LayoutLoadResult
    {
        public List<IApporFolder> Items { get; set; }
        public HashSet<string> ActiveFullNames { get; set; }
    }
}
