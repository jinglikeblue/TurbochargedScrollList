namespace Jing.TurbochargedScrollList
{
    class HorizontalLayoutSettings : BaseLayoutSettings
    {
        public float PaddingLeft
        {
            get
            {
                return paddingLeft;
            }
            set
            {
                paddingLeft = value;
            }
        }

        public float PaddingRight
        {
            get
            {
                return paddingRight;
            }
            set
            {
                paddingRight = value;
            }
        }

        public float Gap
        {
            get
            {
                return gapX;
            }
            set
            {
                gapX = value;
            }
        }
    }
}
