using System;

namespace appLauncher.Core.CustomEvent
{

    public delegate void PageNumChangedDelegate(PageNumChangedArgs e);
    public class PageNumChangedArgs : EventArgs
    {
        public int numofpages;
        public PageNumChangedArgs(int numofpages)
        {
            this.numofpages = numofpages;
        }
    }
}


