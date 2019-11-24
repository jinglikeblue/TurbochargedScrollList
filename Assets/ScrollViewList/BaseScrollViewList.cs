using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.ScrollViewList
{
    /// <summary>
    /// 基于UGUI中Scroll View组件的列表工具
    /// </summary>
    public abstract class BaseScrollViewList<TData>
    {
        /// <summary>
        /// 列表项的模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected struct ItemModel<T>
        {
            public readonly T data;

            public float height;

            public float width;

            public ItemModel(T data, Vector2 defaultSize)
            {
                this.data = data;
                height = defaultSize.x;
                width = defaultSize.y;
            }
        }

        /// <summary>
        /// 列表更新方式
        /// </summary>
        protected enum EUpdateType
        {
            NONE,
            REFRESH,
            REBUILD,
        }

        public delegate void OnRenderItem(ScrollListItem item, TData data);

        public ScrollRect scrollRect { get; private set; }

        public RectTransform content { get; private set; }

        public GameObject gameObject { get; private set; }

        public GameObject itemPrefab { get; private set; }

        public Vector2 itemDefaultfSize { get; private set; }

        /// <summary>
        /// 视口大小
        /// </summary>
        public Vector2 viewportSize { get; private set; }

        /// <summary>
        /// 列表项间距
        /// </summary>
        public float gap { get; private set; }

        public Vector2 scrollPos { get; protected set; }

        protected TData[] _datas;

        OnRenderItem _itemRender;

        /// <summary>
        /// 内容容器滚动位置
        /// </summary>
        public float contentRenderStartPos { get; protected set; }

        /// <summary>
        /// 列表项数据
        /// </summary>
        protected ItemModel<TData>[] _itemModels;

        /// <summary>
        /// 当前显示的Item表(key: 数据索引)
        /// </summary>
        protected Dictionary<int, ScrollListItem> _showingItems = new Dictionary<int, ScrollListItem>();

        /// <summary>
        /// 回收列表项
        /// </summary>
        protected List<ScrollListItem> _recycledItems = new List<ScrollListItem>();

        /// <summary>
        /// 更新方式
        /// </summary>
        protected EUpdateType _updateType;

        public BaseScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap)
        {
            Init(scrollView);            
            this.gap = gap;
            this.itemPrefab = itemPrefab;
            itemDefaultfSize = itemPrefab.GetComponent<RectTransform>().sizeDelta;

            _itemRender = itemRender;
            
            scrollPos = Vector2.up;
        }

        void Init(GameObject gameObject)
        {
            scrollRect = gameObject.GetComponent<ScrollRect>();

            content = scrollRect.content;
            content.localPosition = Vector3.zero;

            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        /// <summary>
        /// 刷新显示视口的大小
        /// </summary>
        protected void UpdateViewportSize()
        {
            viewportSize = scrollRect.viewport.rect.size;
            //viewportHeight = scrollRect.viewport.rect.height;
            if (0 == viewportSize.x && 0 == viewportSize.y)
            {
                viewportSize = scrollRect.GetComponent<RectTransform>().rect.size;
            }            
        }

        protected void RenderItem(ScrollListItem item, TData data)
        {
            _itemRender.Invoke(item, data);
        }

        /// <summary>
        /// 获取数据拷贝
        /// </summary>
        public TData[] GetDatasCopy()
        {
            if (null == _datas)
            {
                return null;
            }

            TData[] datas = new TData[_datas.Length];
            _datas.CopyTo(datas, 0);
            return datas;
        }

        private void OnScroll(Vector2 v)
        {
            scrollPos = v;
            MarkDirty(EUpdateType.REFRESH);
        }

        public void SetDatas(TData[] datas)
        {
            _datas = new TData[datas.Length];            
            Array.Copy(datas, _datas, _datas.Length);
            Clear();            
            OnSetDatas();
        }

        protected void OnSetDatas()
        {
            _itemModels = new ItemModel<TData>[_datas.Length];
            for (int i = 0; i < _datas.Length; i++)
            {
                //初始化位置，用Prefab的默认数据即可
                _itemModels[i] = new ItemModel<TData>(_datas[i], itemDefaultfSize);
            }
            MarkDirty(EUpdateType.REBUILD);
        }

        protected void MarkDirty(EUpdateType updateType)
        {
            if (_updateType == updateType || _updateType == EUpdateType.REBUILD)
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

        protected abstract void Refresh();

        protected abstract void RebuildContent();

        protected abstract void CheckItemsSize();

        public void Clear()
        {
            int childIdx = content.childCount;
            while (--childIdx > -1)
            {
                GameObject.Destroy(content.GetChild(childIdx).gameObject);
            }
        }

        protected void SetContentSize(float x, float y)
        {
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
        }

        public void Destroy()
        {
            scrollRect.onValueChanged.RemoveListener(OnScroll);
        }

        /// <summary>
        /// 创建一个列表项Item
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataIdx"></param>
        /// <param name="lastShowingItems"></param>
        /// <returns></returns>
        protected ScrollListItem CreateItem(TData data, int dataIdx, Dictionary<int, ScrollListItem> lastShowingItems)
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
                //tempItem = _recycledItems[0];
                int idx = _recycledItems.Count;
                while (--idx > -1)
                {
                    tempItem = _recycledItems[idx];
                    if (tempItem.index == dataIdx && tempItem.data.Equals(data))
                    {
                        //以前回收的一个对象，刚好对应的数据一致
                        item = tempItem;
                        _recycledItems.RemoveAt(idx);
                        item.gameObject.SetActive(true);
                        return item;
                    }
                }

                //没有绝对匹配的，则抽取最后找到的item，之后刷新数据并返回
                item = tempItem;
                _recycledItems.Remove(tempItem);
            }
            else
            {
                var go = GameObject.Instantiate(itemPrefab, content);
                item = go.AddComponent<ScrollListItem>();
                item.rectTransform.anchorMin = Vector2.up;
                item.rectTransform.anchorMax = Vector2.up;
                item.rectTransform.pivot = Vector2.up;
                //Debug.Log($"新生成的 Item GameObject");                
            }

            var name = string.Format("item_{0}", dataIdx);

            item.gameObject.name = name;
            item.index = dataIdx;
            item.data = data;
            item.width = _itemModels[dataIdx].width;
            item.height = _itemModels[dataIdx].height;

            if (false == item.gameObject.activeInHierarchy)
            {
                item.gameObject.SetActive(true);
            }

            RenderItem(item, data);

            return item;
        }
    }
}