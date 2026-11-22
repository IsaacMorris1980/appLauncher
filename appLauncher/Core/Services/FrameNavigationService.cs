using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.Services
{
    public class FrameNavigationService : INavigationService
    {
        private  Frame _frame;
        public FrameNavigationService()
        { }
        public void Initialize(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public bool CanGoBack => _frame.CanGoBack;

        public void GoBack()
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        public void NavigateTo(Type pageType, object parameter = null)
        {
            _frame.Navigate(pageType, parameter);
        }
    }
}
