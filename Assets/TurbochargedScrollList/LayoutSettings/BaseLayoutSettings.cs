namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 布局设置
    /// </summary>
    public abstract class BaseLayoutSettings
    {
        public float paddingLeft = 0;
        public float paddingRight = 0;
        public float paddingTop = 0;
        public float paddingBottom = 0;

        public float gapX = 0;
        public float gapY = 0;

        public BaseLayoutSettings()
        {

        }

        protected void SetPadding(float left, float right, float top, float bottom)
        {
            paddingLeft = left;
            paddingRight = right;
            paddingTop = top;
            paddingBottom = bottom;
        }
    }
}
