using appLauncher.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appLauncher.Core.Model
{
    public class NavButton 
    {
        private IApporFolder _appNav = null;
        private string _tip=null;
        private string _glyph=null;
        private string _text=null;
        private string _tag= null;  
        public NavButton() { }
        public IApporFolder AppNav
        {
            get 
            { 
                return _appNav;
            }
            set 
            {
                _appNav = value;
            }
        }
        public string Tip
        {
            get
            {
                return (string.IsNullOrEmpty(_tip) || string.IsNullOrWhiteSpace(_tip))? "No Tip" : _tip;   
            }
            set
            {
                _tip = value;
            }
        }
        public string Glyph
        {
            get
            {
                return (string.IsNullOrEmpty(_glyph) || string.IsNullOrWhiteSpace(_glyph)) ? "&#E783;" : _glyph;
            }
            set
            {
                _glyph = value;
            }
        }
        public string Text
        {
            get
            {
                return (string.IsNullOrEmpty(_text) || string.IsNullOrWhiteSpace(_text)) ? "No Text" : _text;
            }
            set
            {
                _text = value;
            }
        }
        public string Tag
        {
            get
            {
                return (string.IsNullOrEmpty(_tag) || string.IsNullOrWhiteSpace(_tag)) ? "No Tag" : _tag;
            }
            set
            {
                _tag = value;
            }
        }
    }
}
