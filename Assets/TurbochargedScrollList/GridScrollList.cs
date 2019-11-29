using System;
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
            var scrollX = -content.localPosition.x;
            var scrollY = content.localPosition.y;                       

            switch (constraint)
            {
                case EGridConstraint.FLEXIBLE:
                case EGridConstraint.FIXED_COLUMN_COUNT:
                    lastStartIndex = RefreshRowByRow(scrollX, scrollY);
                    break;
                case EGridConstraint.FIXED_ROW_COUNT:
                    lastStartIndex = RefreshColByCol(scrollX, scrollY);
                    break;
                default:
                    throw new Exception($"Wrong constraint:{constraint}");
            }            
        }

        /// <summary>
        /// 从左往右，从上往下依次刷新
        /// ↓ ↓ ↓ Example
        /// 0 1 2
        /// 3 4 5
        /// 6 7 
        /// </summary>
        int RefreshRowByRow(float scrollX, float scrollY)
        {
            float bigW = itemDefaultfSize.x + gap.x;
            float bigH = itemDefaultfSize.y + gap.y;

            int startIndex = 0;

            //根据滚动区域左上角算出起始item的二维位置;
            Vector2Int startPos = new Vector2Int((int)(scrollX / bigW), (int)(scrollY / bigH));
            //根据滚动区域左下角算出结束item的二维位置
            Vector2Int endPos = new Vector2Int((int)(scrollX + content.rect.width / bigW), (int)(scrollY + content.rect.height / bigH));
            //计算出关联的所有Item的索引


            return startIndex;
        }

        /// <summary>
        /// 从上往下，从左往右依次刷新
        /// ↓ ↓ ↓ Example
        /// 0 3 6 
        /// 1 4 7
        /// 2 5 
        /// </summary>
        int RefreshColByCol(float scrollX, float scrollY)
        {
            float bigW = itemDefaultfSize.x + gap.x;
            float bigH = itemDefaultfSize.y + gap.y;

            int startIndex = 0;
            return startIndex;
        }

        protected override void ResizeContent(UpdateData updateConfig)
        {
            float contentW = 0;
            float contentH = 0 ;
            int itemAmount = _itemModels.Count;

            float bigW = itemDefaultfSize.x + gap.x;
            float bigH = itemDefaultfSize.y + gap.y;            

            //计算Content大小
            switch (constraint)
            {
                case EGridConstraint.FLEXIBLE: //根据视口确定Content大小，并且计算出constraint数量
                    contentW = viewportSize.x;
                    //计算出constraintCount
                    constraintCount = (int)((contentW + gap.x) / bigW);
                    //确定高度,通过item总数和constraintCount算出                    
                    if(itemAmount > 0)
                    {
                        int row = (itemAmount - 1) / constraintCount + 1;
                        contentH = row * bigH - gap.y;
                    }                    
                    break;
                case EGridConstraint.FIXED_COLUMN_COUNT: //根据列数确定Content大小
                    contentW = constraintCount * (itemDefaultfSize.x + gap.x) - gap.x;
                    //确定高度,通过item总数和constraintCount算出
                    if (itemAmount > 0)
                    {
                        int row = (itemAmount - 1) / constraintCount + 1;
                        contentH = row * bigH - gap.y;
                    }
                    break;
                case EGridConstraint.FIXED_ROW_COUNT: //根据行数确定Content大小
                    contentH = constraintCount * (itemDefaultfSize.y + gap.y) - gap.y;
                    //确定宽度,通过item总数和constraintCount算出                    
                    if(itemAmount > 0)
                    {
                        int col = (itemAmount - 1) / constraintCount + 1;
                        contentW = col * bigW - gap.x;
                    }
                    break;
            }

            SetContentSize(contentW, contentH);
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
