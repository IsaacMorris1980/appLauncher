namespace appLauncher.Core.Model
{
    public class PageChangingVariables : ModelBase
    {
        private bool _isNext = false;
        private bool _isPrevious = false;
        public bool IsNext
        {
            get
            {
                return _isNext;
            }
            set
            {
                SetProperty(ref _isNext, value);
            }
        }
        public bool IsPrevious
        {
            get
            {
                return _isPrevious;
            }
            set
            {
                SetProperty(ref _isPrevious, value);
            }
        }
    }
}