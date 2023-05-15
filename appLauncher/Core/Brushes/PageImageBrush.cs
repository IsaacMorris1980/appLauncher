using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Brushes
{
    public class PageImageBrush : XamlCompositionBrushBase
    {
        public PageImageBrush(IRandomAccessStream stream)
        {
            this.backimage = stream;

        }
        private IRandomAccessStream backimage;
        private CompositionSurfaceBrush backimageBrush;
        private SpriteVisual spriteVisual;
        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {


                // Get a reference to the Compositor
                Compositor compositor = Window.Current.Compositor;

                // Use LoadedImageSurface API to get ICompositionSurface from image uri provided

                backimageBrush = compositor.CreateSurfaceBrush();
                spriteVisual = compositor.CreateSpriteVisual();

                LoadedImageSurface loadedSurface = LoadedImageSurface.StartLoadFromStream(backimage);
                backimageBrush.Surface = loadedSurface;
                spriteVisual.Brush = backimageBrush;
                backimageBrush.Stretch = CompositionStretch.UniformToFill;
                CompositionBrush = backimageBrush;
            }
        }

        protected override void OnDisconnected()
        {
            // Dispose Surface and CompositionBrushes if XamlCompBrushBase is removed from tree
            backimageBrush?.Dispose();
            backimageBrush = null;

            CompositionBrush?.Dispose();
            CompositionBrush = null;
        }
    }
}
