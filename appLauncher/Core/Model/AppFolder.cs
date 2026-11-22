using appLauncher.Core.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AppFolder : IApporFolder
    {
        // --- Layout & Persistence Properties (Saved to JSON) ---
        [JsonProperty] public string Name { get; set; }
        [JsonIgnore] public string FullName { get; set; }
        [JsonProperty] public DateTimeOffset InstallDate { get; set; }
        [JsonProperty] public string Developer { get; set; }
        [JsonProperty] public int ListPos { get; set; }
        [JsonProperty] public int FolderListPos { get; set; }
        [JsonProperty] public bool Favorite { get; set; }
        [JsonProperty] public string BackColor { get; set; }
        [JsonProperty] public string LogoColor { get; set; }
        [JsonProperty] public string TextColor { get; set; }
        [JsonProperty] public string FolderId { get; set; }

        // Interface Implementation
        public string UniqueId => FolderId;
        [JsonIgnore] public SolidColorBrush BackBrush { get; set; }
        [JsonIgnore] public SolidColorBrush TextBrush { get; set; }
        // --- Nested Group Items (Saved to JSON) ---
        [JsonProperty] public List<FinalTiles> FolderApps { get; set; } = new List<FinalTiles>();
    }
}
