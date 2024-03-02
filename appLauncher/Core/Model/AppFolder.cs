using appLauncher.Core.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
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
        private bool _favorite = false;
        private long _createdDate = 0;
        private int _launchedcount = 0;
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
        public bool Favorite
        {
            get
            {
                return _favorite;
            }
            set
            {
                SetProperty(ref _favorite, value);
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
        public List<FinalTiles> FolderApps
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
        [JsonProperty]
        public int LaunchedCount
        {
            get
            {
                return _launchedcount;
            }
            set
            {
                SetProperty(ref _launchedcount, value);
            }
        }
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
        [JsonProperty]
        public DateTimeOffset InstalledDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_createdDate);
            }
            set
            {
                SetProperty(ref _createdDate, value.ToUnixTimeSeconds());
            }
        }
    }
}
