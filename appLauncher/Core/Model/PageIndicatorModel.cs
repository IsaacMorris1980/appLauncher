using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class PageIndicatorModel: ModelBase
    {
        private int _pageIndex;
        public int PageIndex
        {
            get => _pageIndex;
            set => SetProperty(ref _pageIndex, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    OnPropertyChanged(nameof(FillColor));
                    OnPropertyChanged(nameof(IndicatorBrush));
                }
            }
        }

        // Supports your existing tooltip property structure
        public string Tip => $"Page {PageIndex + 1}";

        // Active page uses an accent color (e.g., Orange), inactive pages are muted gray
        public Brush FillColor => IsSelected
            ? new SolidColorBrush(Colors.Orange)
            : new SolidColorBrush(Colors.Gray);

        public Brush IndicatorBrush => FillColor;

        public ICommand SelectPageCommand { get; }

        public PageIndicatorModel(int pageIndex, ICommand selectPageCommand)
        {
            PageIndex = pageIndex;
            SelectPageCommand = selectPageCommand;
        }
    }
}
