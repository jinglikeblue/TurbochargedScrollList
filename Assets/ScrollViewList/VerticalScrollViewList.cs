using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 垂直滚动列表
/// </summary>
public class VerticalScrollViewList<TData> : BaseScrollViewList<TData>
{
    public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, float gap = 0) : base(scrollView, itemPrefab, gap)
    {
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

    protected override void Refresh()
    {
        var itemHeightWithGap = itemSize.y + gap;

        //滚动位置
        var scrollY = 1 - scrollPos.y;        

        //内容容器高度
        var contentHeight = content.sizeDelta.y;

        //内容容器滚动位置
        var contentScrollPos = scrollY * (contentHeight - scrollRect.viewport.rect.height);

        Debug.LogFormat("滚动位置:{0}, Content位置:{1}", scrollY, contentScrollPos);

        //通过内容容器滚动位置，计算显示的数据索引
        int dataIdx = (int)(contentScrollPos / itemHeightWithGap);

        if(dataIdx < 0)
        {
            dataIdx = 0;
        }

        //通过索引反推出精确的渲染坐标
        float startPos = -1 * dataIdx * itemHeightWithGap;

        Debug.LogFormat("显示的开始索引:{0} 开始的位置:{1}", dataIdx, startPos);

        var items = RecycleItems();
        int usedRecycleItem = 0;
        //显示的内容刚好大于这个值即可           
        float contentHeightLimit = scrollRect.viewport.rect.height;
        float itemY = startPos;
        do
        {
            if(dataIdx >= _datas.Length)
            {
                break;
            }

            var data = _datas[dataIdx];
            GameObject go;
            RectTransform rt;
            if (usedRecycleItem < items.Count)
            {
                go = items[usedRecycleItem++];
                rt = go.GetComponent<RectTransform>();
            }
            else
            {
                go = GameObject.Instantiate(itemPrefab, content);
                rt = go.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.up;
                rt.anchorMax = Vector2.up;
                rt.pivot = Vector2.up;
            }

            go.name = string.Format("item_{0}", dataIdx);

            //先渲染，再定位
            RenderItem(dataIdx, go, data);

            var pos = Vector3.zero;
            pos.y = itemY;
            rt.localPosition = pos;
            //下一个item的Y坐标
            itemY -= (rt.rect.height + gap);
            //下一个item的索引
            dataIdx++;

            if(-contentScrollPos - itemY >= contentHeightLimit)
            {
                //显示区域已满
                break;
            }
        }
        while (true);

        for(int i = usedRecycleItem; i < items.Count; i++)
        {
            GameObject.Destroy(items[i]);
        }
    }
    
    /// <summary>
    /// 回收列表项
    /// </summary>
    /// <returns></returns>
    List<GameObject> RecycleItems()
    {
        List<GameObject> recycledItems = new List<GameObject>();
        for(int i = 0; i < content.childCount; i++)
        {
            recycledItems.Add(content.GetChild(i).gameObject);
        }
        return recycledItems;
    }
}
