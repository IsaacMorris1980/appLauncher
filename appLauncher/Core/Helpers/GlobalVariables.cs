using appLauncher.Core.CustomEvent;

namespace appLauncher.Core.Helpers
{
    public static class GlobalVariables
    {
        private static int _pageNum;
        private static int _appsPerScreen;
        private static int _numOfPages;

        public static event PageChangedDelegate PageNumChanged;

        public static event PageSizeChangedDelegate NumofApps;
        public static event PageNumChangedDelegate NumofPagesChanged;


        public static void SetNumOfPages(int appNumOfPages)
        {
            _numOfPages = appNumOfPages;
            NumofPagesChanged?.Invoke(new PageNumChangedArgs(appNumOfPages));
        }

        public static void SetPageNumber(int appPageNum)
        {
            if (_pageNum != appPageNum)
            {
                _pageNum = appPageNum;
                PageNumChanged?.Invoke(new PageChangedEventArgs(appPageNum));
            }

        }
        public static void SetPageSize(int appNumPerScreen)
        {
            if (_appsPerScreen != appNumPerScreen)
            {
                _appsPerScreen = appNumPerScreen;
                NumofApps?.Invoke(new PageSizeEventArgs(appNumPerScreen));
            }


        }
    }
}
