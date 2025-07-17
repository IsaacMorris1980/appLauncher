using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Interfaces
{
    public interface IImageService
    {
        ObservableCollection<PageBackgrounds> BackgroundImages { get; }
        Brush CurrentBackgroundBrush { get; }

        event EventHandler ImagesRetrieved;

        Task SetNextBackgroundImage();
        void AddPageBackground(PageBackgrounds pageBackgrounds);
        void RemovePageBackground(string pageBackgroundDisplayName);
        Task LoadBackgroundImages();
        Task SaveImageOrder();
        Task<byte[]> ConvertImageFiletoByteArrayAsync(StorageFile fileName);
    }
}
