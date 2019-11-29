using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    public enum EGridConstraint
    {
        /// <summary>
        /// 根据视口自动填充
        /// </summary>
        FLEXIBLE,

        /// <summary>
        /// 固定列数
        /// </summary>
        FIXED_COLUMN_COUNT,

        /// <summary>
        /// 固定行数
        /// </summary>
        FIXED_ROW_COUNT,
    }

    public class GridScrollList : GridScrollList<object>
    {
        public GridScrollList(GameObject scrollView, OnRenderItem itemRender, Vector2 gap, EGridConstraint constraint, int constraintCount = 0) : base(scrollView, itemRender, gap, constraint, constraintCount)
        {

        }

        public GridScrollList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, Vector2 gap, EGridConstraint constraint, int constraintCount = 0) : base(scrollView, itemPrefab, itemRender, gap, constraint, constraintCount)
        {
        }

        public virtual void AddRange<TInput>(IEnumerable<TInput> collection)
        {
            foreach (var data in collection)
            {
                Add(data);
            }
        }
    }

    public class GridScrollList<TData> : BaseScrollList<TData>
    {
        /// <summary>
        /// 列表项之间的间距
        /// </summary>
        public Vector2 gap { get; private set; }

        /// <summary>
        /// 列表项排列方式限定
        /// </summary>
        public EGridConstraint constraint { get; private set; }

        /// <summary>
        /// 当constraint不为「FLEXIBLE」，根据该值确定列数或者行数
        /// </summary>
        public int constraintCount { get; private set; }

        public GridScrollList(GameObject scrollView, OnRenderItem itemRender, Vector2 gap, EGridConstraint constraint, int constraintCount = 0)
        {
            this.gap = gap;
            this.constraint = constraint;
            this.constraintCount = constraintCount;

            var scrollRect = scrollView.GetComponent<ScrollRect>();
            var itemPrefab = scrollRect.content.GetChild(0);
            itemPrefab.gameObject.SetActive(false);
            itemPrefab.SetParent(scrollView.transform);

            Init(scrollView, itemPrefab.gameObject, itemRender);
        }

        public GridScrollList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, Vector2 gap, EGridConstraint constraint, int constraintCount = 0)
        {
            this.gap = gap;
            this.constraint = constraint;
            this.constraintCount = constraintCount;

            Init(scrollView, itemPrefab, itemRender);

            var glg = content.gameObject.AddComponent<GridLayoutGroup>();
            glg.spacing = gap;
            glg.cellSize = itemPrefab.GetComponent<RectTransform>().sizeDelta;
            var csf = content.gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        protected override bool AdjustmentItemSize(ScrollListItem item)
        {
            return false;
        }

        protected override void Refresh(UpdateData updateConfig, out int lastStartIndex)
        {
            lastStartIndex = 0;

            for (int i = 0; i < content.transform.childCount; i++)
            {
                var t = content.transform.GetChild(i);
                var item = t.GetComponent<ScrollListItem>();
                if (item.index != i)
                {
                    item.index = i;
                    RenderItem(item, (TData)item.data);
                }
            }
        }

        protected override void ResizeContent(UpdateData updateConfig)
        {
            //计算Content大小

        }

        ScrollListItem Create(TData data)
        {
            var go = GameObject.Instantiate(itemPrefab, content);
            var item = go.AddComponent<ScrollListItem>();
            item.rectTransform.anchorMin = Vector2.up;
            item.rectTransform.anchorMax = Vector2.up;
            item.rectTransform.pivot = Vector2.up;
            item.gameObject.name = $"item_{content.transform.childCount}";
            item.data = data;
            item.index = item.transform.GetSiblingIndex();
            return item;
        }

        public override void AddRange(IEnumerable<TData> collection)
        {
            foreach (var data in collection)
            {
                Add(data);
            }
        }

        public override void Add(TData data)
        {
            RenderItem(Create(data), data);
        }

        public override void Insert(int index, TData data)
        {
            var item = Create(data);
            item.transform.SetSiblingIndex(index);
            item.index = index;
            RenderItem(item, data);
            MarkDirty();
        }

        public override bool Remove(TData data)
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                var t = content.transform.GetChild(i);
                if (t.GetComponent<ScrollListItem>().data.Equals(data))
                {
                    GameObject.Destroy(t.gameObject);
                    MarkDirty();
                    return true;
                }
            }
            return false;
        }

        public override void RemoveAt(int index)
        {
            var t = content.transform.GetChild(index);
            GameObject.Destroy(t.gameObject);
            MarkDirty();
        }

        public override void Clear()
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                var t = content.transform.GetChild(i);
                GameObject.Destroy(t.gameObject);
            }
        }
    }
}
