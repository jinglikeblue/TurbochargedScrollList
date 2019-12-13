using System;

namespace Jing.TurbochargedScrollList
{
    [Serializable]
    public class GridLayoutSettings : BaseLayoutSettings
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

        public float GapX
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

        public float GapY
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
