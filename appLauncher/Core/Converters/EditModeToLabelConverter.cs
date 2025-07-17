using System;

using Windows.UI.Xaml.Controls; // Required for SymbolIcon
using Windows.UI.Xaml.Data;    // Required for IValueConverter

namespace appLauncher.Core.Converters
{
    /// <summary>
    /// Converts a boolean value (representing an edit mode) to an appropriate SymbolIcon.
    /// Returns a Save icon if in edit mode (true), otherwise returns an Edit icon.
    /// </summary>
    public class EditModeToLabelConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a SymbolIcon.
        /// </summary>
        /// <param name="value">The boolean value (expected to be 'bool').</param>
        /// <param name="targetType">The type of the target property (expected to be 'IconElement').</param>
        /// <param name="parameter">An optional parameter (not used in this converter).</param>
        /// <param name="language">The language to use for conversion (not used in this converter).</param>
        /// <returns>A string 'Save' if true, or 'Edit' if false.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isEditMode)
            {
                return isEditMode ? "Save" : "Edit";
            }
            // Return a Edit if the value is not a boolean or is null
            return "Edit";
        }
        /// <summary>
        /// Not implemented for one-way binding.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("EditModeToIconConverter can only be used for one-way binding.");
        }
    }
}
