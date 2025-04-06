using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
   public class AppLinks
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
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public Uri Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
            }
        }
    }
}
