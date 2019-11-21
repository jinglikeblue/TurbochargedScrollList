using System.Collections.Generic;
using UnityEngine;
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

    public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, float gap = 0) : base(scrollView, itemPrefab, gap)
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

    protected override void Refresh()
    {        
        //滚动位置
        var scrollY = 1 - scrollPos.y;        

        //内容容器高度
        var contentHeight = content.sizeDelta.y;
        
        contentScrollPos = scrollY * (contentHeight - scrollRect.viewport.rect.height);        

        //通过内容容器滚动位置，计算显示的数据索引(这里+gap让索引开始的位置更精确)
        int dataIdx = (int)((contentScrollPos + gap) / itemHeightWithGap);

        if(dataIdx < 0)
        {
            dataIdx = 0;
        }

        //通过索引反推出精确的渲染坐标
        float startPos = -1 * dataIdx * itemHeightWithGap;

        //Debug.LogFormat("滚动比例:{0}, Content滚动位置:{1}, 显示的开始索引:{2} 开始的位置:{3}", scrollY, contentScrollPos, dataIdx, startPos);

        var recycledItems = RecycleItems();
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
            
            if (usedRecycleItem < recycledItems.Count)
            {
                go = recycledItems[usedRecycleItem++];
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
            _itemMap[data] = go;

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

        for(int i = usedRecycleItem; i < recycledItems.Count; i++)
        {
            GameObject.Destroy(recycledItems[i]);
        }
    }

    Dictionary<TData, GameObject> _itemMap = new Dictionary<TData, GameObject>();
    
    /// <summary>
    /// 回收列表项
    /// </summary>
    /// <returns></returns>
    List<GameObject> RecycleItems()
    {               
        List<GameObject> recycledItems = new List<GameObject>();

        float viewportMinY = contentScrollPos;
        float viewportMaxY = contentScrollPos + scrollRect.viewport.rect.height;

        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i).GetComponent<RectTransform>();
            var top = -child.localPosition.y;
            var bottom = top + child.rect.height;
            if (bottom >= viewportMinY && top < viewportMaxY)
            {
                //不用刷新数据
            }
            else
            {
                Debug.Log("回收并刷新数据:" + child.name);
            }
            recycledItems.Add(child.gameObject);
        }
        return recycledItems;
    }
}
