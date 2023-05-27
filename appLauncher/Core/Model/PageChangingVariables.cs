namespace appLauncher.Core.Model
{
    public class PageChangingVariables : ModelBase
    {
        private bool isNext = false;
        private bool isPrevious = false;
        public bool IsNext
        {
            get
            {
                return isNext;
            }
            set
            {
                SetProperty(ref isNext, value);
            }
        }
        public bool IsPrevious
        {
            get
            {
                return isPrevious;
            }
            set
            {
                SetProperty(ref isPrevious, value);
            }
        }
    }
}