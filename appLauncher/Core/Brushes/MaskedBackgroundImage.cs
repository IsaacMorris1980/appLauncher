using System;

using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Brushes
{
    public class MaskedBackgroundImage : XamlCompositionBrushBase
    {
        public MaskedBackgroundImage(String images, Color overlaycolor)
        {
            this.logo = images;
            base.FallbackColor = Colors.Transparent;
            this.overlaycolor = overlaycolor;
        }
        public MaskedBackgroundImage() { }
        private string logo;
        private CompositionMaskBrush _maskedbrush;
        private Color overlaycolor { get; set; }
        protected override void OnConnected()
        {
            // Get a reference to the Compositor
            Compositor compositor = Window.Current.Compositor;
            CompositionColorBrush colorbrush;
            // Use LoadedImageSurface API to get ICompositionSurface from image uri provided
            colorbrush = compositor.CreateColorBrush(overlaycolor);
            _maskedbrush = compositor.CreateMaskBrush();
            _maskedbrush.Source = colorbrush;

            LoadedImageSurface loadedSurface = LoadedImageSurface.StartLoadFromUri(new Uri(logo));// StartLoadFromStream(logo);
            _maskedbrush.Mask = compositor.CreateSurfaceBrush(loadedSurface);
            CompositionBrush = _maskedbrush;

        }

        protected override void OnDisconnected()
        {
            // Dispose Surface and CompositionBrushes if XamlCompBrushBase is removed from tree
            _maskedbrush?.Dispose();
            _maskedbrush = null;

            CompositionBrush?.Dispose();
            CompositionBrush = null;
        }
    }
}
