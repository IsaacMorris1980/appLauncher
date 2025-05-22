using System.Diagnostics;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class PageIndicators : ModelBase
    {
        private int _pageNumb;
        private bool _selected;
        private string _tip = "Unknown Page Selected";
        public PageIndicators()
        {

        }
        public PageIndicators(int pageNum)
        {
            _pageNumb = pageNum;
            _selected = false;
            _tip = string.Format("{0} Page Selected", (pageNum+1));  
        }
        public PageIndicators(int pageNum,bool selected, string tip)
        {
            _pageNumb = pageNum;
            _selected = selected;
            _tip = tip;
        }
        public PageIndicators(int pageNum, bool selected)
        {
            _pageNumb = pageNum;
            _selected = selected;
        }
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                SetProperty(ref _selected, value);
            }
        }
        public string Tip
        {
            get
            {
                return ((string.IsNullOrEmpty(_tip)||string.IsNullOrWhiteSpace(_tip))?string.Format("{0} Page Selected","Unknown"): string.Format("{0} Page Selected", DisplayPageNum));
            }
            set
            {
                SetProperty(ref _tip, value);
            }

        }
        public Brush FillColor

        {
            get
            {
               return (Selected==true)?new SolidColorBrush(Colors.Orange):new SolidColorBrush(Colors.Gray);
            }
        }
        public int PageNum
        {
            get
            {
                return (_pageNumb >= 0) ? _pageNumb : 0;
            }              
            set
            {
                SetProperty(ref _pageNumb, value);
            }
        }
        public int DisplayPageNum
        {
            get
            {
                return ((PageNum + 1)>=1)?(PageNum+1):1;
            }
        }
    }

}
