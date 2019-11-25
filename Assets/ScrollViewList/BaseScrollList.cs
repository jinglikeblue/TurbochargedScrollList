using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 基于UGUI中Scroll View组件的列表工具
    /// </summary>
    public abstract class BaseScrollList<TData>
    {
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

        /// <summary>
        /// 列表项数据
        /// </summary>
        protected readonly List<ScrollListItemModel<TData>> _itemModels = new List<ScrollListItemModel<TData>>();

        OnRenderItem _itemRender;

        /// <summary>
        /// 内容容器滚动位置
        /// </summary>
        public float contentRenderStartPos { get; protected set; }               

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

        ScrollListAdapter _proxy;

        public BaseScrollList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap)
        {
            Init(scrollView);            
            this.gap = gap;
            this.itemPrefab = itemPrefab;
            itemDefaultfSize = itemPrefab.GetComponent<RectTransform>().sizeDelta;

            _itemRender = itemRender;
            
            scrollPos = Vector2.up;

            _proxy = scrollView.GetComponent<ScrollListAdapter>();
            if(null == _proxy)
            {
                _proxy = scrollView.AddComponent<ScrollListAdapter>();
            }
            _proxy.onUpdate += Update;
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
            TData[] datas = new TData[_itemModels.Count];
            for(int i = 0; i < _itemModels.Count; i++)
            {
                datas[i] = _itemModels[i].data;
            }

            return datas;
        }

        private void OnScroll(Vector2 v)
        {
            scrollPos = v;
            MarkDirty(EUpdateType.REFRESH);
        }

        public void AddDatas(IEnumerable<TData> collection)
        {           
            foreach(var data in collection)
            {
                AddData(data);
            }
        }        

        public void AddData(TData data)
        {
            var model = new ScrollListItemModel<TData>(data, itemDefaultfSize);
            _itemModels.Add(model);
            MarkDirty(EUpdateType.REBUILD);
        }

        public void AddDataAt(TData data, int index)
        {
            MarkDirty(EUpdateType.REBUILD);
        }

        public void RemoveAt(int index)
        {
            MarkDirty(EUpdateType.REBUILD);
        }

        /// <summary>
        /// 移除列表找到的第一个和data数据相同的项
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Remove(TData data)
        {
            return false;
        }

        protected void MarkDirty(EUpdateType updateType)
        {
            if (_updateType == updateType || _updateType == EUpdateType.REBUILD)
            {
                return;
            }

            _updateType = updateType;
        }

        void Update()
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

        /// <summary>
        /// 清空列表，实际上是将数据清除，并将对象放入对象池
        /// </summary>
        public void Clear()
        {
            _recycledItems.Clear();
            _showingItems.Clear();
            _itemModels.Clear();            
            int childIdx = content.childCount;
            while (--childIdx > -1)
            {
                var item = content.GetChild(childIdx).GetComponent<ScrollListItem>();
                item.gameObject.SetActive(false);
                _recycledItems.Add(item);
            }
            MarkDirty(EUpdateType.REBUILD);

            //SetDatas(new TData[0]);
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