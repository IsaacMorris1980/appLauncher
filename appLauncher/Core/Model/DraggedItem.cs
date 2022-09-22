namespace appLauncher.Core.Model
{
    public class DraggedItem
    {
        public int initialindex { get; set; }
        public AppTile itemdragged { get; set; }
        public int initalPagenumber { get; set; }
        public int newpage { get; set; }
        public int indexonnewpage { get; set; }
    }
}
