using appLauncher.Core.Interfaces;

using Newtonsoft.Json;

using System.Collections.Generic;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppFolder : ModelBase, IApporFolder
    {
        private string _name;
        private string _description;
        private List<FinalTiles> _folderapps = new List<FinalTiles>();
        private Color _textcolor = Colors.Orange;
        private Color _backcolor = Colors.Black;
        [JsonProperty]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return string.Empty;
                }
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        [JsonProperty]
        public string Description { get; set; }
        [JsonProperty]
        public List<FinalTiles> FolderApps { get; set; }
        [JsonProperty]
        public int ListPos { get; set; }
        [JsonIgnore]
        public SolidColorBrush TextBrush { get; }
        [JsonIgnore]
        public SolidColorBrush BackBrush { get; }
        [JsonProperty]
        public Color TextColor { get; set; }
        [JsonProperty]
        public Color BackColor { get; set; }
    }
}
