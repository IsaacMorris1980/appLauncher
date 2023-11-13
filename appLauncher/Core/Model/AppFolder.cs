using appLauncher.Core.Interfaces;

using Newtonsoft.Json;

using System.Collections.ObjectModel;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppFolder : ModelBase, IApporFolder
    {
        private string _name;
        private string _description;
        private ObservableCollection<FinalTiles> _folderapps = new ObservableCollection<FinalTiles>();
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
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return string.Empty;
                }
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
            }
        }
        [JsonProperty]
        public ObservableCollection<FinalTiles> FolderApps
        {
            get
            {
                return _folderapps;
            }
            set
            {
                SetProperty(ref _folderapps, value);
            }
        }
        [JsonProperty]
        public int ListPos { get; set; }
        [JsonIgnore]
        public SolidColorBrush TextBrush
        {
            get
            {
                return new SolidColorBrush(TextColor);
            }
        }
        [JsonIgnore]
        public SolidColorBrush BackBrush { get { return new SolidColorBrush(BackColor); } }
        [JsonProperty]
        public Color TextColor { get; set; } = Colors.Red;
        [JsonProperty]
        public Color BackColor { get; set; } = Colors.Blue;
    }
}
