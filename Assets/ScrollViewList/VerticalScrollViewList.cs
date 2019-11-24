using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Jing.ScrollViewList
{
    public class VerticalScrollViewList : VerticalScrollViewList<object>
    {
        public VerticalScrollViewList(GameObject scrollView, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemRender, gap)
        {

        }

        public void SetDatas<TData>(TData[] datas, GameObject itemPrefab)
        {
            _datas = new object[datas.Length];
            Array.Copy(datas, _datas, _datas.Length);
            Clear();
            OnSetDatas(itemPrefab);
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
        /// 数据对象显示的起始位置，这里是垂直列表，只存储Y坐标
        /// </summary>
        ItemModel<TData>[] _itemModels;

        /// <summary>
        /// 当前显示的Item表(key: 数据索引)
        /// </summary>
        Dictionary<int, ScrollListItem> _showingItems = new Dictionary<int, ScrollListItem>();

        /// <summary>
        /// 回收列表项
        /// </summary>
        List<ScrollListItem> _recycledItems = new List<ScrollListItem>();

        public VerticalScrollViewList(GameObject scrollView, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemRender, gap)
        {

        }

        protected override void OnSetDatas(GameObject itemPrefab)
        {            
            _itemModels = new ItemModel<TData>[_datas.Length];
            for(int i = 0; i < _datas.Length; i++)
            {
                //初始化位置，用Prefab的默认数据即可
                _itemModels[i] = new ItemModel<TData>(_datas[i], itemPrefab);                                
            }
            //RebuildContent();
            MarkDirty(EUpdateType.REBUILD);
        }

        public void RebuildContent()
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

        protected override void OnScroll()
        {
            MarkDirty(EUpdateType.REFRESH);
        }

        protected void Refresh()
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
                //GameObject.Destroy(item.gameObject);
                item.gameObject.SetActive(false);
                _recycledItems.Add(item);
            }            
        }

        enum EUpdateType
        {
            NONE,
            REFRESH, 
            REBUILD,            
        }
        
        /// <summary>
        /// 更新方式
        /// </summary>
        EUpdateType _updateType;

        void MarkDirty(EUpdateType updateType)
        {
            if(_updateType == updateType || _updateType == EUpdateType.REBUILD)
            {
                return;
            }

            _updateType = updateType;   
        }

        public void Update()
        {
            var type = _updateType;

            _updateType = EUpdateType.NONE;

            switch (type)
            {
                case EUpdateType.REFRESH:
                    Refresh();
                    break;
                case EUpdateType.REBUILD:
                    //Debug.Log("Rebuild");
                    RebuildContent();
                    break;
            }

            CheckItemsSize();
        }

        void CheckItemsSize()
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

        public ScrollListItem CreateItem(TData data, int dataIdx, Dictionary<int, ScrollListItem> lastShowingItems)
        {
            ScrollListItem item;
            if (lastShowingItems.ContainsKey(dataIdx))
            {
                item = lastShowingItems[dataIdx];
                lastShowingItems.Remove(dataIdx);
                return item;
            }

            if (_recycledItems.Count > 0)
            {
                ScrollListItem tempItem = null;

                int idx = _recycledItems.Count;
                while(--idx > -1)
                {
                    tempItem = _recycledItems[idx];
                    if (tempItem.index == dataIdx && tempItem.data.Equals(data))
                    {
                        //以前回收的一个对象，刚好对应的数据一致
                        item = tempItem;                                     
                        item.gameObject.SetActive(true);
                        _recycledItems.RemoveAt(idx);
                        return item;
                    }
                }

                //没有绝对匹配的，则抽取最后找到的item，之后刷新数据并返回
                item = tempItem;                
                item.gameObject.SetActive(true);
                _recycledItems.Remove(tempItem);                
            }
            else
            {
                var go = GameObject.Instantiate(_itemModels[dataIdx].itemPrefab, content);
                item = go.AddComponent<ScrollListItem>();
                item.rectTransform.anchorMin = Vector2.up;
                item.rectTransform.anchorMax = Vector2.up;
                item.rectTransform.pivot = Vector2.up;
                Debug.Log($"新生成的 Item GameObject");

                item.height = _itemModels[dataIdx].height;
            }
            
            var name = string.Format("item_{0}", dataIdx);
            //Debug.Log($"刷新Item: {name}");
            item.gameObject.name = name;

            item.index = dataIdx;
            item.data = data;
            RenderItem(item, data);

            return item;
        }
    }
}