using Newtonsoft.Json;

using System;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IApporFolder
    {
        [JsonProperty] string Name { get; set; }
        [JsonProperty] string FullName { get; set; }
        [JsonProperty] string Developer { get; set; }
        [JsonProperty] DateTimeOffset InstallDate { get; set; }
        [JsonProperty] int ListPos { get; set; }
        [JsonProperty] int FolderListPos { get; set; }
        [JsonProperty] bool Favorite { get; set; }
        [JsonProperty] string BackColor { get; set; }
        [JsonProperty] string LogoColor { get; set; }
        [JsonProperty] string TextColor { get; set; }

        /// <summary>
        /// Unique identifier for reconciliation and layout matching (App PFN or Folder ID).
        /// </summary>
        string UniqueId { get; }
    }
}
