using appLauncher.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.SortComparer
{
    internal class DevNameComparerAscending : IComparer<IApporFolder>
    {
        public int Compare(IApporFolder x, IApporFolder y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1; 
            if (y == null) return 1;
            return x.Developer.CompareTo(y.Developer);            
        }
    }
    public class DevNameComparerDescending : IComparer<IApporFolder>
    {
        public int Compare(IApporFolder x, IApporFolder y)
        {
            if (x==null && y==null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return y.Developer.CompareTo(x.Developer);
        }
    }
}
