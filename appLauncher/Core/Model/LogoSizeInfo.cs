using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage.Streams;

namespace appLauncher.Core.Model
{
    public class LogoSizeInfo
    {
        public Size RequestedSize { get; set; }
        public RandomAccessStreamReference StreamRef { get; set; }
    }
}
