using appLauncher.Core.Brushes;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Services;

using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.Networking.Sockets;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Web.Syndication;

namespace appLauncher.Core.Model
{
    public class AppTileSettings:ModelBase
    {
        private string _textColor;
        private string _backgroundColor;
        private string _logoColor;
        private bool _isFavorite;
        private double _textOpacity;
        private double _backgroundOpacity;
        private double _logoOpacity;
        public AppTileSettings()
        {

        }
        public AppTileSettings(string textColor, string backgroundColor, string logoColor, bool isFavorite, double textOpacity,double backOpacity,double logoOpacity)
        {
            _textColor = textColor;
            _backgroundColor = backgroundColor;
            _logoColor = logoColor;
            _isFavorite = isFavorite;
            _textOpacity = textOpacity;
            _backgroundOpacity = backOpacity;
            _logoOpacity = logoOpacity;
        }
        public Color TextColor
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_textColor)||!string.IsNullOrWhiteSpace(_textColor))
                    {
                        return _textColor.ToColor();
                    }
                
                }
                catch (Exception e)
                {
                  
                }
                return Colors.Red;
            }
            set
            {
                try
                {
                    SetProperty(ref _textColor, value.ToString());
                }
                catch (Exception e)
                {
                   
                }
            }
        }
        public Color BackgroundColor
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_backgroundColor)||!string.IsNullOrWhiteSpace(_backgroundColor))
                    {
                        return _backgroundColor.ToColor();
                    }
                }
                catch (Exception e)
                {
                   
                }
                return Colors.Black;
            }
            set
            {
                try
                {
                    SetProperty(ref _backgroundColor, value.ToString());
                }
                catch (Exception e)
                {

                  
                }
            }
        }
        public Color LogoColor
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_logoColor)||!string.IsNullOrWhiteSpace(_logoColor))
                    {
                        return _logoColor.ToColor();
                    }
                }
                catch (Exception e)
                {
                   
                }
                return Colors.Blue;
            }
            set
            {
                try
                {
                    SetProperty(ref _logoColor, value.ToString());
                }
                catch (Exception e)
                {
                  
                }
            }
        }
        public bool IsFavorite
        {
            get
            {
                try
                {
                    return _isFavorite;
                }
                catch (Exception e)
                {
                  
                }
                return false;
            }
            set
            {
                try
                {
                    SetProperty(ref _isFavorite, value);
                }
                catch (Exception e)
                {
                   
                }
            }
        }
        public double TextOpacity
        {
            get
            {
                try
                {
                    if (_textOpacity>0)
                    {
                        return _textOpacity;
                    }
                }
                catch (Exception e)
                {
                   
                }
                return 1.0;
            }
            set
            {
                try
                {
                    SetProperty(ref _textOpacity, value);
                }
                catch (Exception e)
                {
                 
                }
            }
        }
        public double BackgroundOpacity
        {
            get
            {
                try
                {
                    if (_backgroundOpacity>0)
                    {
                        return _backgroundOpacity;
                    }
                }
                catch (Exception e)
                {
                  
                }
                return 1.0;
            }
            set
            {
                try
                {
                    SetProperty(ref _backgroundOpacity, value);
                }
                catch (Exception e)
                {
                   
                }
            }
        }
       public double LogoOpacity
        {
            get
            {
                try
                {
                    if (_logoOpacity>0)
                    {
                        return _logoOpacity;
                    }
                }
                catch (Exception e)
                {
                   
                }
                return 1.0;
            }
            set
            {
                try
                {
                    SetProperty(ref _logoOpacity, value);
                }
                catch (Exception e)
                {
                   
                }
            }
        }
       


    }
}
