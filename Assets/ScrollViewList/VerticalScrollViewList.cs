using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Jing.ScrollViewList
{
    public class VerticalScrollViewList : VerticalScrollViewList<object>
    {
        public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemPrefab, itemRender, gap)
        {
        }
    }

    /// <summary>
    /// 垂直滚动列表
    /// </summary>
    public class VerticalScrollViewList<TData> : BaseScrollViewList<TData>
    {
        /// <summary>
        /// 内容容器滚动位置
        /// </summary>
        public float contentRenderStartPos { get; private set; }

        /// <summary>
        /// 列表项高度加上间距的值
        /// </summary>
        public float itemHeightWithGap { get; private set; }

        /// <summary>
        /// 可视区域高度
        /// </summary>
        public float viewportHeight { get; private set; }

        /// <summary>
        /// 数据对象显示的起始位置，这里是垂直列表，只存储Y坐标
        /// </summary>
        ItemModel<TData>[] _itemModels;

        public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemPrefab, itemRender, gap)
        {
            itemHeightWithGap = itemSize.y + gap;
        }

        protected override void OnSetDatas()
        {
            _itemModels = new ItemModel<TData>[_datas.Length];
            for(int i = 0; i < _datas.Length; i++)
            {
                //初始化位置，用Prefab的默认数据即可
                _itemModels[i] = new ItemModel<TData>(_datas[i]);
                _itemModels[i].height = itemSize.y;
            }

            //RebuildContent();
            MarkDirty(true);
        }

        public void RebuildContent()
        {
            float h = 0;
            for(int i = 0; i < _itemModels.Length; i++)
            {
                h += (_itemModels[i].height + gap);
            }
            h -= gap;

            SetContentSize(itemSize.x, h);

            Refresh();
        }

        /// <summary>
        /// 刷新显示视口的高度
        /// </summary>
        void UpdateViewportHeight()
        {
            viewportHeight = scrollRect.viewport.rect.height;
            if (0 == viewportHeight)
            {
                viewportHeight = scrollRect.GetComponent<RectTransform>().rect.height;
            }
        }

        protected override void OnScroll()
        {
            MarkDirty(false);
        }

        protected void Refresh()
        {
            UpdateViewportHeight();

            //内容容器高度
            var contentHeight = content.sizeDelta.y;            

            contentRenderStartPos = content.localPosition.y;
            if(contentRenderStartPos < 0)
            {
                contentRenderStartPos = 0;
            }
            else if(contentRenderStartPos > contentHeight - viewportHeight)
            {
                contentRenderStartPos = contentHeight - viewportHeight;
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

            //Debug.Log($"起始索引:{dataIdx}");

            List<ScrollListItem> recycledItems;
            RecycleItems(out recycledItems);

            int usedRecycleItem = 0;
            //显示的内容刚好大于这个值即可           
            float contentHeightLimit = viewportHeight;
            float itemY = -startPos;
            Dictionary<int, ScrollListItem> lastItemMap = new Dictionary<int, ScrollListItem>(_showingItems);
            _showingItems.Clear();
            do
            {
                if (dataIdx >= _datas.Length)
                {
                    break;
                }

                var data = _datas[dataIdx];
                
                ScrollListItem item;                

                //如果对应的数据对象，已经在 Hierarchy 中了，则直接更新位置即可
                if (false == lastItemMap.TryGetValue(dataIdx, out item))
                {
                    if (usedRecycleItem < recycledItems.Count)
                    {
                        item = recycledItems[usedRecycleItem++];                        
                    }
                    else
                    {
                        var go = GameObject.Instantiate(itemPrefab, content);
                        item = go.AddComponent<ScrollListItem>();
                        item.rectTransform.anchorMin = Vector2.up;
                        item.rectTransform.anchorMax = Vector2.up;
                        item.rectTransform.pivot = Vector2.up;
                    }

                    item.height = _itemModels[dataIdx].height;
                    var name = string.Format("item_{0}", dataIdx);
                    if (GameObject.Find(item.gameObject.name))
                    {
                        Debug.Log("?????");
                    }
                    item.gameObject.name = name;
                    

                    //先渲染，再定位
                    item.index = dataIdx;
                    item.data = data;
                    RenderItem(item, data);
                }
                else
                {
                    lastItemMap.Remove(dataIdx);
                }

                if (item.height != _itemModels[dataIdx].height)
                {
                    Debug.Log($"item[{dataIdx}]的尺寸改变，重构列表");
                    _itemModels[dataIdx].height = item.height;
                    MarkDirty(true);
                }                
                
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
                    //显示区域已满

                    foreach(var unusefulItem in lastItemMap.Values)
                    {
                        recycledItems.Add(unusefulItem);
                    }
                    break;
                }
            }
            while (true);

            for (int i = usedRecycleItem; i < recycledItems.Count; i++)
            {
                GameObject.Destroy(recycledItems[i].gameObject);
            }
        }

        Coroutine _updateFlag;
        bool _isRebuild = false;

        void MarkDirty(bool isRebuild)
        {
            _isRebuild = _isRebuild | isRebuild;

            if (null == _updateFlag)
            {
                _updateFlag = scrollRect.StartCoroutine(NextFrameWork());
            }        
        }

        IEnumerator NextFrameWork()
        {
            yield return new WaitForEndOfFrame();            
            var isRebuild = _isRebuild;
            _isRebuild = false;
            _updateFlag = null;

            if (isRebuild)
            {
                Debug.Log("Rebuild");
                RebuildContent();
            }
            else
            {
                Refresh();
            }

        }

        /// <summary>
        /// 当前显示的Item表
        /// </summary>
        Dictionary<int, ScrollListItem> _showingItems = new Dictionary<int, ScrollListItem>();

        /// <summary>
        /// 回收列表项
        /// </summary>
        /// <returns></returns>
        void RecycleItems(out List<ScrollListItem> recycledItems)
        {
            recycledItems = new List<ScrollListItem>();            

            float viewportMinY = contentRenderStartPos;
            float viewportMaxY = contentRenderStartPos + viewportHeight;

            foreach (var item in _showingItems.Values)
            {                
                var top = -item.rectTransform.localPosition.y;
                var bottom = top + item.height;
                if (bottom >= viewportMinY && top < viewportMaxY)
                {
                    //不用刷新数据，仅需要调整位置
                }
                else
                {
                    //Debug.Log("回收并刷新数据:" + child.name);
                    recycledItems.Add(item);
                }
            }
        }
    }
}