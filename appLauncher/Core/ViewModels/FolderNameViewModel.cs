using appLauncher.Core.Interfaces;
using appLauncher.Core.Services; // For ILoggingService

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace appLauncher.Core.ViewModels
{
    /// <summary>
    /// ViewModel for the FolderNamePage ContentDialog, managing the folder name input.
    /// </summary>
    public class FolderNameViewModel : ViewModelBase // Inherit from ViewModelBase
    {
        private readonly ILoggingService _loggingService;

        private string _folderNameInput;
        /// <summary>
        /// Gets or sets the name entered by the user for the new folder.
        /// </summary>
        public string FolderNameInput
        {
            get => _folderNameInput;
            set
            {
                var oldfoldername = _folderNameInput;
                FolderNameInput = value;
                if (IsFolderNameValid)
                {
                    SetProperty(ref _folderNameInput, value);
                }
                else
                {
                    FolderNameInput = oldfoldername;
                }
               
            }
        }

        // You could add commands here for Primary/Secondary buttons if you need
        // more complex logic before the dialog closes, but for simple input
        // ContentDialog's built-in PrimaryButtonCommand/SecondaryButtonCommand
        // combined with direct binding to FolderNameInput is often sufficient.

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderNameViewModel"/> class.
        /// </summary>
        /// <param name="loggingService">The service for logging exceptions.</param>
        public FolderNameViewModel(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        // Example of simple validation that could be exposed
        public bool IsFolderNameValid => !string.IsNullOrWhiteSpace(FolderNameInput);
    }
}
