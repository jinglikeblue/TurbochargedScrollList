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
        public delegate void OnRenderItem(ScrollListItem item, TData data);

        public ScrollRect scrollRect { get; private set; }

        public RectTransform content { get; private set; }

        public GameObject gameObject { get; private set; }

        /// <summary>
        /// 列表项Prefab
        /// </summary>
        public GameObject itemPrefab { get; private set; }

        /// <summary>
        /// 列表项大小
        /// </summary>
        public Vector2 itemSize { get; private set; }

        /// <summary>
        /// 列表项间距
        /// </summary>
        public float gap { get; private set; }

        public Vector2 scrollPos { get; protected set; }

        protected TData[] _datas;

        OnRenderItem _itemRender;

        public BaseScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap)
        {
            Init(scrollView);
            this.itemPrefab = itemPrefab;
            this.gap = gap;
            _itemRender = itemRender;

            itemSize = itemPrefab.GetComponent<RectTransform>().sizeDelta;
            scrollPos = Vector2.up;
        }

        void Init(GameObject gameObject)
        {
            scrollRect = gameObject.GetComponent<ScrollRect>();

            content = scrollRect.content;
            content.localPosition = Vector3.zero;

            scrollRect.onValueChanged.AddListener(OnScroll);
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

        public void SetDatas(TData[] datas)
        {
            _datas = new TData[datas.Length];            
            Array.Copy(datas, _datas, _datas.Length);
            Clear();            
            OnSetDatas();
        }

        protected abstract void OnSetDatas();

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