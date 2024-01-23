using System;

using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Brushes
{
    public class PageImageBrush : XamlCompositionBrushBase
    {
        public PageImageBrush()
        {
        }

        public PageImageBrush(IRandomAccessStream stream)
        {
            this._backImage = stream;

        }
        public IRandomAccessStream _backImage;
        private CompositionSurfaceBrush _backImageBrush;
        private SpriteVisual spriteVisual;
        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {


                // Get a reference to the Compositor
                Compositor compositor = Window.Current.Compositor;

                // Use LoadedImageSurface API to get ICompositionSurface from image uri provided

                _backImageBrush = compositor.CreateSurfaceBrush();
                spriteVisual = compositor.CreateSpriteVisual();

                LoadedImageSurface loadedSurface = LoadedImageSurface.StartLoadFromStream(_backImage);
                _backImageBrush.Surface = loadedSurface;
                spriteVisual.Brush = _backImageBrush;
                _backImageBrush.Stretch = CompositionStretch.UniformToFill;
                CompositionBrush = _backImageBrush;
            }
        }

        protected override void OnDisconnected()
        {
            // Dispose Surface and CompositionBrushes if XamlCompBrushBase is removed from tree
            _backImageBrush?.Dispose();
            _backImageBrush = null;

            CompositionBrush?.Dispose();
            CompositionBrush = null;
            GC.WaitForPendingFinalizers();
        }
    }
}
