using appLauncher.Core.Brushes;
using appLauncher.Core.Interfaces;

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
        public int _folderListPos;
        private byte[] _logo;
        private string _logoColor;
        private string _backColor;
        private string _textColor;
        private string _tip;
        private bool _inFolder;
        private bool _favorite;
        private int _launcedcount;
        private List<string> _folderName = new List<string>();
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
                return (_pack == null)?string.Format(_notRetrieved, "Package Name"):_pack.DisplayName;
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
                return (_pack == null)?string.Format(_notRetrieved, "Package Description") : _pack.Description;             
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
                return (_pack == null) ? string.Format(_notRetrieved, "App Developer") : _pack.PublisherDisplayName;                
            }
        }
        [JsonIgnore]
        public DateTimeOffset InstalledDate
        {
            get
            {
                return (_pack==null)?DateTimeOffset.FromUnixTimeSeconds(0):_pack.InstalledDate;               
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
        [JsonProperty]
        public int FolderListPos
        {
            get
            {
                return (_folderListPos<=0)?0:_folderListPos;
            }
            set
            {
                SetProperty(ref _folderListPos, value);
            }
        }
        [JsonIgnore]
        public byte[] Logo
        {
            get
            {
                return (_logo==null)?new byte[0]:_logo;
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
        [JsonProperty]
        public Color LogoColor
        {
            get
            {
                return (string.IsNullOrEmpty(_logoColor)||string.IsNullOrWhiteSpace(_logoColor))?"Blue".ToColor():_logoColor.ToColor();
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
                return (string.IsNullOrEmpty(_backColor)||string.IsNullOrWhiteSpace(_backColor))? "Transparent".ToColor():_backColor.ToColor();
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
               return (string.IsNullOrEmpty(_textColor) || string.IsNullOrWhiteSpace(_textColor))?"Red".ToColor():_textColor.ToColor();
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
                return (string.IsNullOrEmpty(_tip) || string.IsNullOrWhiteSpace(_tip))?string.Format(_notRetrieved, "App Tool Tip"):_tip;
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
        public List<string> FolderName
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
                return (_launcedcount<=0)?0:_launcedcount;
            }
            set
            {
                SetProperty(ref _launcedcount, value);
            }
        }
        public async Task<bool> Launch()
        {
            return await Entry.LaunchAsync();
        }
        public string  GetVersion
        {
            get
            {
               return (_pack == null)?string.Format(_notRetrieved, "App Version"):string.Format("Installed App Version: {0}.{1}.{2}.{3}", _pack.Id.Version.Major, _pack.Id.Version.Minor, _pack.Id.Version.Build,_pack.Id.Version.Revision);
            }
        }


    }

}
