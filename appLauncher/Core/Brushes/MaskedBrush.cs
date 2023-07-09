using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Brushes
{
    public class MaskedBrush : XamlCompositionBrushBase
    {
        public MaskedBrush(IRandomAccessStream stream, Color color)
        {
            this._logo = stream;
            base.FallbackColor = Colors.Transparent;
            this.overlayColor = color;
        }
        public MaskedBrush() { }
        private IRandomAccessStream _logo;
        private CompositionMaskBrush _maskedBrush;
        private Color overlayColor { get; set; }
        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {


                // Get a reference to the Compositor
                Compositor compositor = Window.Current.Compositor;
                CompositionColorBrush colorbrush;
                // Use LoadedImageSurface API to get ICompositionSurface from image uri provided
                colorbrush = compositor.CreateColorBrush(overlayColor);
                _maskedBrush = compositor.CreateMaskBrush();
                _maskedBrush.Source = colorbrush;

                LoadedImageSurface loadedSurface = LoadedImageSurface.StartLoadFromStream(_logo);
                _maskedBrush.Mask = compositor.CreateSurfaceBrush(loadedSurface);
                CompositionBrush = _maskedBrush;
            }
        }

        protected override void OnDisconnected()
        {
            // Dispose Surface and CompositionBrushes if XamlCompBrushBase is removed from tree
            _maskedBrush?.Dispose();
            _maskedBrush = null;

            CompositionBrush?.Dispose();
            CompositionBrush = null;
        }
    }
}
