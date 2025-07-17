using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using appLauncher.Core.Model;


using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;


namespace appLauncher.Core.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Gets the currently logged-in user's information.
        /// This property will be populated after calling <see cref="InitializeUserAsync"/>.
        /// </summary>
        UserInfo CurrentUser { get; } // Read-only property in the interface

        /// <summary>
        /// Initializes the user service by attempting to retrieve the currently logged-in user's information.
        /// This method requires the 'UserAccountInformation' capability in your Package.appxmanifest.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task InitializeUserAsync();
    } 
}
