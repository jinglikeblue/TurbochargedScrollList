using System.Collections.Generic;
using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 垂直滚动列表
    /// </summary>
    public class VerticalScrollList : BaseScrollList
    {
        public VerticalLayoutSettings layout { get; private set; }

        public VerticalScrollList(GameObject scrollView, GameObject itemPrefab, VerticalLayoutSettings layoutSettings)
        {
            layout = layoutSettings;

            InitScrollView(scrollView);                                  

            InitItem(itemPrefab);             
        }

        protected override void ResizeContent(UpdateData updateConfig)
        {
            float h = 0;
            for (int i = 0; i < _itemModels.Count; i++)
            {
                h += (_itemModels[i].height + layout.gap);
            }
            h -= layout.gap;

            //加上上下边距
            h = h + layout.paddingTop + layout.paddingBottom;            
            var w = viewportSize.x;
            SetContentSize(w, h);
        }

        protected override void Refresh(UpdateData updateConfig, out int lastStartIndex)
        {                                  
            if (updateConfig.keepPaddingType == EKeepPaddingType.END)
            {
                var targetRenderStartPos = (ContentHeight - updateConfig.tempLastContentRect.height) + contentRenderStartPos;
                var temp = content.localPosition;
                temp.y = targetRenderStartPos;
                content.localPosition = temp;                
            }

            contentRenderStartPos = content.localPosition.y;
            if(contentRenderStartPos < 0)
            {
                contentRenderStartPos = 0;
            }
            else if(contentRenderStartPos > ContentHeight - viewportSize.y)
            {
                contentRenderStartPos = ContentHeight - viewportSize.y;
            }

            int dataIdx;
            float startPos = layout.paddingTop;

            for(dataIdx = 0; dataIdx < _itemModels.Count; dataIdx++)
            {
                var dataBottom = startPos + _itemModels[dataIdx].height;
                if (dataBottom >= contentRenderStartPos)
                {
                    //就是我了
                    break;
                }

                startPos = dataBottom + layout.gap;
            }

            lastStartIndex = dataIdx;

            //显示的内容刚好大于这个值即可           
            float contentHeightLimit = viewportSize.y;
            float itemY = -startPos;

            /// <summary>
            /// 最后一次显示的Item的缓存
            /// </summary>
            Dictionary<ScrollListItemModel, ScrollListItem> lastShowingItems = new Dictionary<ScrollListItemModel, ScrollListItem>(_showingItems);

            _showingItems.Clear();

            while(dataIdx < _itemModels.Count)
            {
                var model = _itemModels[dataIdx];

                ScrollListItem item = CreateItem(model, dataIdx, lastShowingItems);
                //item.gameObject.name += $"_{_itemModels[dataIdx].height}";
                
                _showingItems[model] = item;

                var pos = Vector3.zero;                
                pos.y = itemY;                
                item.rectTransform.localPosition = pos;
                //下一个item的Y坐标
                itemY -= (item.height + layout.gap);
                //下一个item的索引
                dataIdx++;

                if (-contentRenderStartPos - itemY >= contentHeightLimit)
                {
                    break;
                }
            }

            //回收没有使用的item
            foreach(var item in lastShowingItems.Values)
            {
                //如果不要内存池，则直接Destroy即可
                //GameObject.Destroy(item.gameObject);

                item.gameObject.SetActive(false);
                _recycledItems.Add(item);
            }            
        }
        
        protected override bool AdjustmentItemSize(ScrollListItem item)
        {
            if (item.height != _itemModels[item.index].height)
            {
                //Debug.Log($"item[{item.index}]的尺寸改变 {_itemModels[item.index].height} => {item.height}");
                _itemModels[item.index].height = item.height;
                return true;
            }
            return false;
        }

        public override void ScrollToItem(int index)
        {          
            if (index < 0)
            {
                index = 0;
            }
            else if(index >= _itemModels.Count)
            {
                index = _itemModels.Count - 1;
            }

            float pos = layout.paddingTop;
            for (int i = 0; i < index; i++)
            {
                pos += (_itemModels[i].height + layout.gap);
            }

            ScrollToPosition(pos);
        }

        public void ScrollToPosition(float position)
        {
            ScrollToPosition(new Vector2(0, position));
        }
    }
}