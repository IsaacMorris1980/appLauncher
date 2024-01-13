using appLauncher.Core.Brushes;
using appLauncher.Core.Interfaces;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class FinalTiles : ModelBase, IApporFolder
    {
        private const string _notRetrieved = "{0} was not retrieved";
        private AppListEntry _entry;
        private Package _pack;
        private string _fullName;
        public int _listPos;
        private byte[] _logo = new byte[1];
        private string _logoColor;
        private string _backColor;
        private string _textColor;
        private string _tip;
        private bool _inFolder;
        private bool _favorite;
        private int _launcedcount;
        private string _folderName = string.Empty;
        [JsonIgnore]
        public Package Pack
        {
            get
            {
                return _pack;
            }
            set
            {
                _pack = value;
                _fullName = value.Id.FullName;
            }
        }
        [JsonIgnore]
        public AppListEntry Entry
        {
            get
            {
                return _entry;
            }
            set
            {
                _entry = value;
            }
        }
        [JsonIgnore]
        public string Name
        {
            get
            {
                if (_pack == null)
                {
                    return string.Format(_notRetrieved, "Package Name");
                }
                return _pack.DisplayName;
            }
            set
            {

            }

        }
        [JsonProperty]
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
            }

        }
        [JsonIgnore]
        public string Description
        {
            get
            {
                if (_pack == null)
                {
                    return string.Format(_notRetrieved, "Package Description");
                }
                return _pack.Description;
            }
            set
            {

            }

        }
        [JsonIgnore]
        public string Developer
        {
            get
            {
                if (_pack == null)
                {
                    return string.Format(_notRetrieved, "App Developer");
                }
                return _pack.PublisherDisplayName;
            }
        }
        [JsonIgnore]
        public DateTimeOffset InstalledDate
        {
            get
            {
                if (_pack == null)
                {
                    return DateTimeOffset.FromUnixTimeSeconds(0);
                }
                return _pack.InstalledDate;
            }
        }
        [JsonProperty]
        public int ListPos
        {
            get
            {
                return _listPos;
            }
            set
            {
                SetProperty(ref _listPos, value);
            }
        }
        [JsonIgnore]
        public byte[] Logo
        {
            get
            {
                return _logo;
            }
            set
            {
                _logo = value;
            }
        }
        public async Task SetLogo()
        {

            try
            {

                try
                {
                    RandomAccessStreamReference logoStream = _entry.DisplayInfo.GetLogo(new Size(50, 50));
                    IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                    byte[] temp = new byte[whatIWant.Size];
                    using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                    {
                        await read.LoadAsync((uint)whatIWant.Size);
                        read.ReadBytes(temp);
                    }
                    Logo = temp;
                }
                catch (Exception es)
                {
                    Logo = new byte[1];
                }

            }
            catch (Exception es)
            {
                Logo = new byte[1];

            }
        }
        [JsonProperty]
        public Color LogoColor
        {
            get
            {
                if (string.IsNullOrEmpty(_logoColor))
                {
                    return "Blue".ToColor();
                }
                return _logoColor.ToColor();
            }
            set
            {
                SetProperty(ref _logoColor, value.ToString(), "LogoBrush");
            }
        }
        [JsonProperty]
        public Color BackColor
        {
            get
            {
                if (string.IsNullOrEmpty(_backColor))
                {
                    return "Transparent".ToColor();
                }
                return _backColor.ToColor();
            }
            set
            {
                SetProperty(ref _backColor, value.ToString(), "BackBrush");
            }
        }
        [JsonProperty]
        public Color TextColor
        {
            get
            {
                if (string.IsNullOrEmpty(_textColor))
                {
                    return "Red".ToColor();
                }
                return _textColor.ToColor();
            }
            set
            {
                SetProperty(ref _textColor, value.ToString(), "TextBrush");
            }
        }
        [JsonProperty]
        public string Tip
        {
            get
            {
                if (string.IsNullOrEmpty(_tip))
                {
                    return string.Format(_notRetrieved, "App Tool Tip");
                }
                return _tip;
            }
            set
            {
                SetProperty(ref _tip, value.ToString());
            }
        }
        [JsonIgnore]
        public MaskedBrush LogoBrush
        {
            get
            {
                return new MaskedBrush(Logo.AsBuffer().AsStream().AsRandomAccessStream(), LogoColor);
            }
        }
        [JsonIgnore]
        public SolidColorBrush BackBrush
        {
            get
            {
                return new SolidColorBrush(BackColor);
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
        [JsonProperty]
        public bool InFolder
        {
            get
            {
                return _inFolder;
            }
            set
            {
                _inFolder = value;
            }
        }
        [JsonProperty]
        public string FolderName
        {
            get
            {
                return _folderName;
            }
            set
            {
                _folderName = value;
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
        public int LaunchedCount
        {
            get
            {
                return _launcedcount;
            }
            set
            {
                SetProperty(ref _launcedcount, value);
            }
        }


    }

}
