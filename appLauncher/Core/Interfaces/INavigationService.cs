using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Interfaces
{
    public interface INavigationService
    {
        bool CanGoBack { get; }
        void GoBack();
        void NavigateTo(Type pageType, object parameter = null);
    }
}
