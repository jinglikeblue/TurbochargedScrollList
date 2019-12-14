namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// GridScrollList使用的格子位置参数
    /// </summary>
    public struct GridPos
    {
        public int x { get; private set; }

        public int y { get; private set; }

        public float pixelX { get; private set; }

        public float pixelY { get; private set; }

        public int index { get; private set; }

        int _gridRowCount;
        int _gridColCount;
        float _gridW;
        float _gridH;

        public GridPos(int x, int y, int gridColCount, int gridRowCount, float gridW, float gridH)
        {
            this._gridRowCount = gridRowCount;
            this._gridColCount = gridColCount;
            this._gridW = gridW;
            this._gridH = gridH;

            if (gridColCount == 0 || gridRowCount == 0)
            {
                this.x = this.y = 0;
                this.pixelX = this.pixelY = 0;
                this.index = -1;
            }
            else
            {
                this.x = x >= gridColCount ? gridColCount - 1 : x;
                this.y = y >= gridRowCount ? gridRowCount - 1 : y;
                this.pixelX = x * _gridW;
                this.pixelY = y * _gridH;
                this.index = y * _gridColCount + x;
            }
        }

        /// <summary>
        /// 将index在x、y轴上的位置
        /// </summary>
        public void AxisFlip()
        {
            this.index = x * _gridRowCount + y;
        }
    }
}
