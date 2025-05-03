using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
   public class AppLinks:ModelBase
    {
        private string _name;
        private Uri _link;
        public AppLinks() { }
        public  AppLinks(string name)
        {
            _name = name;
            _link = null;
        }
        public AppLinks(string name,string link)
        {
            _name = name;
            _link = new Uri(link);
        }
        public string Name
        {
            get
            {
                return ((string.IsNullOrEmpty(_name)) || string.IsNullOrWhiteSpace(_name)) ? "No Link Name Set" : _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public Uri Link
        {
            get
            {
                return _link ?? new Uri("https://github.com/IsaacMorris1980/appLauncher");
            }
            set
            {
                SetProperty(ref _link, value);
            }
        }
    }
}
