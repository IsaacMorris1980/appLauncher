using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage.Streams;

namespace appLauncher.Core.Model
{
    public class LogoSelectionResult
    {
        public RandomAccessStreamReference StreamRef { get; set; }
        public int? TargetDecodeWidth { get; set; }
    }
}
