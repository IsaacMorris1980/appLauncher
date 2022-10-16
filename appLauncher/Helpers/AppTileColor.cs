
using Newtonsoft.Json;

using Windows.UI;

namespace appLauncher.Helpers
{
    public static class AppTileColor
    {
        [JsonProperty]
        public static Color foregroundColor { get; set; } = Colors.Blue;
        [JsonProperty]
        public static Color backgroundColor { get; set; } = Colors.Orange;
        [JsonProperty]
        public static double foregroundOpacity { get; set; } = 1;
        [JsonProperty]
        public static double backgroundOpacity { get; set; } = 1;
    }
}
