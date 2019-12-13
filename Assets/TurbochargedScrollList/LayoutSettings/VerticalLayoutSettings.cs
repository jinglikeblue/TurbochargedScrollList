namespace Jing.TurbochargedScrollList
{
    public class VerticalLayoutSettings : BaseLayoutSettings
    {
        public float PaddingTop
        {
            get
            {
                return paddingTop;
            }
            set
            {
                paddingTop = value;
            }
        }

        public float PaddingBottom
        {
            get
            {
                return paddingBottom;
            }
            set
            {
                paddingBottom = value;
            }
        }

        public float Gap
        {
            get
            {
                return gapY;
            }
            set
            {
                gapY = value;
            }
        }
    }
}
