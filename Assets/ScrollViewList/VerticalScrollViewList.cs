using System.Collections.Generic;
using UnityEngine;
namespace Jing.ScrollViewList
{
    /// <summary>
    /// 垂直滚动列表
    /// </summary>
    public class VerticalScrollViewList<TData> : BaseScrollViewList<TData>
    {
        /// <summary>
        /// 内容容器滚动位置
        /// </summary>
        public float contentScrollPos { get; private set; }

        /// <summary>
        /// 列表项高度加上间距的值
        /// </summary>
        public float itemHeightWithGap { get; private set; }

        /// <summary>
        /// 可视区域高度
        /// </summary>
        public float viewportHeight { get; private set; }

        public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, OnRenderItem itemRender, float gap = 0) : base(scrollView, itemPrefab, itemRender, gap)
        {
            itemHeightWithGap = itemSize.y + gap;
        }

        protected override void OnSetDatas()
        {
            //计算content高度
            var h = _datas.Length * itemSize.y;
            //加上间距
            h += ((_datas.Length - 1) * gap);

            SetContentSize(itemSize.x, h);

            Refresh();
        }

        void UpdateViewportHeight()
        {
            viewportHeight = scrollRect.viewport.rect.height;
            if (0 == viewportHeight)
            {
                viewportHeight = scrollRect.GetComponent<RectTransform>().rect.height;
            }
        }

        protected override void Refresh()
        {
            //滚动位置
            var scrollY = 1 - scrollPos.y;

            //内容容器高度
            var contentHeight = content.sizeDelta.y;

            UpdateViewportHeight();

            contentScrollPos = scrollY * (contentHeight - viewportHeight);

            //通过内容容器滚动位置，计算显示的数据索引(这里+gap让索引开始的位置更精确)
            int dataIdx = (int)((contentScrollPos + gap) / itemHeightWithGap);

            if (dataIdx < 0)
            {
                dataIdx = 0;
            }

            //通过索引反推出精确的渲染坐标
            float startPos = -1 * dataIdx * itemHeightWithGap;

            //Debug.LogFormat("滚动比例:{0}, Content滚动位置:{1}, 显示的开始索引:{2} 开始的位置:{3}", scrollY, contentScrollPos, dataIdx, startPos);

            List<ScrollListItem> recycledItems;
            HashSet<ScrollListItem> reusableItems;
            RecycleItems(out recycledItems, out reusableItems);

            int usedRecycleItem = 0;
            //显示的内容刚好大于这个值即可           
            float contentHeightLimit = viewportHeight;
            float itemY = startPos;
            Dictionary<int, ScrollListItem> lastItemMap = new Dictionary<int, ScrollListItem>(_itemMap);
            _itemMap.Clear();
            do
            {
                if (dataIdx >= _datas.Length)
                {
                    break;
                }

                var data = _datas[dataIdx];
                
                ScrollListItem item;                

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

                    item.gameObject.name = string.Format("item_{0}", dataIdx);

                    //先渲染，再定位
                    item.index = dataIdx;
                    item.data = data;
                    RenderItem(item, data);
                }
                
                _itemMap[dataIdx] = item;

                var pos = Vector3.zero;
                pos.y = itemY;
                item.rectTransform.localPosition = pos;
                //下一个item的Y坐标
                itemY -= (item.rectTransform.rect.height + gap);
                //下一个item的索引
                dataIdx++;

                if (-contentScrollPos - itemY >= contentHeightLimit)
                {
                    //显示区域已满
                    break;
                }
            }
            while (true);

            for (int i = usedRecycleItem; i < recycledItems.Count; i++)
            {
                GameObject.Destroy(recycledItems[i].gameObject);
            }
        }

        /// <summary>
        /// 当前显示的Item表
        /// </summary>
        Dictionary<int, ScrollListItem> _itemMap = new Dictionary<int, ScrollListItem>();

        /// <summary>
        /// 回收列表项
        /// </summary>
        /// <returns></returns>
        void RecycleItems(out List<ScrollListItem> recycledItems, out HashSet<ScrollListItem> reusableItems)
        {
            recycledItems = new List<ScrollListItem>();
            reusableItems = new HashSet<ScrollListItem>();

            float viewportMinY = contentScrollPos;
            float viewportMaxY = contentScrollPos + viewportHeight;

            foreach (var item in _itemMap.Values)
            {
                var top = -item.rectTransform.localPosition.y;
                var bottom = top + item.height;
                if (bottom >= viewportMinY && top < viewportMaxY)
                {
                    //不用刷新数据，仅需要调整位置
                    reusableItems.Add(item);
                }
                else
                {
                    //Debug.Log("回收并刷新数据:" + child.name);
                    recycledItems.Add(item);
                }
            }

            //for (int i = 0; i < content.childCount; i++)
            //{
            //    var t = content.GetChild(i);
            //    var child = t.GetComponent<ScrollListItem>();
            //    var top = -child.rectTransform.localPosition.y;
            //    var bottom = top + child.height;
            //    if (bottom >= viewportMinY && top < viewportMaxY)
            //    {
            //        //不用刷新数据，仅需要调整位置
            //        reusableItems.Add(child);
            //    }
            //    else
            //    {
            //        //Debug.Log("回收并刷新数据:" + child.name);
            //        recycledItems.Add(child);
            //    }
            //}
        }
    }
}