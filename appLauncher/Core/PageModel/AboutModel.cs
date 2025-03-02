using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.PageModel
{
    public class AboutModel : ModelBase
    {
        private string _title = "About appLauncher";
        private string _maintainersAndContributors = "Maintainers and Contributors";
        private string _appInfos = "App Github Links";
        private List<AppLinks> _collabortors;
        private AppLinks _issues;
        private AppLinks _latest;
        private AppLinks _previous;
        private string _version;
      
        public AboutModel()
        {
            _collabortors = new List<AppLinks>();
            SetCollaborators();
            CreateOtherLinks();
            SetVersion();
        }
        private void SetCollaborators()
        {
            _collabortors.Add(new AppLinks()
            {
                Name = "Original Maintainer:  Colin Kiama",
                Link = new Uri("https://github.com/colinkiama")
            });
            _collabortors.Add(new AppLinks()
            {
                Name = "Current Maintainer:  Isaac Morris",
                Link = new Uri("https://github.com/IsaacMorris1980")
            });
            _collabortors.Add(new AppLinks()
            {
                Name = "Contributor:  Bruno Alexander Cremonese de Morais",
                Link = new Uri("https://github.com/brucremo")
            });
        }
        public void CreateOtherLinks()
        {
            _issues = new AppLinks()
            {
                Name = "Issues",
                Link = new Uri("https://github.com/IsaacMorris1980/appLauncher/issues")
            };
            _latest = new AppLinks()
            {
                Name = "Latest Version",
                Link = new Uri("https://github.com/IsaacMorris1980/appLauncher/releases/latest")
            };
            _previous = new AppLinks()
            {
                Name = "Releases",
                Link = new Uri("https://github.com/IsaacMorris1980/appLauncher/releases/")
            };
        }
        private void SetVersion()
        {
            Package pack = Package.Current;
            PackageVersion version = new PackageVersion();
            version = pack.Id.Version;
            _version = string.Format("Installed App Version: {0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
        public string Title
        {
            get
            {
                return _title;
            }         
        }
        public List<AppLinks> Collaborators
        {
            get
            {
                return _collabortors;
            }       
        }
        public AppLinks Issues
        {
            get
            {
                return _issues;
            }          
        }
        public AppLinks Latest
        {
            get
            {
                return _latest;
            }
        }
        public AppLinks Previous
        {
            get
            {
                return _previous;
            }          
        }
   
        public string Versions
        {
            get
            {
                return _version;
            }
        }

        public string MaintanersAndCollaborators
        {
            get
            {
                return _maintainersAndContributors;
            }
        }

        public string AppInfos
        {
            get
            {
                return _appInfos;
            }
        }
    }
}
