using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Interfaces
{
    public interface IImageService
    {
        Task<BitmapImage> LoadLogoAsync(
            RandomAccessStreamReference streamRef,
            string cacheKey = null,
            int? targetDecodeWidth = null,
            SemaphoreSlim externalSemaphore = null);

        Task<BitmapImage> LoadBackgroundImageAsync(
            StorageFile file,
            string cacheKey = null,
            SemaphoreSlim externalSemaphore = null);
    }
}
