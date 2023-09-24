using appLauncher.Core.Interfaces;

using System.Collections.Generic;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
    public class AppFolder : ModelBase, IApporFolder
    {
        private string _name;
        private string _description;
        private List<FinalTiles> _folderapps = new List<FinalTiles>();
        private Color _textcolor = Colors.Orange;
        private Color _backcolor = Colors.Black;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return string.Empty;
                }
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public string Description { get; set; }
        public List<FinalTiles> FolderApps { get; set; }
        public int ListPos { get; set; }
        public SolidColorBrush TextBrush { get; set; }
        public SolidColorBrush BackBrush { get; set; }
        public Color TextColor { get; set; }
        public Color BackColor { get; set; }
    }
}
