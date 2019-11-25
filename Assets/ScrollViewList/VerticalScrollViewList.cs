using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jing.ScrollViewList
{
    public class VerticalScrollViewList : VerticalScrollViewList<object>
    {
        public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemPrefab, itemRender, gap)
        {

        }

        public void SetDatas<TData>(TData[] datas)
        {
            _datas = new object[datas.Length];
            Array.Copy(datas, _datas, _datas.Length);
            base.AddDatas(_datas);
        }
    }

    /// <summary>
    /// 垂直滚动列表
    /// </summary>
    public class VerticalScrollViewList<TData> : BaseScrollViewList<TData>
    {
        public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemPrefab, itemRender, gap)
        {

        }

        protected override void RebuildContent()
        {
            float h = 0;
            for(int i = 0; i < _itemModels.Length; i++)
            {
                h += (_itemModels[i].height + gap);
            }
            h -= gap;

            SetContentSize(viewportSize.x, h);

            Refresh();
        }

        protected override void Refresh()
        {
            UpdateViewportSize();

            //内容容器高度
            var contentHeight = content.sizeDelta.y;            

            contentRenderStartPos = content.localPosition.y;
            if(contentRenderStartPos < 0)
            {
                contentRenderStartPos = 0;
            }
            else if(contentRenderStartPos > contentHeight - viewportSize.y)
            {
                contentRenderStartPos = contentHeight - viewportSize.y;
            }

            int dataIdx;
            float startPos = 0;

            for(dataIdx = 0; dataIdx < _itemModels.Length; dataIdx++)
            {
                var dataBottom = startPos + _itemModels[dataIdx].height;
                if (dataBottom >= contentRenderStartPos)
                {
                    //就是我了
                    break;
                }

                startPos = dataBottom + gap;
            }                                 
            
            //显示的内容刚好大于这个值即可           
            float contentHeightLimit = viewportSize.y;
            float itemY = -startPos;

            /// <summary>
            /// 最后一次显示的Item的缓存
            /// </summary>
            Dictionary<int, ScrollListItem> lastShowingItems = new Dictionary<int, ScrollListItem>(_showingItems);

            _showingItems.Clear();
            do
            {
                if (dataIdx >= _datas.Length)
                {
                    break;
                }

                var data = _datas[dataIdx];
                
                ScrollListItem item = CreateItem(data, dataIdx, lastShowingItems);
                //item.gameObject.name += $"_{_itemModels[dataIdx].height}";
                _showingItems[dataIdx] = item;

                var pos = Vector3.zero;
                pos.y = itemY;
                item.rectTransform.localPosition = pos;
                //下一个item的Y坐标
                itemY -= (item.height + gap);
                //下一个item的索引
                dataIdx++;

                if (-contentRenderStartPos - itemY >= contentHeightLimit)
                {
                    break;
                }
            }
            while (true);

            //回收没有使用的item
            foreach(var item in lastShowingItems.Values)
            {
                //如果不要内存池，则直接Destroy即可
                //GameObject.Destroy(item.gameObject);

                item.gameObject.SetActive(false);
                _recycledItems.Add(item);
            }            
        }

        protected override void CheckItemsSize()
        {
            foreach(var item in _showingItems.Values)
            {
                if (item.height != _itemModels[item.index].height)
                {
                    //Debug.Log($"item[{item.index}]的尺寸改变 {_itemModels[item.index].height} => {item.height}");
                    _itemModels[item.index].height = item.height;
                    MarkDirty(EUpdateType.REBUILD);
                }
            }
        }        
    }
}