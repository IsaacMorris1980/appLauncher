using UwpAppLauncher.Interfaces;

namespace UwpAppLauncher.Model
{
    public class DraggedItem
    {
        public int initialindex { get; set; }
        public IApporFolder itemdragged { get; set; }
        public int initalPagenumber { get; set; }
        public int newpage { get; set; }
        public int indexonnewpage { get; set; }
    }
}
