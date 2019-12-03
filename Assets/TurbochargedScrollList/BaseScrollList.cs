using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 列表项被复用前的委托
    /// </summary>
    /// <param name="item"></param>
    public delegate void OnItemBeforeReuse(ScrollListItem item);

    /// <summary>
    /// 基于UGUI中Scroll View组件的列表工具
    /// </summary>
    public abstract class BaseScrollList<TData> : IScrollList<TData>
    {
        /// <summary>
        /// 渲染列表项的委托
        /// </summary>
        /// <param name="item">列表项</param>
        /// <param name="data">列表项对应的数据</param>
        /// <param name="isFresh">如果为true, 则item的数据或index产生了改变。如果为false，则仅仅是Active从false变为了true</param>
        public delegate void OnRenderItem(ScrollListItem item, TData data, bool isFresh);

        public enum EKeepPaddingType
        {
            /// <summary>
            /// 不刻意保持间距
            /// </summary>
            NONE,

            /// <summary>
            /// 保持和内容起始的间距
            /// </summary>
            START,

            /// <summary>
            /// 保持和内容结束位置的间距
            /// </summary>
            END
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        protected struct UpdateData
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
            /// 渲染时保持间距的配置
            /// </summary>
            public EKeepPaddingType keepPaddingType;

            /// <summary>
            /// 如果更新时更新了Content大小，那么该字段将在本次刷新中保留之前Content的矩形大小
            /// </summary>
            public Rect tempLastContentRect;

            public void Reset()
            {
                isResizeContent = false;
                isRefresh = false;
                keepPaddingType = EKeepPaddingType.NONE;
            }
        }

        protected UpdateData _updateData;

        public ScrollRect scrollRect { get; private set; }

        public RectTransform content { get; private set; }

        public GameObject gameObject { get; private set; }

        public GameObject itemPrefab { get; private set; }

        public Vector2 itemDefaultfSize { get; private set; }

        public int ItemCount
        {
            get
            {
                return _itemModels.Count;
            }
        }

        /// <summary>
        /// 显示内容宽度
        /// </summary>
        public float ContentWidth
        {
            get
            {
                return content.rect.width;
            }
        }

        /// <summary>
        /// 显示内容高度
        /// </summary>
        public float ContentHeight
        {
            get
            {
                return content.rect.height;
            }
        }

        /// <summary>
        /// 视口大小
        /// </summary>
        public Vector2 viewportSize { get; private set; }

        public Vector2 scrollPos { get; protected set; }

        /// <summary>
        /// 列表项数据
        /// </summary>
        protected readonly List<ScrollListItemModel<TData>> _itemModels = new List<ScrollListItemModel<TData>>();

        OnRenderItem _itemRender;

        public event OnItemBeforeReuse onItemBeforeReuse;

        /// <summary>
        /// 重构内容的事件
        /// </summary>
        public event Action onRebuildContent;

        /// <summary>
        /// 刷新的事件
        /// </summary>
        public event Action onRefresh;

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

        /// <summary>
        /// 最后一次刷新开始的数据索引
        /// </summary>
        public int lastStartIndex { get; private set; }

        protected void InitScrollView(GameObject scrollView)
        {
            scrollRect = scrollView.GetComponent<ScrollRect>();

            content = scrollRect.content;
            content.localPosition = Vector3.zero;
        }

        protected void InitItem(GameObject itemPrefab, OnRenderItem itemRender)
        {
            this.itemPrefab = itemPrefab;
            itemDefaultfSize = itemPrefab.GetComponent<RectTransform>().rect.size;

            _itemRender = itemRender;

            scrollPos = Vector2.up;

            _proxy = scrollRect.gameObject.GetComponent<ScrollListAdapter>();
            if (null == _proxy)
            {
                _proxy = scrollRect.gameObject.AddComponent<ScrollListAdapter>();
            }

            //监听事件
            _proxy.onUpdate += Update;
            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        /// <summary>
        /// 自动设置列表项（从Content中找到列表项的Prefab）
        /// </summary>
        /// <param name="itemRender"></param>
        protected void AutoInitItem(OnRenderItem itemRender)
        {
            var itemPrefab = FindItemPrefabFromContent();
            if (null == itemPrefab)
            {
                throw new System.Exception("Can't Find Item Prefab From Content!!!");
            }

            InitItem(itemPrefab, itemRender);
        }

        /// <summary>
        /// 从Content中找到列表项的prefab        
        /// </summary>
        /// <param name="scrollView"></param>
        /// <returns></returns>
        GameObject FindItemPrefabFromContent()
        {
            var itemTransform = content.GetChild(0);
            if (null == itemTransform)
            {
                return null;
            }

            //换个位置存放 
            itemTransform.SetParent(scrollRect.transform);

            this.itemPrefab = itemTransform.gameObject;

            itemPrefab.SetActive(false);

            return itemPrefab;
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

        protected void RenderItem(ScrollListItem item, TData data, bool isOnlySwitchActive)
        {
            _itemRender.Invoke(item, data, isOnlySwitchActive);
        }

        /// <summary>
        /// 获取数据拷贝
        /// </summary>
        public TData[] GetDatasCopy()
        {
            TData[] datas = new TData[_itemModels.Count];
            for (int i = 0; i < _itemModels.Count; i++)
            {
                datas[i] = _itemModels[i].data;
            }

            return datas;
        }

        private void OnScroll(Vector2 v)
        {
            MarkDirty();
        }

        /// <summary>
        /// 标记列表有变化，将在下一次更新时进行刷新
        /// </summary>
        /// <param name="isResizeContent"></param>
        protected void MarkDirty(bool isResizeContent = false, EKeepPaddingType keepPaddingType = EKeepPaddingType.NONE)
        {
            _updateData.isRefresh = true;
            _updateData.isResizeContent |= isResizeContent;
            if (keepPaddingType != EKeepPaddingType.NONE)
            {
                _updateData.keepPaddingType = keepPaddingType;
            }
        }

        void Update()
        {
            var updateConfig = _updateData;
            _updateData.Reset();

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
                updateConfig.tempLastContentRect = content.rect;
                ResizeContent(updateConfig);
                onRebuildContent?.Invoke();
            }

            if (updateConfig.isRefresh)
            {
                int lastStartIndex;
                Refresh(updateConfig, out lastStartIndex);
                this.lastStartIndex = lastStartIndex;
                onRefresh?.Invoke();
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
        protected abstract void ResizeContent(UpdateData updateConfig);

        /// <summary>
        /// 刷新列表
        /// </summary>
        protected abstract void Refresh(UpdateData updateConfig, out int lastStartIndex);

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
            if (lastShowingItems.ContainsKey(model) && lastShowingItems[model].index == dataIdx) //本来就在显示列表中，直接返回
            {
                item = lastShowingItems[model];
                lastShowingItems.Remove(model);
            }
            else
            {
                if (_recycledItems.Count > 0)
                {
                    ScrollListItem tempItem = null;
                    //tempItem = _recycledItems[0];
                    int idx = _recycledItems.Count;
                    while (--idx > -1)
                    {
                        tempItem = _recycledItems[idx];
                        if (tempItem.index == dataIdx && tempItem.data.Equals(model.data)) //以前回收的一个对象，刚好对应的数据一致，直接返回
                        {
                            item = tempItem;
                            _recycledItems.RemoveAt(idx);
                            item.gameObject.SetActive(true);
                            RenderItem(item, model.data, false);
                            return item;
                        }
                    }

                    //没有绝对匹配的，则抽取最后找到的item，之后刷新数据并返回
                    item = tempItem;
                    _recycledItems.Remove(tempItem);

                    //当一个列表项，将被复用时触发
                    onItemBeforeReuse?.Invoke(item);
                }
                else
                {
                    var go = GameObject.Instantiate(itemPrefab, content);
                    if (false == go.activeInHierarchy)
                    {
                        go.SetActive(true);
                    }
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

                RenderItem(item, model.data, true);
            }

            if (item.index != dataIdx)
            {
                //插入列表项可能导致数据索引改变
                item.index = dataIdx;
            }

            return item;
        }

        /// <summary>
        /// 滚动列表到列表项
        /// </summary>
        /// <param name="index"></param>
        public abstract void ScrollToItem(int index);

        /// <summary>
        /// 滚动列表到指定位置
        /// </summary>
        /// <param name="position"></param>
        public virtual void ScrollToPosition(Vector2 position)
        {
            content.localPosition = new Vector3(-position.x, position.y, 0);
            OnScroll(position);            
        }

        /// <summary>
        /// 距离列表末尾的距离
        /// </summary>
        public Vector2 GetDistanceToEnd()
        {
            Vector2 distance;

            var maxX = content.rect.width - viewportSize.x;
            var posX = content.localPosition.x;
            if(posX > 0)
            {
                posX = 0;
            }
            distance.x = maxX + posX;
            if(distance.x < 0)
            {
                distance.x = 0;
            }

            var maxY = content.rect.height - viewportSize.y;
            var posY = content.localPosition.y;
            if (posY < 0)
            {
                posY = 0;
            }
            distance.y = maxY - posY;
            if (distance.y < 0)
            {
                distance.y = 0;
            }

            return distance;
        }


        /// <summary>
        /// 检查指定的列表项是否在显示列表中
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckItemShowing(int index)
        {
            foreach(var item in _showingItems.Values)
            {
                if(item.index == index)
                {
                    return true;
                }
            }
            return false;
        }


        #region 数据操作
        public virtual void AddRange(IEnumerable<TData> collection)
        {            
            foreach (var data in collection)
            {
                Add(data);
            }
        }

        public virtual void Add(TData data)
        {
            var model = new ScrollListItemModel<TData>(data, itemDefaultfSize);
            _itemModels.Add(model);
            MarkDirty(true);
        }

        public virtual void Insert(int index, TData data)
        {
            var model = new ScrollListItemModel<TData>(data, itemDefaultfSize);
            _itemModels.Insert(index, model);
            if (index <= lastStartIndex)
            {
                MarkDirty(true, EKeepPaddingType.END);
            }
            else
            {
                MarkDirty(true);
            }
        }

        /// <summary>
        /// 移除列表找到的第一个和data数据相同的项
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool Remove(TData data)
        {
            for (int i = 0; i < _itemModels.Count; i++)
            {
                if (_itemModels[i].data.Equals(data))
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
        public virtual void RemoveAt(int index)
        {
            _itemModels.RemoveAt(index);
            if (index <= lastStartIndex)
            {
                MarkDirty(true, EKeepPaddingType.END);
            }
            else
            {
                MarkDirty(true);
            }
        }

        /// <summary>
        /// 清空列表，实际上是将数据清除，并将对象放入对象池
        /// </summary>
        public virtual void Clear()
        {
            _updateData = new UpdateData();
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