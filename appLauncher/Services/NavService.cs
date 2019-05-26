using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace appLauncher.Services
{
    public class NavService
    {
        private static Frame _frame = null;
        public static NavService Instance = new NavService();
        public static void CreateInstance(Frame frame)
        {
            _frame = frame;

        }

        public void GoBack()
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        public void GoForward()
        {
            if (_frame.CanGoForward)
            {
                _frame.GoForward();
            }
        }


        public bool Navigate(Type sourcePageType)
        {
            return _frame.Navigate(sourcePageType);
        }

        public bool Navigate(Type sourcePageType, object parameter)
        {
            return _frame.Navigate(sourcePageType, parameter);
        }

        public bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            return _frame.Navigate(sourcePageType, parameter, infoOverride);
        }

        public bool IsCurrentPageOfType(Type typeQuery)
        {
            return _frame.SourcePageType.Equals(typeQuery);
        }
    }
}
