using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using appLauncher.Core.Interfaces;

namespace appLauncher.Core.SortComparer
{
    public class NameComparerAscending : IComparer<IApporFolder>
    {
        public int Compare(IApporFolder x, IApporFolder y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
          return  string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase); 
        }     
    }
    public class NameComparerDescending : IComparer<IApporFolder>
    {
        public int Compare (IApporFolder x,IApporFolder y)
        {
            if (x==null && y==null) return 0;
            if (x==null) return -1;
            if(y==null) return 1;
            return string.Compare(y.Name,x.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
