using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace applauncher.mobile.Core.Brushes
{
  public class MaskedBrush : XamlCompositionBrushBase
    {
        public MaskedBrush(byte[] stream)
        {
            this.stream = stream.AsBuffer().AsStream().AsRandomAccessStream();
            base.FallbackColor = Colors.Transparent;

        }
        private IRandomAccessStream stream;
        private CompositionMaskBrush _maskedbrush;
        public Color overlaycolor { get; set; }
        protected override void OnConnected()
        {
            // Get a reference to the Compositor
            Compositor compositor = Window.Current.Compositor;
            CompositionColorBrush colorbrush;
            // Use LoadedImageSurface API to get ICompositionSurface from image uri provided
            colorbrush = compositor.CreateColorBrush((overlaycolor == null) ? this.FallbackColor : overlaycolor);
            _maskedbrush = compositor.CreateMaskBrush();
            _maskedbrush.Source = colorbrush;

            LoadedImageSurface loadedSurface = LoadedImageSurface.StartLoadFromStream(stream);
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

