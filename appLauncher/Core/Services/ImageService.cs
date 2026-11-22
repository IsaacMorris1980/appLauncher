using appLauncher.Core.Interfaces;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly ConcurrentDictionary<string, BitmapImage> _imageCache = new ConcurrentDictionary<string, BitmapImage>();
        private readonly SemaphoreSlim _defaultSemaphore = new SemaphoreSlim(4); // Fallback throttle if none passed

        /// <summary>
        /// Loads and caches app icons/logos with optional down-sampling and shared concurrency throttling.
        /// </summary>
        public async Task<BitmapImage> LoadLogoAsync(
            RandomAccessStreamReference streamRef,
            string cacheKey = null,
            int? targetDecodeWidth = null,
            SemaphoreSlim externalSemaphore = null)
        {
            if (streamRef == null) return null;

            string key = targetDecodeWidth.HasValue ? $"{cacheKey}_{targetDecodeWidth}px" : cacheKey;

            if (!string.IsNullOrEmpty(key) && _imageCache.TryGetValue(key, out var cached))
                return cached;

            var semaphore = externalSemaphore ?? _defaultSemaphore;

            await semaphore.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(key) && _imageCache.TryGetValue(key, out cached))
                    return cached;

                var bitmapImage = new BitmapImage();

                if (targetDecodeWidth.HasValue)
                {
                    bitmapImage.DecodePixelWidth = targetDecodeWidth.Value;
                }

                using (var stream = await streamRef.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }

                if (!string.IsNullOrEmpty(key))
                {
                    _imageCache[key] = bitmapImage;
                }

                return bitmapImage;
            }
            catch
            {
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Loads, decodes, and caches full-size background or wallpaper images with concurrency protection.
        /// </summary>
        public async Task<BitmapImage> LoadBackgroundImageAsync(
            StorageFile file,
            string cacheKey = null,
            SemaphoreSlim externalSemaphore = null)
        {
            if (file == null) return null;

            string key = cacheKey ?? $"bg_{file.Path}";

            if (!string.IsNullOrEmpty(key) && _imageCache.TryGetValue(key, out var cached))
                return cached;

            var semaphore = externalSemaphore ?? _defaultSemaphore;

            await semaphore.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(key) && _imageCache.TryGetValue(key, out cached))
                    return cached;

                var bitmapImage = new BitmapImage();

                // Open full read stream for high-res background images without a small decode width constraint
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(stream);
                }

                if (!string.IsNullOrEmpty(key))
                {
                    _imageCache[key] = bitmapImage;
                }

                return bitmapImage;
            }
            catch
            {
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
