using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
namespace appLauncher.Core.Services
{
    /// <summary>
    /// Provides services related to the currently logged-in user's information.
    /// This class encapsulates the logic to retrieve user details and makes them accessible.
    /// It implements the <see cref="IUserService"/> interface.
    /// </summary>
    public class UserService : IUserService // Implements the new interface
    {
        private UserInfo _currentUser;
        /// <summary>
        /// Gets the currently logged-in user's information.
        /// This property will be populated after calling <see cref="InitializeUserAsync"/>.
        /// </summary>
        public UserInfo CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value; // Private setter as it's set internally
        }
        /// <summary>
        /// Initializes the UserService by attempting to retrieve the currently logged-in user's information.
        /// This method requires the 'UserAccountInformation' capability in your Package.appxmanifest.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InitializeUserAsync()
        {
            // Get all users on the device.
            // This requires the 'UserAccountInformation' capability in Package.appxmanifest.
            IReadOnlyList<User> users = await User.FindAllAsync();
            // Try to find the authenticated "local" or "active" user.
            // For SDK 10.0.15063.0, use UserAuthenticationStatus.LocallyAuthenticated.
            // UserType.LocalUser is the primary type for local accounts. UserType.Remote might also apply.
            User loggedInUser = users.FirstOrDefault(user =>
               user.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
               (user.Type == UserType.LocalUser || user.Type == UserType.RemoteUser));
            if (loggedInUser != null)
            {
                var userInfo = new UserInfo();
                // Get user's first name
                string firstName = (string)await loggedInUser.GetPropertyAsync(KnownUserProperties.FirstName);
                if (!string.IsNullOrEmpty(firstName))
                {
                    userInfo.FirstName = firstName;
                }
                // Get user's last name
                string lastName = (string)await loggedInUser.GetPropertyAsync(KnownUserProperties.LastName);
                if (!string.IsNullOrEmpty(lastName))
                {
                    userInfo.LastName = lastName;
                }
                // Get user's display name (often combines first and last, or a default)
                string displayName = (string)await loggedInUser.GetPropertyAsync(KnownUserProperties.DisplayName);
                if (!string.IsNullOrEmpty(displayName))
                {
                    userInfo.DisplayName = displayName;
                }
                // Get the user's ID (unique identifier for the user on this device)
                userInfo.Id = loggedInUser.NonRoamableId;
                CurrentUser = userInfo; // Set the public property
            }
            else
            {
                CurrentUser = null; // No user found or authenticated
            }
        }
    }
}
