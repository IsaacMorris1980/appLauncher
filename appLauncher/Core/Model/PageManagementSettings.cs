using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
    public class PageManagementSettings : ModelBase
    {
        private int _appsPerScreen = 0;
        private int _lastPageNum = 0;
        private int _numofPages = 1;

        public int AppsPerPage
        {
            get { return _appsPerScreen; }
            set { SetProperty(ref _appsPerScreen, value); }
        }

        public int LastPageNumber
        {
            get { return _lastPageNum; }
            set { SetProperty(ref _lastPageNum, value); }
        }

        public int NumOfPages
        {
            get { return _numofPages; }
            set { SetProperty(ref _numofPages, value); }
        }
    }
}