﻿using appLauncher.Core.Brushes;
using appLauncher.Core.Interfaces;

using Microsoft.Toolkit.Uwp.Helpers;

using Newtonsoft.Json;

using System;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class FinalTiles : ModelBase, IApporFolder
    {
        private const string _notRetrieved = "{0} was not retrieved";
        private string _name;
        private string _fullName;
        private string _description;
        private string _developer;
        private long _installedDate;
        private int _listpos;
        private string _logo;
        private string _logoColor;
        private string _backColor;
        private string _textColor;
        private string _tip;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return string.Format(_notRetrieved, "App Name");
                }
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(_fullName))
                {
                    return string.Format(_notRetrieved, "App Full Name");
                }
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return string.Format(_notRetrieved, _description);
                }
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        public string Developer
        {
            get
            {
                if (string.IsNullOrEmpty(_developer))
                {
                    return string.Format(_notRetrieved, "App Developer");
                }
                return _developer;
            }
            set
            {
                _developer = value;
            }
        }
        public DateTimeOffset InstalledDate
        {
            get
            {
                if (_installedDate == 0)
                {
                    return DateTimeOffset.FromUnixTimeSeconds(0);
                }
                return DateTimeOffset.FromUnixTimeSeconds(_installedDate);
            }
            set
            {
                _installedDate = value.ToUnixTimeSeconds();
            }
        }
        public int ListPos
        {
            get
            {
                return _listpos;
            }
            set
            {
                SetProperty(ref _listpos, value);
            }
        }
        public byte[] Logo
        {
            get
            {
                if (string.IsNullOrEmpty(_logo))
                {
                    return new byte[1];
                }
                return Convert.FromBase64String(_logo);
            }
            set
            {
                _logo = Convert.ToBase64String(value);
            }
        }
        public Color LogoColor
        {
            get
            {
                if (string.IsNullOrEmpty(_logoColor))
                {
                    return "Blue".ToColor();
                }
                return _logoColor.ToColor();
            }
            set
            {
                SetProperty(ref _logoColor, value.ToString(), "LogoBrush");
            }
        }
        public Color BackColor
        {
            get
            {
                if (string.IsNullOrEmpty(_backColor))
                {
                    return "Transparent".ToColor();
                }
                return _backColor.ToColor();
            }
            set
            {
                SetProperty(ref _backColor, value.ToString(), "BackBrush");
            }
        }
        public Color TextColor
        {
            get
            {
                if (string.IsNullOrEmpty(_textColor))
                {
                    return "Red".ToColor();
                }
                return _textColor.ToColor();
            }
            set
            {
                SetProperty(ref _textColor, value.ToString(), "TextBrush");
            }
        }
        public string Tip
        {
            get
            {
                if (string.IsNullOrEmpty(_tip))
                {
                    return string.Format(_notRetrieved, "App Tool Tip");
                }
                return _tip;
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
    }
}