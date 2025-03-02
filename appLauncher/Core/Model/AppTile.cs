using appLauncher.Core.Brushes;
using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class AppTile : ModelBase
    {
        private AppTileSettings _settings;
        private AppListEntry _entry;
        private Package _pack;
        private string _fullName;
        public int _appListPos;
        public int _folderAppListPos;
        private byte[] _appLogo;
        private string _apptip;
        private int _appLaunchedCount;

        public AppTile()
        {

        }
        public AppTile(AppTileSettings settings, AppListEntry entry, Package pack, string fullName, int appListPos, int folderAppListPos, byte[] appLogo, string apptip, int appLaunchedCount)
        {
            _settings = settings;
            _entry = entry;
            _pack = pack;
            _fullName = fullName;
            _appListPos = appListPos;
            _folderAppListPos = folderAppListPos;
            _appLogo = appLogo;
            _apptip = apptip;
            _appLaunchedCount = appLaunchedCount;            
        }
        public AppTileSettings Settings
        {
            get
            {
                try
                {
                    return _settings;
                }
                catch (Exception e)
                {
                   
                }

                return new AppTileSettings();
            }
            set
            {
                try
                {
                    SetProperty(ref _settings, value);
                }
                catch (Exception e)
                {
                   
                }
            }
        }
        public AppListEntry Entry
        {
            get
            {
                try
                {
                    return _entry;
                }
                catch (Exception e)
                {
                   throw e;
                }              
            }
            set
            {
                SetProperty(ref _entry, value);
            }
        }
        public Package Pack
        {
            get
            {
                try
                {
                    return _pack;
                }
                catch (NullReferenceException e)
                {
                    
                    throw e;
                }                
            }
            set
            {
                SetProperty(ref _pack, value);
            }
        }
        public string FullName
        {
            get
            {
                try
                {
                    return _fullName;
                }
                catch (Exception e)
                {
                    
                }
                return string.Empty;
            }
            set
            {
                SetProperty(ref _fullName, value);
            }
        }
        public int AppListPostion
        {
            get
            {
                try
                {
                    return _appListPos;
                }
                catch (Exception e)
                {
                  
                }
                return -1;
            }
            set
            {
                SetProperty(ref _appListPos, value);
            }
        }
        public int FolderAppListPostiion
        {
            get
            {
                try
                {
                    return _folderAppListPos;
                }
                catch (Exception e)
                {

                }
                return -1;
            }
            set
            {
                SetProperty(ref _folderAppListPos, value);
            }
        }
        public byte[] AppLogo
        {
            get
            {
                try
                {
                    return _appLogo;
                }
                catch (Exception e)
                { 
                   
                }
                return new byte[1];
            }
            set
            {
                SetProperty(ref _appLogo, value);
            }
        } 
        public string AppTip
        {
            get
            {
                try
                {
                    return _apptip;
                }
                catch (Exception e)
                {
                                   
                }
                return string.Empty;
            }
            set 
            {
                try
                {
                    SetProperty(ref _apptip, value);
                }
                catch (Exception e)
                {
                   
                }
            }
        }
        public int AppLaunchedCount
        {
            get
            {
                try
                {
                    return _appLaunchedCount;
                }
                catch (Exception e)
                {
                }
                return -1;
            }
            set 
            {
                try
                {
                    SetProperty(ref _appLaunchedCount, value);
                }
                catch (Exception e)
                {
                   
                }
            }
        }
        public SolidColorBrush TextBrush
        {
            get
            {
                return new SolidColorBrush() 
                {
                    Color = Settings.TextColor,
                    Opacity = Settings.TextOpacity
                };
            }
        }
        public SolidColorBrush BackgroundBrush
        {
            get
            {
                return new SolidColorBrush()
                {
                    Color  = Settings.BackgroundColor,
                    Opacity = Settings.BackgroundOpacity
                };
            }
        }
        public MaskedBrush LogoBrush => new MaskedBrush()
        {
            //_logo = AppLogo.AsBuffer().AsStream().AsRandomAccessStream(),
            //overlayColor = Settings.LogoColor,
            //Opacity = Settings.LogoOpacity
        };
    }
}
