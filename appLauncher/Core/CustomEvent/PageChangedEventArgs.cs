using System;

namespace appLauncher.Core.CustomEvent
{
    public delegate void PageChangedDelegate(PageChangedEventArgs e);
    public class PageChangedEventArgs : EventArgs
    {
        private int _pageIndex;


        public PageChangedEventArgs(int pageIndex)
        {
            _pageIndex = pageIndex;

        }

        public int PageIndex { get { return _pageIndex; } }
    }
}
