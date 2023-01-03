namespace UwpAppLauncher.Model
{
    public sealed class OldPageBackground
    {
        private string _imageFullPath;
        private string _backgroundDisplayName;
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_imageFullPath))
                {
                    return "";
                }
                return _imageFullPath;
            }
            set
            {
                _imageFullPath = value;
            }
        }
        public string BackgroundImageDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_backgroundDisplayName))
                {
                    return "File Name not retrieved";
                }
                return _backgroundDisplayName;
            }
            set
            {
                _backgroundDisplayName = value;
            }
        }

    }
}
