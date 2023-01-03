using Microsoft.Toolkit.Uwp.Helpers;

using System.Collections.Generic;

using UwpAppLauncher.Interfaces;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UwpAppLauncher.Model
{
    public sealed class FolderTile : ModelBase, IApporFolder
    {
        public FolderTile()
        {

        }
        private int _listlocation;
        private string _name;
        private string _folderTextColor = "Red";
        private string _folderBackColor = "Blue";
        private string _folderFrontColor = "Transparent";
        private List<AppTile> _tiles = new List<AppTile>();

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return "New Folder";
                }
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public int Listlocation
        {
            get
            {
                if (_listlocation > -1)
                {
                    return _listlocation;
                }
                return -1;
            }
            set
            {
                SetProperty(ref _listlocation, value);
            }
        }
        public string PartOfFolderName
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return "New Folder";
                }
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public List<AppTile> FolderApps
        {
            get { return _tiles; }
            set { _tiles = value; }
        }
        public Color TextColor
        {
            get { return _folderTextColor.ToColor(); }
            set { SetProperty(ref _folderTextColor, value.ToString()); }

        }
        public Color FrontColor
        {
            get { return _folderFrontColor.ToColor(); }
            set { SetProperty(ref _folderFrontColor, value.ToString()); }
        }
        public Color BackColor
        {
            get { return _folderBackColor.ToColor(); }
            set
            {
                SetProperty(ref _folderBackColor, value.ToString());
            }
        }
        public SolidColorBrush TextColorBrush
        {
            get
            {
                return new SolidColorBrush(TextColor);
            }
        }
        public SolidColorBrush BackColorBrush
        {
            get { return new SolidColorBrush(BackColor); }
        }


    }
}
