using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.PageModel
{
    public class FolderModel : ModelBase
    {
        private AppFolder _folder;
        private bool _editable;
        private string? _name;
        private List<FinalTiles>? _folderApps;
        private string? _description;

        private List<FinalTiles>? _appToAdd;
        public FolderModel() { }
        public FolderModel(AppFolder folder)
        {
            _folder = folder;
            _name = folder.Name;
            _folderApps = folder.FolderApps;
            _description = folder.Description;
        }
        public AppFolder Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                SetProperty(ref _folder, value);
            }
        }
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name) || (string.IsNullOrWhiteSpace(_name)))
               
                return (string.IsNullOrEmpty(_name) ||(string.IsNullOrWhiteSpace(_name)))?"No Folder Name":_name;
            }
            set
            {
               SetProperty(ref _name, value);
            }
        }
        public List<FinalTiles> FolderApps
        {
            get
            {               
              return  _folderApps ?? new List<FinalTiles>();
            }
            set
            {
                SetProperty(ref _folderApps, value);
            }
        }
        public string Description
        {
            get
            {
                    return (string.IsNullOrEmpty(_description) || (string.IsNullOrWhiteSpace(_description)))?"No Folder Description":_description;
            }
            set
            {
                SetProperty(ref _description, value);
            }
        } 
        public bool IsEditable
        {
            get

            {
                return _editable;
            }
            set
            {
                SetProperty(ref _editable, value);
            }
        }
        public List<FinalTiles> AppsToAdd;
        {
            get
            {
               return _appToAdd ?? new List<FinalTiles>();
            }
            set
            {
                SetProperty(ref _appToAdd, value);
            }
        }
    }
}
