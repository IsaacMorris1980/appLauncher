using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Model
{
   public class AppSettings
    {
        public double fontsize { get; set; } = 10;
        public Color ForegroundColor { get; set; } = Colors.Red;
        public SolidColorBrush ForegroundColorBrush =>  new SolidColorBrush(this.ForegroundColor);
    }
}
