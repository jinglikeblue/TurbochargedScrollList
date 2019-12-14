using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    public class GridScrollList : BaseScrollList
    {
        public GridLayoutSettings layout { get; private set; }

        /// <summary>
        /// 行数
        /// </summary>
        public int rowCount { get; private set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int colCount { get; private set; }

        /// <summary>
        /// item宽度加上间距的值
        /// </summary>
        float _bigW;

        /// <summary>
        /// item高度加上间距的值
        /// </summary>
        float _bigH;

        public GridScrollList(ScrollRect scrollRect, GameObject itemPrefab, GridLayoutSettings layoutSettings = null)
        {
            if (null == layoutSettings)
            {
                layout = new GridLayoutSettings();
            }
            else
            {
                layout = layoutSettings;
            }

            InitScrollView(scrollRect);

            InitItem(itemPrefab);
        }

        protected override bool AdjustmentItemSize(ScrollListItem item)
        {
            if (item.width != itemDefaultfSize.x || item.height != itemDefaultfSize.y)
            {
                item.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemDefaultfSize.x);
                item.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemDefaultfSize.y);
            }
            return false;
        }

        protected override void Refresh(UpdateData updateConfig, out int lastStartIndex)
        {
            var scrollX = -content.localPosition.x;
            if (scrollX < 0)
            {
                scrollX = 0;
            }
            var scrollY = content.localPosition.y;
            if (scrollY < 0)
            {
                scrollY = 0;
            }

            bool isColByCol = layout.constraint == EGridConstraint.FIXED_ROW_COUNT ? true : false;

            //计算出关联的所有Item的索引
            List<GridPos> list = new List<GridPos>();
            lastStartIndex = 0;
            if (colCount > 0 && rowCount > 0)
            {
                //根据滚动区域左上角算出起始item的二维位置;
                GridPos startPos = new GridPos((int)(scrollX / _bigW), (int)(scrollY / _bigH), colCount, rowCount, _bigW, _bigH);
                lastStartIndex = startPos.index;
                //根据滚动区域左下角算出结束item的二维位置
                GridPos endPos = new GridPos((int)((scrollX + viewportSize.x) / _bigW), (int)((scrollY + viewportSize.y) / _bigH), colCount, rowCount, _bigW, _bigH);

                if(isColByCol)
                {
                    RefreshGridPosColByCol(list, ref startPos, ref endPos);
                }
                else
                {
                    RefreshGridPosRowByRow(list, ref startPos, ref endPos);
                }
            }

            /// <summary>
            /// 最后一次显示的Item的缓存
            /// </summary>
            Dictionary<ScrollListItemModel, ScrollListItem> lastShowingItems = new Dictionary<ScrollListItemModel, ScrollListItem>(_showingItems);

            _showingItems.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                var gridPos = list[i];
                var model = _itemModels[gridPos.index];

                ScrollListItem item = CreateItem(model, gridPos.index, lastShowingItems);
                _showingItems[model] = item;
                var pos = new Vector3(gridPos.pixelX, -gridPos.pixelY, 0);
                pos.x += layout.paddingLeft;
                pos.y -= layout.paddingTop;
                item.rectTransform.localPosition = pos;
            }

            //回收没有使用的item
            foreach (var item in lastShowingItems.Values)
            {
                item.gameObject.SetActive(false);
                _recycledItems.Add(item);
            }
        }
        
        
        /// <summary>
        /// 从左往右，从上往下依次刷新
        /// ↓ ↓ ↓ Example
        /// 0 1 2
        /// 3 4 5
        /// 6 7 
        /// </summary>
        void RefreshGridPosRowByRow(List<GridPos> output, ref GridPos startPos, ref GridPos endPos)
        {
            output.Clear();
            for (int y = startPos.y; y <= endPos.y; y++)
            {
                for (int x = startPos.x; x <= endPos.x; x++)
                {
                    var gp = new GridPos(x, y, colCount, rowCount, _bigW, _bigH);
                    if (gp.index >= 0 && gp.index < _itemModels.Count)
                    {
                        output.Add(gp);
                    }
                    else
                    {
                        break;
                    }
                }
            }            
        }

        /// <summary>
        /// 从上往下，从左往右依次刷新
        /// ↓ ↓ ↓ Example
        /// 0 3 6 
        /// 1 4 7
        /// 2 5 
        /// </summary>
        void RefreshGridPosColByCol(List<GridPos> output, ref GridPos startPos, ref GridPos endPos)
        {
            output.Clear();
            for (int x = startPos.x; x <= endPos.x; x++)
            {
                for (int y = startPos.y; y <= endPos.y; y++)
                {
                    var gp = new GridPos(x, y, colCount, rowCount, _bigW, _bigH);
                    gp.AxisFlip();
                    if (gp.index >= 0 && gp.index < _itemModels.Count)
                    {
                        output.Add(gp);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }        

        protected override void ResizeContent(UpdateData updateConfig)
        {
            float contentW = 0;
            float contentH = 0;
            int itemAmount = _itemModels.Count;

            _bigW = itemDefaultfSize.x + layout.gapX;
            _bigH = itemDefaultfSize.y + layout.gapY;

            colCount = 0;
            rowCount = 0;

            //计算Content大小
            switch (layout.constraint)
            {
                case EGridConstraint.FLEXIBLE: //根据视口确定Content大小，并且计算出constraint数量
                    contentW = viewportSize.x - layout.paddingLeft - layout.paddingRight;
                    //计算出constraintCount
                    layout.constraintCount = (int)((contentW + layout.gapX) / _bigW);
                    //确定高度,通过item总数和constraintCount算出                    
                    if (itemAmount > 0)
                    {
                        rowCount = (itemAmount - 1) / layout.constraintCount + 1;
                        contentH = rowCount * _bigH - layout.gapY;
                    }

                    colCount = layout.constraintCount;
                    break;
                case EGridConstraint.FIXED_COLUMN_COUNT: //根据列数确定Content大小

                    colCount = layout.constraintCount;
                    if (_itemModels.Count < colCount)
                    {
                        colCount = _itemModels.Count;
                    }

                    contentW = colCount * _bigW - layout.gapX;
                    //确定高度,通过item总数和constraintCount算出
                    if (itemAmount > 0)
                    {
                        rowCount = (itemAmount - 1) / colCount + 1;
                        contentH = rowCount * _bigH - layout.gapY;
                    }

                    break;
                case EGridConstraint.FIXED_ROW_COUNT: //根据行数确定Content大小

                    rowCount = layout.constraintCount;
                    if (_itemModels.Count < rowCount)
                    {
                        rowCount = _itemModels.Count;
                    }

                    contentH = rowCount * _bigH - layout.gapY;
                    if (contentH < 0)
                    {
                        contentH = 0;
                    }
                    //确定宽度,通过item总数和constraintCount算出                    
                    if (itemAmount > 0)
                    {
                        colCount = (itemAmount - 1) / rowCount + 1;
                        contentW = colCount * _bigW - layout.gapX;
                    }
                    break;
            }

            if (contentW < 0)
            {
                contentW = 0;
            }

            if (contentH < 0)
            {
                contentH = 0;
            }

            var w = contentW + layout.paddingLeft + layout.paddingRight;
            var h = contentH + layout.paddingTop + layout.paddingBottom;

            SetContentSize(w, h);
        }        

        public override void ScrollToItem(int index)
        {
            bool isColByCol = layout.constraint == EGridConstraint.FIXED_ROW_COUNT ? true : false;
            GridPos gridPos;
            if (isColByCol)
            {
                int x = index / rowCount;
                int y = index % rowCount;                
                gridPos = new GridPos(x, y, colCount, rowCount, _bigW, _bigH);
                gridPos.AxisFlip();
            }
            else
            {
                int x = index % colCount;
                int y = index / colCount;
                gridPos = new GridPos(x, y, colCount, rowCount, _bigW, _bigH);
            }            

            ScrollToPosition(gridPos.pixelX + layout.paddingLeft, gridPos.pixelY + layout.paddingTop);
        }

        public void ScrollToPosition(float x, float y)
        {        
            ScrollToPosition(new Vector2(x, y));
        }
    }
}
