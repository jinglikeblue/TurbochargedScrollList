using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 基于UGUI中Scroll View组件的列表工具
    /// </summary>
    public abstract class BaseScrollList<TData>: IScrollList<TData>
    {
        /// <summary>
        /// 更新配置
        /// </summary>
        protected struct UpdateConfig
        {
            /// <summary>
            /// 是否重新计算Content大小
            /// </summary>
            public bool isResizeContent;

            /// <summary>
            /// 是否刷新
            /// </summary>
            public bool isRefresh;

            /// <summary>
            /// 最后开始的数据索引
            /// </summary>
            public int lastStartIndex;

            /// <summary>
            /// 最后渲染开始的位置
            /// </summary>
            public float lastRenderStartPos;

            public void Reset()
            {
                isResizeContent = false;
                isRefresh = false;
                lastStartIndex = 0;
                lastRenderStartPos = 0;
            }
        }

        protected UpdateConfig _updateConfig;


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
        protected readonly Dictionary<ScrollListItemModel<TData>, ScrollListItem> _showingItems = new Dictionary<ScrollListItemModel<TData>, ScrollListItem>();

        /// <summary>
        /// 回收列表项
        /// </summary>
        protected readonly List<ScrollListItem> _recycledItems = new List<ScrollListItem>();

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
            if (false == viewportSize.Equals(scrollRect.viewport.rect.size))
            {
                viewportSize = scrollRect.viewport.rect.size;
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
            MarkDirty();
        }

        protected void MarkDirty(bool isResizeContent = false)
        {
            _updateConfig.isRefresh = true;
            _updateConfig.isResizeContent |= isResizeContent;           
        }

        void Update()
        {
            var updateConfig = _updateConfig;
            _updateConfig.Reset();

            UpdateViewportSize();

            if (viewportSize.Equals(Vector2.zero))
            {
                //还没初始化完成，刷新延迟
                //Debug.Log("还没初始化完成，刷新延迟");
                MarkDirty(true);
                return;
            }

            if (updateConfig.isResizeContent)
            {
                ResizeContent();                
            }

            if (updateConfig.isRefresh)
            {                
                Refresh();                
            }

            CheckItemsSize();            
        }

        /// <summary>
        /// 检查每一个现实中的item的大小
        /// </summary>
        protected void CheckItemsSize()
        {
            foreach (var item in _showingItems.Values)
            {
                if (AdjustmentItemSize(item))
                {
                    MarkDirty(true);
                }
            }
        }

        /// <summary>
        /// 重新计算content大小
        /// </summary>
        protected abstract void ResizeContent();

        /// <summary>
        /// 刷新列表
        /// </summary>
        protected abstract void Refresh();      

        /// <summary>
        /// 检测并校正item的大小，如果进行了校正，返回true
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract bool AdjustmentItemSize(ScrollListItem item);

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
        protected ScrollListItem CreateItem(ScrollListItemModel<TData> model, int dataIdx, Dictionary<ScrollListItemModel<TData>, ScrollListItem> lastShowingItems)
        {
            ScrollListItem item;
            if (lastShowingItems.ContainsKey(model))
            {
                item = lastShowingItems[model];
                lastShowingItems.Remove(model);
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
                    if (tempItem.index == dataIdx && tempItem.data.Equals(model.data))
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
            item.data = model.data;
            item.width = _itemModels[dataIdx].width;
            item.height = _itemModels[dataIdx].height;

            if (false == item.gameObject.activeInHierarchy)
            {
                item.gameObject.SetActive(true);
            }

            RenderItem(item, model.data);

            return item;
        }


        #region 数据操作
        public void AddRange(IEnumerable<TData> collection)
        {                              
            foreach (var data in collection)
            {
                Add(data);
            }
        }

        public void Add(TData data)
        {
            var model = new ScrollListItemModel<TData>(data, itemDefaultfSize);
            _itemModels.Add(model);
            MarkDirty(true);
        }

        public void Insert(int index, TData data)
        {
            var model = new ScrollListItemModel<TData>(data, itemDefaultfSize);
            _itemModels.Insert(index, model);
            MarkDirty(true);
        }

        /// <summary>
        /// 移除列表找到的第一个和data数据相同的项
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Remove(TData data)
        {
            for(int i = 0;i < _itemModels.Count; i++)
            {
                if(_itemModels[i].data.Equals(data))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除指定索引位置的数据
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _itemModels.RemoveAt(index);
            MarkDirty(true);
        }

        /// <summary>
        /// 清空列表，实际上是将数据清除，并将对象放入对象池
        /// </summary>
        public void Clear()
        {
            _updateConfig = new UpdateConfig();
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
            MarkDirty(true);            
        }
        #endregion
    }
}