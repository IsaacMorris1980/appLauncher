using System;

namespace appLauncher.Core.CustomEvent
{
    public delegate void AppPageSizeChangedDelegate(AppPageSizeChangedEventArgs e);
    public class AppPageSizeChangedEventArgs : EventArgs
    {
        private int _appPageSize;
        public AppPageSizeChangedEventArgs(int appPageSize)
        {
            _appPageSize = appPageSize;
        }

        public int AppPageSize { get { return _appPageSize; } }
    }
}
