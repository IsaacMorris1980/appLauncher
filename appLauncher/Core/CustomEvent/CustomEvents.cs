﻿using System;

namespace appLauncher.Core.CustomEvent
{
    public delegate void AppPageSizeChangedDelegate(AppPageSizeChangedEventArgs e);
    public delegate void PageChangedDelegate(PageChangedEventArgs e);
    public delegate void PageNumChangedDelegate(PageNumChangedArgs e);
    public class AppPageSizeChangedEventArgs : EventArgs
    {
        private int _appPageSize;
        public AppPageSizeChangedEventArgs(int appPageSize)
        {
            _appPageSize = appPageSize;
        }
        public int AppPageSize { get { return _appPageSize; } }
    }
    public class PageChangedEventArgs : EventArgs
    {
        private int _pageIndex;


        public PageChangedEventArgs(int pageIndex)
        {
            _pageIndex = pageIndex;

        }

        public int PageIndex { get { return _pageIndex; } }
    }
    public class PageNumChangedArgs : EventArgs
    {
        public int numofpages;
        public PageNumChangedArgs(int numofpages)
        {
            this.numofpages = numofpages;
        }
    }
}