using System;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.ScrollViewList
{
    /// <summary>
    /// 基于UGUI中Scroll View组件的列表工具
    /// </summary>
    public abstract class BaseScrollViewList<TData>
    {
        protected class ItemModel<T>
        {
            public readonly T data;

            public float height;

            public float width;

            public readonly GameObject itemPrefab;

            public ItemModel(T data, GameObject itemPrefab)
            {
                this.data = data;
                this.itemPrefab = itemPrefab;
                var rt = itemPrefab.GetComponent<RectTransform>();
                height = rt.rect.height;
                width = rt.rect.width;
            }
        }

        public delegate void OnRenderItem(ScrollListItem item, TData data);

        public ScrollRect scrollRect { get; private set; }

        public RectTransform content { get; private set; }

        public GameObject gameObject { get; private set; }

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

        public BaseScrollViewList(GameObject scrollView, OnRenderItem itemRender, float gap)
        {
            Init(scrollView);            
            this.gap = gap;
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
        /// 刷新显示视口的高度
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
            OnScroll();
        }

        public void SetDatas(TData[] datas, GameObject itemPrefab)
        {
            _datas = new TData[datas.Length];            
            Array.Copy(datas, _datas, _datas.Length);
            Clear();            
            OnSetDatas(itemPrefab);
        }

        protected abstract void OnSetDatas(GameObject itemPrefab);

        protected abstract void OnScroll();

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
    }
}