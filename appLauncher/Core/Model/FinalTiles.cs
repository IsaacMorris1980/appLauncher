using appLauncher.Core.Brushes;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Services;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FinalTiles : IApporFolder
    {
        // --- Layout & Persistence Properties (Saved to JSON) ---
        [JsonProperty] public string Name { get; set; }
        [JsonProperty] public int ListPos { get; set; }
        [JsonProperty] public string Developer { get; set; }
        [JsonProperty] public DateTimeOffset InstallDate { get; set; }
        [JsonProperty] public int FolderListPos { get; set; }
        [JsonProperty] public bool Favorite { get; set; }
        [JsonProperty] public string BackColor { get; set; }
        [JsonProperty] public string LogoColor { get; set; }
        [JsonProperty] public string TextColor { get; set; }
        [JsonProperty] public string FullName { get; set; } // Used for JSON reconciliation matching

        // --- Package Health Properties (Saved & Evaluated) ---
        [JsonProperty] public bool IsLaunchable { get; set; } = true;
        [JsonProperty] public string StatusDescription { get; set; } = string.Empty;
        [JsonProperty] public int LaunchedCount { get; set; } = 0;
        

        // Interface Implementation
        public string UniqueId => FullName;

        // --- Runtime-Only Handles (Ignored by JSON Serialization) ---
        [JsonIgnore] public Package Pack { get; set; }
        [JsonIgnore] public AppListEntry Entry { get; set; }
        [JsonIgnore] public BitmapImage AppLogo { get; set; }
        [JsonIgnore] public SolidColorBrush BackBrush { get; set; }
        [JsonIgnore] public MaskedBrush LogoBrush { get; set; }
        [JsonIgnore] public SolidColorBrush TextBrush { get; set; }
    }

}
