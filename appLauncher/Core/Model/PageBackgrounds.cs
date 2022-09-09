namespace appLauncher.Core.Model
{
    public class PageBackgrounds : ModelBase
    {
        public string imagelocationstring { get; set; }
        public string imagedisplayname { get; set; }
        public string backgroundoverlaycolor { get; set; } = "Blue";
        public string opacity { get; set; } = 0.30;
    }
}
