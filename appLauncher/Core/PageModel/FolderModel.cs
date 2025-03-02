using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.PageModel
{
    public class FolderModel : ModelBase
    {
        private AppFolder _folder;
      
        private bool _editable;
        public FolderModel() { }
        public FolderModel(AppFolder folder)
        {
            _folder = folder;
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
        
    }
}
