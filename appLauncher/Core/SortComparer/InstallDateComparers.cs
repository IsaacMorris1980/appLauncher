using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.SortComparer
{
    internal class InstallDateComparerNewest : IComparer<IApporFolder>
    {
        public int Compare(IApporFolder x, IApporFolder y)
        {
            if(x == null && y == null) return 0; // Both are null, considered equal
            if (x == null) return -1;            // x is null, y is not, x comes first
            if (y == null) return 1;

            return y.InstalledDate.CompareTo(x.InstalledDate);
        }
    }
    public class InstallDateComparerOldest : IComparer<IApporFolder>
    {
        public int Compare (IApporFolder x,IApporFolder y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.InstalledDate.CompareTo(y.InstalledDate);  
        }
    }
}
